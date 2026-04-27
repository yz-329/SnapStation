using UnityEngine;

public class RandomFishMovement : MonoBehaviour
{
    public float speed = 2f;

    private int direction = -1; // START LEFT (correct)
    private float minX, maxX;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        minX = -width;
        maxX = width;

        // IMPORTANT: DO NOT force flip here
        // Your sprite already faces LEFT correctly
        ApplyFacing();
    }

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        // Only change direction at edges
        if (transform.position.x >= maxX)
        {
            direction = -1;
            ApplyFacing();
        }
        else if (transform.position.x <= minX)
        {
            direction = 1;
            ApplyFacing();
        }
    }

    void ApplyFacing()
    {
        float x = Mathf.Abs(originalScale.x);

        // LEFT = default sprite orientation
        if (direction == -1)
        {
            transform.localScale = new Vector3(-x, originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(x, originalScale.y, originalScale.z);
        }
    }
}