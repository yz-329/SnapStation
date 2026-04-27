using UnityEngine;

public class RandomFishMovement2 : MonoBehaviour
{
    public float speed = 2f;

    private int direction = 1; 
    private float minX, maxX;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        SetFacing(direction);

        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        minX = -width;
        maxX = width;
    }

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if (transform.position.x > maxX)
        {
            direction = -1;
            SetFacing(direction);
        }
        else if (transform.position.x < minX)
        {
            direction = 1;
            SetFacing(direction);
        }
    }

    void SetFacing(int dir)
    {
        // Since your fish faces LEFT by default:
        // LEFT = normal scale
        // RIGHT = flipped X

        float xScale = Mathf.Abs(originalScale.x);

        if (dir == 1)
        {
            transform.localScale = new Vector3(-xScale, originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(xScale, originalScale.y, originalScale.z);
        }
    }
}