using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoImageController : MonoBehaviour
{
    private UnityEngine.UI.Image image;

    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        image.enabled = false;
    }

    public void DisplayImage(Sprite sprite)
    {
        image.enabled = true;
        image.sprite = sprite;
    }

    private IEnumerator HideImageCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideImage();
    }

    public void HideImage()
    {
        image.enabled = false;
    }
}
