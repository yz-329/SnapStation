using System.Collections;
using UnityEngine;

public class SeaweedTrigger : MonoBehaviour
{
    private Animator animator;
    private AudioSource seaweedAudio;
    private Coroutine wiggleCoroutine;

    [Header("Settings")]
    [SerializeField] private float wiggleDuration = 2.5f; // Easily adjust between 2-3s in the inspector

    void Start()
    {
        animator = GetComponent<Animator>();
        seaweedAudio = GetComponent<AudioSource>();
        animator.enabled = false;
    }

    // Called from ArduinoManager when flex sensor threshold is breached
    public void Wiggle()
    {
        // If it is already wiggling, we just refresh the timer 
        // This keeps it animating smoothly if the player holds the flex sensor down
        if (wiggleCoroutine != null)
        {
            StopCoroutine(wiggleCoroutine);
        }
        else
        {
            // This runs ONLY on the first frame of the flex trigger, 
            // preventing the audio from stuttering/restarting every 50ms
            if (seaweedAudio != null && !seaweedAudio.isPlaying)
            {
                seaweedAudio.Play();
            }
            
            animator.enabled = true;
        }

        // Start or restart the countdown
        wiggleCoroutine = StartCoroutine(WiggleDurationRoutine());
    }

    private IEnumerator WiggleDurationRoutine()
    {
        yield return new WaitForSeconds(wiggleDuration);
        
        animator.enabled = false;
        wiggleCoroutine = null; 
    }
}