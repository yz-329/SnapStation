using UnityEngine;

public class FoodController : MonoBehaviour
{
    public Transform basketPos;
    public Transform boardPos;

    public float moveSpeed = 5f;

    private bool isMoving = false;

    private bool hasAppeared = false;

    public CameraSlide cameraSlide;

    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Here the controller has input (e.g. read UID), food appears.  
        if (Input.GetKeyDown(KeyCode.Space) && !hasAppeared)
        {
            gameObject.SetActive(true);

            transform.position = basketPos.position;

            hasAppeared = true;
        }

        // A = move food + move camera -- takes input and move food to the chopping area
        if (Input.GetKeyDown(KeyCode.A) && hasAppeared)
        {
            isMoving = true;

            // Move camera to CamPos2
            cameraSlide.MoveToStep(1);
        }

        // Move food
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                boardPos.position,
                moveSpeed * Time.deltaTime
            );

            // Stop when arrived
            if (Vector2.Distance(transform.position, boardPos.position) < 0.05f)
            {
                transform.position = boardPos.position;

                isMoving = false;
            }
        }
    }
}
