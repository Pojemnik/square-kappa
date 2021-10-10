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

    void Awake()
    {
        MouseSensitivity = new SettingField<float>(PlayerPrefs.GetFloat("MouseSens", 0.3f));
        ZoomMouseSensitivity = new SettingField<float>(PlayerPrefs.GetFloat("ZoomSens", 0.15f));
        RegisterInstance(this);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MouseSens", MouseSensitivity.Value);
        PlayerPrefs.SetFloat("ZoomSens", ZoomMouseSensitivity.Value);
    }

    public void SetLocale(UnityEngine.Localization.LocaleIdentifier id)
    {
        if(UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale != UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.GetLocale(id))
        {
            UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale = UnityEngine.Localization.Settings.LocalizationSettings.AvailableLocales.GetLocale(id);
        }
    }
}
