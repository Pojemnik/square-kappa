using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorDisplayController : MonoBehaviour
{
    [SerializeField]
    private int startValue;

    private TMPro.TextMeshProUGUI text;

    public void UpdateValue(int newValue)
    {
        text.text = string.Format("{0}m", newValue);
    }

    private void Awake()
    {
        text = GetComponent<TMPro.TextMeshProUGUI>();
        UpdateValue(startValue);
    }
}
