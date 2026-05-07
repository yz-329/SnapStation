using UnityEngine;

public class HookCatch : MonoBehaviour
{
    private bool hasCaughtFish = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasCaughtFish) return;

        if (other.CompareTag("Fish"))
        {
            hasCaughtFish = true;

            Transform fish = other.transform;

            // Stop fish movement
            var move = fish.GetComponent<RandomFishMovement>();
            if (move != null)
                move.enabled = false;

            // Attach fish to hook
            fish.SetParent(transform);

            // Snap closer to hook
            fish.localPosition = new Vector3(0f, -0.3f, 0f);

            // Tell hook to move upward
            GetComponent<HookMovement>().ReturnUp();
        }
    }

    void OnEnable()
    {
        hasCaughtFish = false;
    }
}