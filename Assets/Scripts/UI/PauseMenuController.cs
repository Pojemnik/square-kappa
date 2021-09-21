using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private SliderPanelController mouseSensitivitySlider;
    [SerializeField]
    private SliderPanelController zoomSensitivitySlider;

    private void Awake()
    {
        mouseSensitivitySlider.sliderValueChanged += (_,v) => SettingsManager.Instance.MouseSensitivity.Value = v;
        zoomSensitivitySlider.sliderValueChanged += (_,v) => SettingsManager.Instance.ZoomMouseSensitivity.Value = v;
        EventManager.Instance.AddListener("Unpause", () => SettingsManager.Instance.SaveSettings());
    }
}
