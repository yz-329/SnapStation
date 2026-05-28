using System.Collections;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public Transform basketPos;
    public Transform boardPos;
    public Transform potPos;
    public Transform platePos;

    public float moveSpeed = 5f;

    public CameraSlide cameraSlide;

    public Sprite wholeSprite;
    public Sprite twoPieceSprite;
    public Sprite fourPieceSprite;
    public Sprite jamSprite;
    public Sprite cakeSprite;

    public GameObject introScreen;
    public GameObject instruction_1;
    public GameObject instruction_2;
    public GameObject instruction_3;
    public GameObject instruction_4;

    public GameObject photoFrame;
    public SpriteRenderer photoCakeRenderer;

    public AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip chopSound;
    public AudioClip stirSound;
    public AudioClip cakeSound;
    public AudioClip cameraSound;
    private SpriteRenderer sr;

    // private bool gameStarted = false;
    private Vector3 originalPotPosition;

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
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPotPosition = potPos.position;
        sr.enabled = false;
    }

    // void Update()
    // {
    //     if (serialPort == null || !serialPort.IsOpen)
    //         return;

    //     try
    //     {
    //         string data = serialPort.ReadLine().Trim();

    //         Debug.Log(data);
    //         // here takes UID input and food appears in basket
    //         // if (Input.GetKeyDown(KeyCode.Space) && !hasAppeared)
    //         if (data.StartsWith("UID:") && !hasAppeared)
    //         {   
    //             introScreen.SetActive(false);
    //             gameObject.SetActive(true);
    //             transform.position = basketPos.position;
    //             sr.sprite = wholeSprite;
    //             hasAppeared = true;

    //             instruction_1.SetActive(true);
    //         }

    //         // A = takes input and moves food to chopping board
    //         if (data.StartsWith("FLEX:") && hasAppeared && !onBoard)
    //         {
    //             instruction_1.SetActive(false);
    //             audioSource.PlayOneShot(moveSound);
    //             isMoving = true;
    //             cameraSlide.MoveToStep(1);
    //         }
            
    //         // take the hand movement input and chop food on board
    //         else if (data.StartsWith("ACCEL_Y:") && onBoard && !onPot)
    //         {
    //             ChopFood();
    //         }

    //         // After chopping, move to pot and camera to CamPos3
    //         if (chopStage == 2 && onBoard && !onPot)
    //         {
    //             isMoving = true;
    //             audioSource.PlayOneShot(moveSound);
    //             cameraSlide.MoveToStep(2);
    //         }

    //         // Take the joystick input and transform into jam in pot
    //         // STARR process joystick input
    //         if (data.StartsWith("JOY_Y:") && onPot && !isTransforming)
    //         {
    //             StartCoroutine(TransformToJam());
    //         }

    //         // Movement logic
    //         if (isMoving)
    //         {
    //             Vector3 target = onPot ? potPos.position : boardPos.position;

    //             transform.position = Vector2.MoveTowards(
    //                 transform.position,
    //                 target,
    //                 moveSpeed * Time.deltaTime
    //             );

    //             if (Vector2.Distance(transform.position, target) < 0.05f)
    //             {
    //                 transform.position = target;
    //                 isMoving = false;

    //                 if (!onBoard && !onPot)
    //                 {
    //                     onBoard = true;
    //                     instruction_2.SetActive(true);
    //                 }
    //                 else if (!onPot)
    //                 {
    //                     onPot = true; 
    //                     instruction_3.SetActive(true);
    //                 }
    //             }
    //         }

    //         // Take input and take photo
    //         if (cakeReady && !photoTaken && data.StartsWith("FORCE:"))
    //         {
    //             TakePhoto();
    //         }


    //     }
    //     catch (System.TimeoutException)
    //     {
    //         // Normal serial timeout
    //     }
    //     catch (System.Exception e)
    //     {
    //         Debug.LogWarning(e.Message);
    //     }


    // }

    void Update()
    {
        // Movement logic only

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

        // auto move to pot after chopping
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

        // FOOD APPEAR
        if (data.StartsWith("UID:") && !hasAppeared)
        {
            introScreen.SetActive(false);

            sr.enabled = true;

            transform.position = basketPos.position;
            sr.sprite = wholeSprite;

            hasAppeared = true;

            instruction_1.SetActive(true);
        }

        // MOVE TO BOARD
        if (data.StartsWith("FLEX:") && hasAppeared && !onBoard)
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
        if (data.StartsWith("JOY_Y:") && onPot && !isTransforming)
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
        if (cakeReady && !photoTaken && data.StartsWith("FORCE:"))
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

        // START SOUND
        audioSource.clip = stirSound;
        audioSource.Play();
        instruction_3.SetActive(false);

        // FIX: Change to the jam sprite at the START of the stirring so it can fade in
        sr.sprite = jamSprite;

        float duration = 5f; // FULL transformation time
        float t = 0f;

        Vector3 startPos = potPos.position;

        // TRANSFORMATION LOOP (5 seconds total)
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            // fade effect (starts invisible at 0, fades to fully visible at 1)
            float alpha = Mathf.Lerp(0f, 1f, progress);
            sr.color = new Color(1f, 1f, 1f, alpha);

            // shake pot
            potPos.position = startPos + (Vector3)Random.insideUnitCircle * 0.05f;

            yield return null;
        }

        // reset pot position
        potPos.position = originalPotPosition;

        // Ensure the jam is fully opaque at the end
        sr.color = Color.white;

        // STOP SOUND
        audioSource.Stop();
        
        // Optional: Wait 1 extra second here if you want players to look at the jam 
        // before it instantly turns into result.
        yield return new WaitForSeconds(1f); 

        isTransforming = false;

        // spawn cake
        MakeCake();
    }

    void MakeCake()
    {
        // 1. Hide jam in pot
        sr.enabled = false;

        // 2. Create the physical cake on the plate
        GameObject cake = new GameObject("Cake");
        SpriteRenderer cakeSr = cake.AddComponent<SpriteRenderer>();

        cakeSr.sprite = cakeSprite;
        currentCakeSprite = cakeSprite;
        cake.transform.position = platePos.position;
        cakeSr.sortingOrder = 10;

        // 3. Play sound and pan the camera to look at the cake
        audioSource.PlayOneShot(cakeSound);
        cameraSlide.MoveToStep(3); 

        photoCakeRenderer.sprite = currentCakeSprite;

        // 4. Show the instruction (e.g., "Press the force sensor to take a photo!")
        instruction_4.SetActive(true);
        
        // 5. Tell the script that we are now waiting for the FORCE sensor input
        cakeReady = true; 
    }

    void TakePhoto()
    {
        photoTaken = true;

        // 1. Hide the instruction text
        instruction_4.SetActive(false); 

        // 2. Play the camera shutter sound effect
        audioSource.PlayOneShot(cameraSound);

        // 3. Update the polaroid/photo frame UI with the correct cake sprite
        photoCakeRenderer.sprite = currentCakeSprite;

        // 4. Finally, show the photo on screen!
        photoFrame.SetActive(true);
    }
}