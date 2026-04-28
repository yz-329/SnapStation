using UnityEngine;

public class RandomFishMovement2 : MonoBehaviour
{
    public float normalSpeed = 2f;
    public float boostMultiplier = 4f; // How much faster they go when disturbed
    
    private float currentSpeed;
    private float boostTimer = 0f;

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
        // 1. Handle Boost Logic
        if (boostTimer > 0)
        {
            boostTimer -= Time.deltaTime;
            currentSpeed = normalSpeed * boostMultiplier;
        }
        else
        {
            currentSpeed = normalSpeed;
        }

        // 2. Move using the currentSpeed
        transform.position += Vector3.right * direction * currentSpeed * Time.deltaTime;

        // 3. Boundary Checks
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

    // This is the function called by the Serial Manager
    public void ApplyBoost(float duration)
    {
        boostTimer = duration;
    }

    void SetFacing(int dir)
    {
        float xScale = Mathf.Abs(originalScale.x);
        if (dir == 1)
            transform.localScale = new Vector3(-xScale, originalScale.y, originalScale.z);
        else
            transform.localScale = new Vector3(xScale, originalScale.y, originalScale.z);
    }
}