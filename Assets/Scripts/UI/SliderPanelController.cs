using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPanelController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TMPro.TextMeshProUGUI valueDisplay;
    [SerializeField]
    private UnityEngine.UI.Slider slider;

    private float value;

    [HideInInspector]
    public event System.EventHandler<float> sliderValueChanged;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocaleChanged += (v) => UpdateValueDisplay(value, v.Formatter);
    }

    public void InitializeSlider(float startValue)
    {
        OnSliderValueChanged(startValue);
        slider.value = startValue;
    }

    private void OnSliderValueChanged(float newValue)
    {
        UpdateValueDisplay(newValue, UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale.Formatter);
        if (sliderValueChanged != null)
        {
            sliderValueChanged(this, newValue);
        }
        value = newValue;
    }

    private void UpdateValueDisplay(float newValue, System.IFormatProvider format)
    {
        valueDisplay.text = ((float)Mathf.Round(newValue * 100) / 100).ToString(format);
    }
}
