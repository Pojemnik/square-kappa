using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDisplayController : MonoBehaviour
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
        if(value > maxValue)
        {
            Debug.LogError("Too high value in display!");
            return;
        }
        if(value < 0)
        {
            value = 0;
        }
        int i = 0;
        while(value > 0)
        {
            int d = value % 10;
            digits[i++].sprite = digitTextures[d];
            value /= 10;
        }
        //display zero if value = 0
        if(i == 0)
        {
            digits[i++].sprite = digitTextures[0];
        }
        //do not display leading zeros
        while (i < digits.Capacity)
        {
            digits[i++].enabled = false;
        }
    }
}
