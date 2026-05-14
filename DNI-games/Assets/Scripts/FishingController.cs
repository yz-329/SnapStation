using UnityEngine;

public class FishingController : MonoBehaviour
{
    public HookMovement hookMovement;
    public GameObject hook;
    private AudioSource hookAudio;

    [Header("Cast Detection")]
    public float castThreshold = 7f;

    private float lastAccelY = 0f;
    private bool castTriggered = false;

    void Start()
    {
        hookAudio = hook.GetComponent<AudioSource>();
    }

    public void ProcessAccelY(float accelY)
    {
        // Detect sudden upward flick
        float delta = accelY - lastAccelY;

        if (!castTriggered && delta > castThreshold)
        {
            CastHook();
            castTriggered = true;
        }

        // Reset when rod returns to normal
        if (Mathf.Abs(accelY) < 2f)
        {
            castTriggered = false;
        }

        lastAccelY = accelY;
    }

    void CastHook()
    {
        hookMovement.gameObject.SetActive(true);
        hookMovement.StartDrop();
        hookAudio?.Play();
    }
}