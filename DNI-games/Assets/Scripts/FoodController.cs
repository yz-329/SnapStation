using System.Collections;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    [Header("Positions")]
    public Transform basketPos;
    public Transform boardPos;
    public Transform potPos;
    public Transform platePos;

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
    private Vector3 originalPotPosition;
    private GameObject generatedCake; 

    private bool isMoving = false;
    private bool hasAppeared = false;
    private bool onBoard = false;
    private bool onPot = false;
    private int chopStage = 0;
    private bool isTransforming = false;
    private Sprite currentCakeSprite;
    private bool cakeReady = false;
    private bool photoTaken = false;

    public void SpawnAt(Transform spawnPoint)
    {
        transform.position = spawnPoint.position;
        gameObject.SetActive(true);
        ResetState();
    }

    public void ResetState()
    {
        // CRITICAL: Stop the jam transformation if the player skips early!
        StopAllCoroutines(); 

        hasAppeared = false;
        onBoard = false;
        onPot = false;
        chopStage = 0;
        isMoving = false;
        isTransforming = false;
        cakeReady = false;
        photoTaken = false;

        sr.color = Color.white;
        sr.enabled = true;

        if (generatedCake != null)
        {
            Destroy(generatedCake);
        }

        if (photoFrame != null) photoFrame.SetActive(false);
        if (infoText != null) infoText.SetActive(true); 
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPotPosition = potPos.position;
        sr.enabled = false;
    }

    void Update()
    {
        if (isMoving)
        {
            Vector3 target = onPot ? potPos.position : boardPos.position;

            transform.position = Vector2.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, target) < 0.05f)
            {
                transform.position = target;
                isMoving = false;

                if (!onBoard && !onPot)
                {
                    onBoard = true;
                    instruction_2.SetActive(true);
                }
                else if (!onPot)
                {
                    onPot = true;
                    instruction_3.SetActive(true);
                }
            }
        }

        if (chopStage == 2 && onBoard && !onPot)
        {
            isMoving = true;
            audioSource.PlayOneShot(moveSound);
            cameraSlide.MoveToStep(2);
        }
    }

    public void ProcessInput(string data)
    {
        Debug.Log(data);

        // PRIORITY 1: Master Button Override
        // This will instantly reset the game and move to the next fruit from ANY stage.
        if (data.StartsWith("BUTTON:"))
        {
            string valStr = data.Replace("BUTTON:", "").Trim();
            if (valStr == "yes")
            {
                // Reset the camera back to the basket
                if (cameraSlide != null)
                {
                    cameraSlide.MoveToStep(1);
                    instruction_2.SetActive(false);
                    instruction_3.SetActive(false);
                    instruction_4.SetActive(false);
                }

                // Tell the spawner to jump to the next fruit
                FindObjectOfType<FruitSpawner>().SpawnNextFruit();
                
                // return stops Unity from reading the rest of this function for this frame
                return; 
            }
        }

        // FOOD APPEAR 
        if (data.StartsWith("UID:") && !hasAppeared)
        {
            if (introScreen != null) introScreen.SetActive(false);
            if (infoText != null) infoText.SetActive(false);

            sr.enabled = true;
            transform.position = basketPos.position;
            sr.sprite = wholeSprite;

            hasAppeared = true;
            instruction_1.SetActive(true);
        }

        // MOVE TO BOARD
        else if (data.StartsWith("FLEX:") && hasAppeared && !onBoard)
        {
            string valStr = data.Replace("FLEX:", "").Trim();
            
            if (int.TryParse(valStr, out int valInt))
            {
                if (valInt < 2000)
                {
                    instruction_1.SetActive(false);
                    audioSource.PlayOneShot(moveSound);
                    isMoving = true;
                    cameraSlide.MoveToStep(1);
                }
            }
        }

        // CHOP
        else if (data.StartsWith("ACCEL_Y:") && onBoard && !onPot)
        {
            string valStr = data.Replace("ACCEL_Y:", "").Trim();
    
            if (float.TryParse(valStr, out float valFloat))
            {
                if (valFloat > 7f)
                {
                    ChopFood();
                }
            }
        }

        // STIR
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

        // TAKE PHOTO
        else if (cakeReady && !photoTaken && data.StartsWith("FORCE:"))
        {
            TakePhoto();
        }
    }

    void ChopFood()
    {
        audioSource.PlayOneShot(chopSound);
        instruction_2.SetActive(false);

        chopStage++;

        if (chopStage == 1)
        {
            sr.sprite = twoPieceSprite;
        }
        else if (chopStage == 2)
        {
            sr.sprite = fourPieceSprite;
        }
    }

    IEnumerator TransformToJam()
    {
        isTransforming = true;
        audioSource.clip = stirSound;
        audioSource.Play();
        instruction_3.SetActive(false);

        sr.sprite = jamSprite;

        float duration = 5f; 
        float t = 0f;
        Vector3 startPos = potPos.position;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            float alpha = Mathf.Lerp(0f, 1f, progress);
            sr.color = new Color(1f, 1f, 1f, alpha);
            potPos.position = startPos + (Vector3)Random.insideUnitCircle * 0.05f;

            yield return null;
        }

        potPos.position = originalPotPosition;
        sr.color = Color.white;
        audioSource.Stop();
        
        yield return new WaitForSeconds(1f); 

        isTransforming = false;

        // Calls the cake rendering function immediately at the end of stirring
        MakeCake();
    }

    void MakeCake()
    {
        sr.enabled = false;

        // This block renders the physical cake on the plate!
        generatedCake = new GameObject("Cake");
        SpriteRenderer cakeSr = generatedCake.AddComponent<SpriteRenderer>();

        cakeSr.sprite = cakeSprite;
        currentCakeSprite = cakeSprite;

        generatedCake.transform.position = platePos.position;
        cakeSr.sortingOrder = 10;

        audioSource.PlayOneShot(cakeSound);
        instruction_4.SetActive(true);
        cakeReady = true;

        cameraSlide.MoveToStep(3);
        photoCakeRenderer.sprite = currentCakeSprite;
    }

    void TakePhoto()
    {
        photoTaken = true;
        instruction_4.SetActive(false);
        audioSource.PlayOneShot(cameraSound);

        photoFrame.SetActive(true);
        
        // Note: We removed the auto-loop timer here. The game will now 
        // wait indefinitely until the player presses the BUTTON:yes !
    }
}