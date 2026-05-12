using UnityEngine;

public class HookMovement : MonoBehaviour
{
    [Header("Movement")]
    public float dropSpeed = 2.5f;
    public float reelSpeed = 3.5f;

    [Header("Depth")]
    public float targetDepth = 0f;
    public float topPosition = 4f;

    private bool dropping = false;
    private bool reachedDepth = false;

    void Update()
    {
        HandleDrop();
    }

    void HandleDrop()
    {
        if (!dropping) return;

        transform.Translate(Vector2.down * dropSpeed * Time.deltaTime);

        if (transform.position.y <= targetDepth)
        {
            dropping = false;
            reachedDepth = true;
        }
    }

    // Called when accelerometer detects cast
    public void StartDrop()
    {
        dropping = true;
        reachedDepth = false;
    }

    // Joystick reeling
    public void ProcessJoystick(int joyValue)
    {
        // Debug.Log("JOY: " + joyValue + " | reachedDepth: " + reachedDepth);

        // if (!reachedDepth) return;

        if (joyValue > 3000)
        {
            transform.Translate(Vector2.up * reelSpeed * Time.deltaTime);

            if (transform.position.y >= topPosition)
            {
                gameObject.SetActive(false);
            }
        }
    }
}