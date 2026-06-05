using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem; 

public class FoodController : MonoBehaviour
{
    [Header("Positions")]
    public Transform basketPos;
    public Transform boardPos;
    public Transform potPos;
    public Transform platePos;
    public Vector3 jamOffset; // NEW: Use this in the Inspector to nudge the jam perfectly into place!

    public float moveSpeed = 5f;
    public CameraSlide cameraSlide;

    [Header("Sprites")]
    public Sprite wholeSprite;
    public Sprite twoPieceSprite;
    public Sprite fourPieceSprite;
    public Sprite jamSprite;
    public Sprite cakeSprite;

    [Header("UI Elements")]
    public GameObject introScreen;
    public GameObject infoText; 
    public GameObject instruction_1;
    public GameObject instruction_2;
    public GameObject instruction_3;
    public GameObject instruction_4;
    public GameObject photoFrame;
    public SpriteRenderer photoCakeRenderer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip chopSound;
    public AudioClip stirSound;
    public AudioClip cakeSound;
    public AudioClip cameraSound;

    // Private State Variables
    private SpriteRenderer sr;
    private GameObject generatedCake; 
    private Sprite currentCakeSprite;

    private bool isMoving = false;
    private bool hasAppeared = false;
    private bool onBoard = false;
    private bool onPot = false;
    private int chopStage = 0;
    private bool isTransforming = false;
    private bool cakeReady = false;
    private bool photoTaken = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;
    }

    // ==========================================
    // INITIALIZATION & RESET
    // ==========================================
    public void SpawnAt(Transform spawnPoint)
    {
        transform.position = spawnPoint.position;
        gameObject.SetActive(true);
        ResetState();
    }

    public void ResetState()
    {
        // Stop any ongoing animations (like stirring) instantly
        StopAllCoroutines(); 

        // Reset all logic flags
        hasAppeared = false;
        onBoard = false;
        onPot = false;
        chopStage = 0;
        isMoving = false;
        isTransforming = false;
        cakeReady = false;
        photoTaken = false;
        if (photoCakeRenderer != null)
        {
            photoCakeRenderer.sprite = null;
        }

        // Reset sprite visuals
        sr.color = Color.white;
        sr.flipX = false; 
        sr.enabled = false;

        // Destroy the leftover cake if there is one
        if (generatedCake != null)
        {
            Destroy(generatedCake);
            generatedCake = null;
        }

        // Clean up the UI
        if (photoFrame != null) photoFrame.SetActive(false);
        if (infoText != null) infoText.SetActive(true); 
        if (introScreen != null) introScreen.SetActive(true);
        
        if (instruction_1 != null) instruction_1.SetActive(false);
        if (instruction_2 != null) instruction_2.SetActive(false);
        if (instruction_3 != null) instruction_3.SetActive(false);
        if (instruction_4 != null) instruction_4.SetActive(false);

        // Snap camera back to the start
        if (cameraSlide != null) cameraSlide.MoveToStep(1);
    }

    // ==========================================
    // UPDATE LOOP (Movement & Keyboards)
    // ==========================================
    void Update()
    {
        // Manual Keyboard Overrides for Testing
        if (Keyboard.current != null)
        {
            if (Keyboard.current.mKey.wasPressedThisFrame) ProcessInput("FLEX: 1000"); 
            else if (Keyboard.current.cKey.wasPressedThisFrame) ProcessInput("ACCEL_Y: 10.0");
            else if (Keyboard.current.sKey.wasPressedThisFrame) ProcessInput("JOY_Y: 0");
            else if (Keyboard.current.pKey.wasPressedThisFrame) ProcessInput("FORCE: 100");
            else if (Keyboard.current.rKey.wasPressedThisFrame) ProcessInput("BUTTON: yes");
        }

        // Smooth Movement Logic
        if (isMoving)
        {
            Vector3 target = onPot ? potPos.position : boardPos.position;
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target) < 0.05f)
            {
                transform.position = target;
                isMoving = false;

                if (!onBoard && !onPot)
                {
                    onBoard = true;
                    if (instruction_2 != null) instruction_2.SetActive(true);
                }
                else if (!onPot)
                {
                    onPot = true;
                    if (instruction_3 != null) instruction_3.SetActive(true);
                }
            }
        }

        // Auto-move to pot after the 2nd chop
        if (chopStage == 2 && onBoard && !onPot)
        {
            isMoving = true;
            audioSource.PlayOneShot(moveSound);
            cameraSlide.MoveToStep(2);
        }
    }

    // ==========================================
    // HARDWARE INPUT PROCESSING
    // ==========================================
    public void ProcessInput(string data)
    {
        Debug.Log("FoodController received: " + data);

        // PRIORITY 1: Master Reset Button
        if (data.StartsWith("BUTTON:"))
        {
            string valStr = data.Replace("BUTTON:", "").Trim();
            if (valStr == "yes")
            {
                ResetState();
                FruitSpawner spawner = FindObjectOfType<FruitSpawner>();
                if (spawner != null) spawner.SpawnNextFruit();
                return; 
            }
        }

        // STAGE 1: Food Appears (NFC Scan)
        if (data.StartsWith("UID:") && !hasAppeared)
        {
            if (introScreen != null) introScreen.SetActive(false);
            if (infoText != null) infoText.SetActive(false);

            sr.enabled = true;
            transform.position = basketPos.position;
            sr.sprite = wholeSprite;

            hasAppeared = true;
            if (instruction_1 != null) instruction_1.SetActive(true);
        }

        // STAGE 2: Move to Board (Flex Sensor)
        else if (data.StartsWith("FLEX:") && hasAppeared && !onBoard)
        {
            string valStr = data.Replace("FLEX:", "").Trim();
            if (int.TryParse(valStr, out int valInt))
            {
                if (instruction_1 != null) instruction_1.SetActive(false);
                audioSource.PlayOneShot(moveSound);
                isMoving = true;
                cameraSlide.MoveToStep(1);
            }
        }

        // STAGE 3: Chop Food (Accelerometer)
        else if (data.StartsWith("ACCEL_Y:") && onBoard && !onPot)
        {
            string valStr = data.Replace("ACCEL_Y:", "").Trim();
            if (float.TryParse(valStr, out float valFloat))
            {
                if (valFloat > 7f) ChopFood();
            }
        }

        // STAGE 4: Stir Pot (Joystick)
        else if (data.StartsWith("JOY_Y:") && onPot && !isTransforming)
        {
            string valStr = data.Replace("JOY_Y:", "").Trim();
            if (int.TryParse(valStr, out int valInt))
            {
                if (valInt == 0 || valInt == 4096)
                {
                    StartCoroutine(TransformToJam());
                }
            }
        }

        // STAGE 5: Take Photo (Force Sensor)
        else if (cakeReady && !photoTaken && data.StartsWith("FORCE:"))
        {
            TakePhoto();
        }
    }

    // ==========================================
    // ACTION METHODS
    // ==========================================
    void ChopFood()
    {
        audioSource.PlayOneShot(chopSound);
        if (instruction_2 != null) instruction_2.SetActive(false);

        chopStage++;

        if (chopStage == 1) sr.sprite = twoPieceSprite;
        else if (chopStage == 2) sr.sprite = fourPieceSprite;
    }

    IEnumerator TransformToJam()
    {
        isTransforming = true;
        audioSource.clip = stirSound;
        audioSource.Play();
        if (instruction_3 != null) instruction_3.SetActive(false);

        sr.sprite = jamSprite;
        sr.flipX = true; // Mirrors the image if needed (remove this line if you don't want it flipped!)

        // Snap exactly to the pot position + the custom offset you set in the Inspector
        transform.position = potPos.position + jamOffset;
        Vector3 basePos = transform.position; 

        float duration = 5f; 
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            // Fade in effect
            float alpha = Mathf.Lerp(0f, 1f, progress);
            sr.color = new Color(1f, 1f, 1f, alpha);
            
            // Shake the jam slightly around its new base position
            transform.position = basePos + (Vector3)Random.insideUnitCircle * 0.05f;

            yield return null;
        }

        // Lock perfectly back to center when done shaking
        transform.position = basePos;
        sr.color = Color.white;
        audioSource.Stop();
        
        yield return new WaitForSeconds(1f); 

        isTransforming = false;
        MakeCake();
    }

    void MakeCake()
    {
        sr.enabled = false;

        // Render the physical cake
        generatedCake = new GameObject("Cake");
        SpriteRenderer cakeSr = generatedCake.AddComponent<SpriteRenderer>();

        cakeSr.sprite = cakeSprite;
        currentCakeSprite = cakeSprite;

        generatedCake.transform.position = platePos.position;
        cakeSr.sortingOrder = 10;

        audioSource.PlayOneShot(cakeSound);
        if (instruction_4 != null) instruction_4.SetActive(true);
        cakeReady = true;

        cameraSlide.MoveToStep(3);
        if (photoCakeRenderer != null) photoCakeRenderer.sprite = currentCakeSprite;
    }

    void TakePhoto()
    {
        photoTaken = true;
        if (instruction_4 != null) instruction_4.SetActive(false);
        audioSource.PlayOneShot(cameraSound);

        if (photoFrame != null) photoFrame.SetActive(true);
    }
}