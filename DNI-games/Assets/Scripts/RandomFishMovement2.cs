using UnityEngine;

public class RandomFishMovement2 : MonoBehaviour
{
    [Header("Movement Settings")]
    public float normalSpeed = 2f;
    public float boostMultiplier = 4f;

    private float currentSpeed;
    private float boostTimer = 0f;

    private int direction = 1;

    private float minX;
    private float maxX;

    [Header("State")]
    public bool isCaught = false;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;

        currentSpeed = normalSpeed;

        SetFacing(direction);

        Camera cam = Camera.main;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        minX = -width;
        maxX = width;
    }

    void Update()
    {
        if (isCaught)
            return;

        // Handle boost timing
        if (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
            currentSpeed = normalSpeed * boostMultiplier;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        // Move fish
        transform.position += Vector3.right * direction * currentSpeed * Time.deltaTime;

        // Boundary checks
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

    // Called externally to temporarily speed up the fish
    public void ApplyBoost(float duration)
    {
        boostTimer = duration;
    }

    void SetFacing(int dir)
    {
        float xScale = Mathf.Abs(originalScale.x);

        if (dir == 1)
        {
            transform.localScale = new Vector3(
                -xScale,
                originalScale.y,
                originalScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                xScale,
                originalScale.y,
                originalScale.z
            );
        }
    }
}