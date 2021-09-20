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
                if (ValueChanged != null)
                {
                    ValueChanged(this, _value);
                }
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
        MouseSensitivity = new SettingField<float>(PlayerPrefs.GetFloat("MouseSens"));
        ZoomMouseSensitivity = new SettingField<float>(PlayerPrefs.GetFloat("ZoomSens"));
        RegisterInstance(this);
    }
}
