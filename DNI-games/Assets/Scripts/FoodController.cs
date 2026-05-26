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

    public AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip chopSound;
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
        }

        // A = takes input and moves food to chopping board
        if (Input.GetKeyDown(KeyCode.A) && hasAppeared && !onBoard)
        {
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
            audioSource.PlayOneShot(moveSound);
            isMoving = true;
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
                }
                else if (!onPot)
                {
                    onPot = true; 
                }
            }
        }

    }

    void ChopFood()
    {
        audioSource.PlayOneShot(chopSound);

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

        float duration = 1f;
        float t = 0f;

        Vector3 startPos = potPos.position;

        // SHAKE + FADE OUT
        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = 1f - (t / duration);
            sr.color = new Color(1f, 1f, 1f, alpha);

            // pot shake effect (small random offset)
            potPos.position = startPos + (Vector3)Random.insideUnitCircle * 0.05f;

            yield return null;
        }

        // reset pot position after shake
        potPos.position = originalPotPosition;

        // switch sprite to jam
        sr.sprite = jamSprite;

        // FADE IN
        t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float alpha = t / duration;
            sr.color = new Color(1f, 1f, 1f, alpha);

            yield return null;
        }

        sr.color = Color.white;

        isTransforming = false;
        yield return new WaitForSeconds(5f);

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
    }
}