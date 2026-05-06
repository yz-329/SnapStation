using UnityEngine;

public class HookCatch : MonoBehaviour
{
    private Transform caughtFish;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fish"))
        {
            // caughtFish = other.transform;
            Transform fish = other.transform;
            fish.SetParent(transform);
            // Attach fish to hook
            // caughtFish.SetParent(transform);
            fish.localPosition = new Vector3(-2.0f, -2.5f, 0f);
            // other.GetComponent<RandomFishMovement>().enabled = false;
        }
    }
}
