using UnityEngine;

public class HookMovement : MonoBehaviour
{
    public float dropSpeed = 2.5f;
    public float reelSpeed = 3.5f;

    public float targetDepth = 0f;
    public float topPosition = 4f;

    private bool dropping = false;
    private bool reeling = false;

    void Update()
    {
        if (dropping)
        {
            HandleDrop();
        }

        if (reeling)
        {
            HandleReel();
        }
    }

    void HandleDrop()
    {
        transform.Translate(Vector2.down * dropSpeed * Time.deltaTime);

        if (transform.position.y <= targetDepth)
        {
            dropping = false;
        }
    }

    void HandleReel()
    {
        transform.Translate(Vector2.up * reelSpeed * Time.deltaTime);

        if (transform.position.y >= topPosition)
        {
            reeling = false;
            gameObject.SetActive(false);
        }
    }

    public void StartDrop()
    {
        dropping = true;
        reeling = false;
    }

    // CALL THIS when fish is caught OR joystick engaged
    public void StartReel()
    {
        reeling = true;
        dropping = false;
    }

    public void ProcessJoystick(int joyValue)
    {
        if (joyValue > 3000)
        {
            StartReel();
        }

        if (joyValue < 3000)
        {
            reeling = false;
        }
    }
}