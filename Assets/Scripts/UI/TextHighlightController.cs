using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHighlightController : MonoBehaviour
{
    public event System.EventHandler animationFinished;
    public delegate float curveShapeFunction(float t);

    private UnityEngine.UI.Image image;
    private bool isPlaying;
    private curveShapeFunction currentCurve;
    private float animationDuration;
    private float animationTime;

    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();    
    }

    private void Update()
    {
        if(isPlaying)
        {
            animationTime += Time.deltaTime;
            Color newColor = image.color;
            newColor.a = currentCurve(animationTime);
            image.color = newColor;
            if(animationTime >= animationDuration)
            {
                newColor.a = 0;
                image.color = newColor;
                isPlaying = false;
                animationFinished?.Invoke(this, null);
            }
        }
    }

    public void PlayAnimation(curveShapeFunction curve, float duration)
    {
        isPlaying = true;
        currentCurve = curve;
        animationDuration = duration;
        animationTime = 0;
    }
}
