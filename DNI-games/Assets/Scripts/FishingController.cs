using UnityEngine;

public class FishingController : MonoBehaviour
{
    public GameObject hook; // assign in Inspector
    private AudioSource hookAudio;

    void Start()
    {
        hookAudio = hook.GetComponent<AudioSource>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hook.SetActive(true);
            if (hookAudio != null)
            {
                hookAudio.Play();
            }
        }
    }
}