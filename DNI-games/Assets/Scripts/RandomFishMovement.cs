using UnityEngine;

public class RandomFishMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float boostMultiplier = 2.5f; // Multiplies base speed when forced

    private float currentSpeed;
    private float boostTimer = 0f;
    private int direction = -1; // START LEFT (correct)
    private float minX, maxX;
    
    [Header("State")]
    public bool isCaught = false;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
        currentSpeed = speed;

        Camera cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        minX = -width;
        maxX = width;

        ApplyFacing();
    }

    void Update()
    {
        if (isCaught) return;

        // Handle force sensor boost countdown
        if (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
            currentSpeed = speed * boostMultiplier;
        }
        else
        {
            currentSpeed = speed;
        }

        // Move fish using the dynamic current speed
        transform.position += Vector3.right * direction * currentSpeed * Time.deltaTime;

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

    // Called from FishSpawner when the physical Force sensor is hit
    public void ApplyBoost(float duration)
    {
        boostTimer = duration;
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