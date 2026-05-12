using UnityEngine;
using UnityEngine.UI;

public class OverlayFade : MonoBehaviour
{
    public Image overlayImage;
    public Image specialImage;

    public float fadeSpeed = 1f;

    private bool isDark = false;

    // void Update()
    // {
    //     // Take button input and trigger the overlay effect (I used Space here)
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         isDark = !isDark;
    //     }

    //     // TARGET VALUES
    //     float overlayTarget = isDark ? 0.5f : 0f;
    //     float imageTarget = isDark ? 0.5f : 0f;

    //     // OVERLAY FADE
    //     Color overlayColor = overlayImage.color;
    //     overlayColor.a = Mathf.Lerp(
    //         overlayColor.a,
    //         overlayTarget,
    //         fadeSpeed * Time.deltaTime
    //     );
    //     overlayImage.color = overlayColor;

    //     // SPECIAL IMAGE FADE
    //     Color imageColor = specialImage.color;
    //     imageColor.a = Mathf.Lerp(
    //         imageColor.a,
    //         imageTarget,
    //         fadeSpeed * Time.deltaTime
    //     );
    //     specialImage.color = imageColor;
    // }
}