using UnityEngine;

public class IntroSwitcher : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;

    private SpriteRenderer sr;
    private bool showingFirst = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        InvokeRepeating(nameof(SwitchSprite), 0.5f, 0.5f);
    }

    void SwitchSprite()
    {
        showingFirst = !showingFirst;
        sr.sprite = showingFirst ? sprite1 : sprite2;
    }
}