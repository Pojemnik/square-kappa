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

    [HideInInspector]
    public event System.EventHandler<float> sliderValueChanged;

    private void Awake()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        valueDisplay.text = slider.value.ToString();
    }

    private void OnSliderValueChanged(float newValue)
    {
        valueDisplay.text = newValue.ToString();
    }
}
