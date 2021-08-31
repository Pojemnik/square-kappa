using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    [Tooltip("Width of bar when health is 0")]
    [SerializeField]
    private float minWidth;
    [Tooltip("Width of bar when health is 100")]
    [SerializeField]
    private float maxWidth;

    private RectTransform rect;

    public void OnPlayerHealthChange(int value)
    {
        if (value < 0)
        {
            //Debug.LogError("Health value less than 0");
            value = 0;
        }
        float hpPercent = value / 100f;
        float width = Mathf.Lerp(minWidth, maxWidth, hpPercent);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
    }

}
