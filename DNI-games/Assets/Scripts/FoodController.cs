using System.Collections;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    public Transform basketPos;
    public Transform boardPos;
    public Transform potPos;

    public float moveSpeed = 5f;

    public CameraSlide cameraSlide;

    public Sprite wholeSprite;
    public Sprite twoPieceSprite;
    public Sprite fourPieceSprite;
    public Sprite jamSprite;
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
        // SPACE = spawn
        if (Input.GetKeyDown(KeyCode.Space) && !hasAppeared)
        {
            gameObject.SetActive(true);
            transform.position = basketPos.position;
            sr.sprite = wholeSprite;
            hasAppeared = true;
        }

        // A = move to board + camera to CamPos2
        if (Input.GetKeyDown(KeyCode.A) && hasAppeared && !onBoard)
        {
            isMoving = true;
            cameraSlide.MoveToStep(1);
        }
        
        // take the input and chop food on board
        else if (Input.GetKeyDown(KeyCode.Space) && onBoard && !onPot)
        {
            ChopFood();
        }

        // W = move to pot + camera to CamPos3
        if (Input.GetKeyDown(KeyCode.W) && chopStage == 2 && onBoard && !onPot)
        {
            isMoving = true;
            cameraSlide.MoveToStep(2);
        }

        // D = transform into jam in pot
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
    }
}