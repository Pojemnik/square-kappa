using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    public class SettingField<T>
    {
        private T _value;
        public event System.EventHandler<T> ValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, _value);
            }
        }

        public SettingField(T initalValue)
        {
            _value = initalValue;
        }
    }

    public SettingField<float> MouseSensitivity { get; private set; }
    public SettingField<float> ZoomMouseSensitivity { get; private set; }
    public SettingField<float> SoundVolume { get; private set; }

    [SerializeField]
    public float defaultMouseSensitivity;
    [SerializeField]
    public float defaultZoomSensitivity;
    [SerializeField]
    public float defaultSoundVolume;

    private void Awake()
    {
        MouseSensitivity = new SettingField<float>(PlayerPrefs.GetFloat("MouseSens", defaultMouseSensitivity));
        ZoomMouseSensitivity = new SettingField<float>(PlayerPrefs.GetFloat("ZoomSens", defaultZoomSensitivity));
        SoundVolume = new SettingField<float>(PlayerPrefs.GetFloat("SoundVolume", defaultSoundVolume));
        RegisterInstance(this);
    }

    private void OnDestroy()
    {
        SaveSettings();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSens", MouseSensitivity.Value);
        PlayerPrefs.SetFloat("ZoomSens", ZoomMouseSensitivity.Value);
        PlayerPrefs.SetFloat("SoundVolume", SoundVolume.Value);
    }

    public void SetLocale(UnityEngine.Localization.LocaleIdentifier id)
    {
        if(UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale != UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.GetLocale(id))
        {
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.GetLocale(id);
        }
    }
}
