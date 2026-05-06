using UnityEngine;

public class HookMovement : MonoBehaviour
{
    public float speed = 3f;
    private bool movingDown = true;

    void OnEnable()
    {
        movingDown = true;
    }

    void Update()
    {
        if (movingDown)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);

            // stop at certain depth
            if (transform.position.y < -3f)
            {
                movingDown = false;
            }
        }
        else
        {
            // move back up
            transform.Translate(Vector2.up * speed * Time.deltaTime);

            if (transform.position.y > 4f)
            {
                gameObject.SetActive(false); // reset
            }
        }
    }
}