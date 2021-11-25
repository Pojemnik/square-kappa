using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OptionsMenuController : MonoBehaviour
{
    [SerializeField]
    private SliderPanelController mouseSensitivitySlider;
    [SerializeField]
    private SliderPanelController zoomSensitivitySlider;
    [SerializeField]
    private SliderPanelController soundVolumeSlider;
    [SerializeField]
    private DropdownPanelController languageDropdown;

    private List<UnityEngine.Localization.Locale> locales;
    private UnityEngine.Localization.LocaleIdentifier selectedLocale;

    private void Start()
    {
        InitSliders();
        locales = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.Locales.ToList();
        languageDropdown.SetDropdownContent(locales.Select((e) => e.LocaleName).ToList());
        int selectedIndex = locales.FindIndex((l) => l == UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale);
        languageDropdown.SetSelectedOption(selectedIndex);
        languageDropdown.DropdownFieldSelected += (_, index) => selectedLocale = locales[index].Identifier;
        EventManager.Instance?.AddListener("Unpause", OnMenuClose);
    }

    private void OnDisable()
    {
        OnMenuClose();
    }

    private void InitSliders()
    {
        mouseSensitivitySlider.InitializeSlider(SettingsManager.Instance.MouseSensitivity.Value);
        mouseSensitivitySlider.sliderValueChanged += (_, v) => SettingsManager.Instance.MouseSensitivity.Value = v;
        zoomSensitivitySlider.InitializeSlider(SettingsManager.Instance.ZoomMouseSensitivity.Value);
        zoomSensitivitySlider.sliderValueChanged += (_, v) => SettingsManager.Instance.ZoomMouseSensitivity.Value = v;
        soundVolumeSlider.InitializeSlider(SettingsManager.Instance.SoundVolume.Value);
        soundVolumeSlider.sliderValueChanged += (_, v) => SettingsManager.Instance.SoundVolume.Value = v;
    }

    private void OnMenuClose()
    {
        SettingsManager.Instance?.SetLocale(selectedLocale);
        SettingsManager.Instance?.SaveSettings();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
