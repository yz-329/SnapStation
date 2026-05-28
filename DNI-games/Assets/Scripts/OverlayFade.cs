using UnityEngine;
using UnityEngine.UI;

public class OverlayFade : MonoBehaviour
{
    public Image overlayImage;
    public Image specialImage;
    public AudioSource clickAudio;

    public float fadeSpeed = 1f;

    private bool isDark = false;

    // Called externally from ArduinoManager
    public void ToggleOverlay()
    {
        isDark = !isDark;

        // Play click once
        clickAudio.PlayOneShot(clickAudio.clip);
    }

    void Update()
    {
        // TARGET VALUES
        float overlayTarget = isDark ? 0.5f : 0f;
        float imageTarget = isDark ? 0.5f : 0f;

        // OVERLAY FADE
        Color overlayColor = overlayImage.color;
        overlayColor.a = Mathf.Lerp(
            overlayColor.a,
            overlayTarget,
            fadeSpeed * Time.deltaTime
        );
        overlayImage.color = overlayColor;

        // SPECIAL IMAGE FADE
        Color imageColor = specialImage.color;
        imageColor.a = Mathf.Lerp(
            imageColor.a,
            imageTarget,
            fadeSpeed * Time.deltaTime
        );
        specialImage.color = imageColor;
    }
}