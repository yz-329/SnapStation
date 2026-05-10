using UnityEngine;
using UnityEngine.UI;

public class OverlayFade : MonoBehaviour
{
    public Image overlayImage;

    public float fadeSpeed = 2f;

    private bool fading = false;

    void Update()
    {
        // Take button input to trigger fade
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     fading = true;
        // }

        // Smooth fade
        if (fading)
        {
            Color color = overlayImage.color;

            color.a = Mathf.Lerp(color.a, 0.5f, fadeSpeed * Time.deltaTime);

            overlayImage.color = color;
        }
    }
}