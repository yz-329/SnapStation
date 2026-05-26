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

    public GameObject instruction_1;
    public GameObject instruction_2;
    public GameObject instruction_3;
    public GameObject instruction_4;

    public AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip chopSound;
    public AudioClip stirSound;
    public AudioClip cakeSound;
    private SpriteRenderer sr;
    private Vector3 originalPotPosition;

    private bool isMoving = false;
    private bool hasAppeared = false;
    private bool onBoard = false;
    private bool onPot = false;

    private int chopStage = 0;
    private bool isTransforming = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPotPosition = potPos.position;
        gameObject.SetActive(false);
    }

    void Update()
    {
        // here takes UID input and food appears in basket
        if (Input.GetKeyDown(KeyCode.Space) && !hasAppeared)
        {
            gameObject.SetActive(true);
            transform.position = basketPos.position;
            sr.sprite = wholeSprite;
            hasAppeared = true;

            instruction_1.SetActive(true);
        }

        // A = takes input and moves food to chopping board
        if (Input.GetKeyDown(KeyCode.A) && hasAppeared && !onBoard)
        {
            instruction_1.SetActive(false);
            audioSource.PlayOneShot(moveSound);
            isMoving = true;
            cameraSlide.MoveToStep(1);
        }
        
        // take the hand movement input and chop food on board
        else if (Input.GetKeyDown(KeyCode.Space) && onBoard && !onPot)
        {
            ChopFood();
        }

        // After chopping, move to pot and camera to CamPos3
        if (chopStage == 2 && onBoard && !onPot)
        {
            isMoving = true;
            audioSource.PlayOneShot(moveSound);
            cameraSlide.MoveToStep(2);
        }

        // Take the joystick input and transform into jam in pot
        if (Input.GetKeyDown(KeyCode.D) && onPot && !isTransforming)
        {
            StartCoroutine(TransformToJam());
        }

        // Movement logic
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

        float duration = 5f; // FULL transformation time
        float t = 0f;

        Vector3 startPos = potPos.position;

        // TRANSFORMATION LOOP (5 seconds total)
        while (t < duration)
        {
            t += Time.deltaTime;

            float progress = t / duration;

            // fade effect (optional: adjust as you like)
            float alpha = Mathf.Lerp(0f, 1f, progress);
            sr.color = new Color(1f, 1f, 1f, alpha);

            // shake pot
            potPos.position = startPos + (Vector3)Random.insideUnitCircle * 0.05f;

            yield return null;
        }

        // reset pot position
        potPos.position = originalPotPosition;

        // switch sprite to jam at end
        sr.sprite = jamSprite;
        sr.color = Color.white;

        // STOP SOUND
        audioSource.Stop();

        isTransforming = false;

        // spawn cake
        MakeCake();
    }
    void MakeCake()
    {
        // hide jam in pot
        sr.enabled = false;

        // create cake object
        GameObject cake = new GameObject("Cake");

        SpriteRenderer cakeSr = cake.AddComponent<SpriteRenderer>();

        cakeSr.sprite = cakeSprite;

        // place cake on plate
        cake.transform.position = platePos.position;

        // optional visual order
        cakeSr.sortingOrder = 10;

        audioSource.PlayOneShot(cakeSound);
        instruction_4.SetActive(true);
    }
}