using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField]
    private SliderPanelController mouseSensitivitySlider;
    [SerializeField]
    private SliderPanelController zoomSensitivitySlider;
    [SerializeField]
    private DropdownPanelController languageDropdown;

    private List<UnityEngine.Localization.Locale> locales;
    private UnityEngine.Localization.LocaleIdentifier selectedLocale;

    private void Start()
    {
        mouseSensitivitySlider.InitializeSlider(SettingsManager.Instance.MouseSensitivity.Value);
        mouseSensitivitySlider.sliderValueChanged += (_, v) => SettingsManager.Instance.MouseSensitivity.Value = v;
        zoomSensitivitySlider.InitializeSlider(SettingsManager.Instance.ZoomMouseSensitivity.Value);
        zoomSensitivitySlider.sliderValueChanged += (_, v) => SettingsManager.Instance.ZoomMouseSensitivity.Value = v;
        locales = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales.ToList();
        languageDropdown.SetDropdownContent(locales.Select((e) => e.LocaleName).ToList());
        int selectedIndex = locales.FindIndex((l) => l == UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale);
        languageDropdown.SetSelectedOption(selectedIndex);
        languageDropdown.DropdownFieldSelected += (_, index) => selectedLocale = locales[index].Identifier;
        EventManager.Instance.AddListener("Unpause", OnMenuClose);
    }

    private void OnMenuClose()
    {
        SettingsManager.Instance.SetLocale(selectedLocale);
        SettingsManager.Instance.SaveSettings();
    }
}
