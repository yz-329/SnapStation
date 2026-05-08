using UnityEngine;

public class HookCatch : MonoBehaviour
{
    private bool hasCaughtFish = false;

    private HookMovement hookMovement;

    void Start()
    {
        // Get parent HookMovement
        hookMovement = GetComponentInParent<HookMovement>();
    }

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

            // Attach fish to MAIN hook
            fish.SetParent(transform.parent);

            // Position fish near hook tip
            fish.position = transform.position;

            // Tell hook to move upward
            hookMovement.ReturnUp();
        }
    }

    void OnEnable()
    {
        hasCaughtFish = false;
    }
}