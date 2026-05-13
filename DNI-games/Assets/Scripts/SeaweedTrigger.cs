using UnityEngine;

public class SeaweedTrigger : MonoBehaviour
{
    private Animator animator;
    private AudioSource seaweedAudio;

    void Start()
    {
        animator = GetComponent<Animator>();
        seaweedAudio = GetComponent<AudioSource>();
        animator.enabled = false;
        
    }

// take controller input and enable animation
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.enabled = true;
            // Play sound once
            if (seaweedAudio != null)
            {
                seaweedAudio.Play();
            }
        }
    }
}