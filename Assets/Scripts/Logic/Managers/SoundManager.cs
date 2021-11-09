using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField]
    private UnityEngine.Audio.AudioMixer audioMixer;

    private void Awake()
    {
        RegisterInstance(this);
    }

    private void Start()
    {
        ChangeEffectVolume(SettingsManager.Instance.SoundVolume.Value);
        SettingsManager.Instance.SoundVolume.ValueChanged += (_, v) => ChangeEffectVolume(v);
    }

    private float MapFloatToVolume(float value)
    {
        return Mathf.Lerp(-80, 20, value);
    }

    public void ChangeEffectVolume(float value)
    {
        audioMixer.SetFloat("EffectsVolume", MapFloatToVolume(value));
    }    
}
