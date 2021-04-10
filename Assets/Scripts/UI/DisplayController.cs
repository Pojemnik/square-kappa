using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : MonoBehaviour
{
    public List<Sprite> digitTextures;
    public int defaultValue;
    public List<UnityEngine.UI.Image> digits;

    private int maxValue;

    private void Awake()
    {
        if(digitTextures.Capacity != 10)
        {
            Debug.LogError("Incorrect digits in display!");
        }
        maxValue = (int)Mathf.Pow(10, digits.Capacity);
        SetValue(defaultValue);
    }

    public void SetValue(int value)
    {
        if(value > maxValue || value < 0)
        {
            Debug.LogError("Incorrect value in display!");
            return;
        }
        int i = 0;
        while(value > 0)
        {
            int d = value % 10;
            digits[i++].sprite = digitTextures[d];
            value /= 10;
        }
    }
}
