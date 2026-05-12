using UnityEngine;

public class SeaweedTrigger : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        animator.enabled = false;
    }

// take controller input and enable animation
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         animator.enabled = true;
    //     }
    // }
}