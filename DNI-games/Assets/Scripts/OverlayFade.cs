using UnityEngine;
using UnityEngine.UI;

public class OverlayFade : MonoBehaviour
{
    public Image overlayImage;

    public float fadeSpeed = 2f;

    private bool isDark = false;

    void Update()
    {
        // Take button input to control the overaly background (I used space here)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isDark = !isDark;
        }

        Color color = overlayImage.color;

        // Choose target alpha
        float targetAlpha = isDark ? 0.5f : 0f;

        // Smooth fade
        color.a = Mathf.Lerp(
            color.a,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        overlayImage.color = color;
    }
}