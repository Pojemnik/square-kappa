using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCoverController : MonoBehaviour
{
    [SerializeField]
    private float fadeTime;

    private UnityEngine.UI.Image image;

    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
    }

    public IEnumerator HideCoverCoroutine()
    {
        Color color = image.color;
        color.a = 1;
        image.color = color;
        float timer = Time.unscaledTime;
        while ((Time.unscaledTime - timer) < fadeTime)
        {
            color.a = Mathf.Cos(((Time.unscaledTime - timer) * Mathf.PI) / (2 * fadeTime));
            image.color = color;
            yield return null;
        }
        color.a = 0;
        image.color = color;
    }

    public IEnumerator ShowCoverCoroutine()
    {
        Color color = image.color;
        color.a = 0;
        image.color = color;
        float timer = Time.unscaledTime;
        while ((Time.unscaledTime - timer) < fadeTime)
        {
            color.a = Mathf.Sin(((Time.unscaledTime - timer) * Mathf.PI) / (2 * fadeTime));
            image.color = color;
            yield return null;
        }
        color.a = 1;
        image.color = color;
    }
}
