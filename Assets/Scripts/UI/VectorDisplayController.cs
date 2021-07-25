using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorDisplayController : DisplayController
{
    private TMPro.TextMeshProUGUI text;

    public override void SetValue(int newValue)
    {
        text.text = string.Format("{0}m", newValue);
    }

    public void Awake()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        SetValue(startValue);
    }
}
