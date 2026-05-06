using UnityEngine;

public class FishingController : MonoBehaviour
{
    public GameObject hook; // assign in Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            hook.SetActive(true);
        }
    }
}