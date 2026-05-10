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
            Transform mouth = fish.Find("MouthPoint");
            if (mouth != null)
            {
                Vector3 offset = fish.position - mouth.position;
                fish.position = transform.position + offset;
            }
            // Tell hook to move upward
            hookMovement.ReturnUp();
        }
    }

    void OnEnable()
    {
        hasCaughtFish = false;
    }
}