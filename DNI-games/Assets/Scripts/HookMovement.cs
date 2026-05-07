using UnityEngine;

public class HookMovement : MonoBehaviour
{
    public float speed = 2.5f;
    public float targetDepth = 0f; //how deep the hook gets

    private bool dropping = false;
    private bool returning = false;

    void OnEnable()
    {
        dropping = true;
        returning = false;
    }

    void Update()
    {
        // DROPPING
        if (dropping)
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);

            // Reached fishing depth
            if (transform.position.y <= targetDepth)
            {
                dropping = false;
            }
        }

        // RETURNING
        if (returning)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);

            // Back to top
            if (transform.position.y >= 4f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Called when fish is caught
    public void ReturnUp()
    {
        returning = true;
    }
}