using UnityEngine;

public class HookCatch : MonoBehaviour
{
    private bool hasCaughtFish = false;

    private HookMovement hookMovement;

    void Start()
    {
        hookMovement = GetComponentInParent<HookMovement>();
    }

    void OnEnable()
    {
        hasCaughtFish = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasCaughtFish)
            return;

        if (other.CompareTag("Fish"))
        {
            hasCaughtFish = true;

            Transform fish = other.transform;

            // Stop fish movement
            RandomFishMovement move =
                fish.GetComponent<RandomFishMovement>();

            if (move != null)
            {
                move.enabled = false;
            }

            // Attach fish to hook
            fish.SetParent(transform.parent);

            // Snap mouth to hook tip
            Transform mouth = fish.Find("MouthPoint");

            if (mouth != null)
            {
                Vector3 offset =
                    fish.position - mouth.position;

                fish.position =
                    transform.position + offset;
            }

            Debug.Log("Fish Hooked!");
        }
    }
}