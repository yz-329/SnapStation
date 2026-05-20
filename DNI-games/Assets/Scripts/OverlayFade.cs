using UnityEngine;
using UnityEngine.UI;

public class OverlayFade : MonoBehaviour
{
    public Image overlayImage;
    public Image specialImage;

    public float fadeSpeed = 2f;

    private bool isDark = false;

    private float overlayTarget;
    private float imageTarget;

    public void ToggleSwitch()
    {
        isDark = !isDark;

        overlayTarget = isDark ? 0.5f : 0f;
        imageTarget = isDark ? 0.5f : 0f;

        Debug.Log("Toggle pressed");
    }

    void Update()
    {
        // Fade overlay
        Color overlayColor = overlayImage.color;
        overlayColor.a = Mathf.Lerp(
            overlayColor.a,
            overlayTarget,
            fadeSpeed * Time.deltaTime
        );
        overlayImage.color = overlayColor;

        // Button state replacement image
        Color imageColor = specialImage.color;
        imageColor.a = imageTarget;
        specialImage.color = imageColor;
    }
}