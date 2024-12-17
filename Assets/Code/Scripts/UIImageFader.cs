using UnityEngine;
using UnityEngine.UI;

public class UIImageFader : MonoBehaviour
{
    public float opacityDelta = 0.04f;
    private Image image;
    private float speed;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        var tempColor = image.color;
        tempColor.a = Mathf.Clamp(tempColor.a + speed * opacityDelta, 0, 1.0f);
        image.color = tempColor;
    }

    /// <summary>
    ///     Trigger CharacterImage to fade in or out.
    /// </summary>
    /// <param name="dir">True: Fade in, false: Fade out.</param>
    public void TriggerFade(bool dir, bool restart = false)
    {
        if (restart)
        {
            var tempColor = image.color;
            tempColor.a = dir ? 0f : 1.0f;
            image.color = tempColor;
        }

        speed = dir ? 1.0f : -1.0f;
    }

    public void StopFade()
    {
        speed = 0;
    }
}