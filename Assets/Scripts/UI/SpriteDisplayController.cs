using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDisplayController : DisplayController
{
    [SerializeField]
    private List<Sprite> digitTextures;
    [SerializeField]
    private int defaultValue;
    [SerializeField]
    private List<UnityEngine.UI.Image> digits;
    [SerializeField]
    private bool displayLeadingZeros;

    private int maxValue;
    private int[] digitValues;

    public void Awake()
    {
        if (digitTextures.Count != 10)
        {
            Debug.LogError("Incorrect digits in display!");
        }
        maxValue = (int)Mathf.Pow(10, digits.Count);
        digitValues = new int[digits.Count];
        SetValue(defaultValue);
    }

    public override void SetValue(int value)
    {
        if (value > maxValue)
        {
            Debug.LogError("Too high value in display!");
            return;
        }
        if (value < 0)
        {
            value = 0;
        }
        for (int i = 0; i < digits.Count; i++)
        {
            digitValues[i] = value % 10;
            digits[i].enabled = true;
            digits[i].sprite = digitTextures[digitValues[i]];
            value /= 10;
        }
        if (!displayLeadingZeros)
        {
            int i = digits.Count - 1;
            while (i > 0)
            {
                if (digitValues[i] == 0)
                {
                    digits[i].enabled = false;
                    i--;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
