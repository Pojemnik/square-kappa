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
        EventManager.Instance.AddListener("Pause", () => PauseSounds());
        EventManager.Instance.AddListener("StopTime", () => PauseSounds());
        EventManager.Instance.AddListener("Victory", () => PauseSounds());
        EventManager.Instance.AddListener("PlayerDeath", () => PauseSounds());
        EventManager.Instance.AddListener("Unpause", () => UnpauseSounds());
        EventManager.Instance.AddListener("ResetTimescale", () => UnpauseSounds());
        EventManager.Instance.AddListener("GameReloaded", () => UnpauseSounds());
    }

    private void PauseSounds()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach(AudioSource source in audioSources)
        {
            source.Pause();
        }
    }

    private void UnpauseSounds()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            source.UnPause();
        }
    }

    private float MapFloatToVolume(float value)
    {
        return Mathf.Log10(value) * 20;
    }

    public void ChangeEffectVolume(float value)
    {
        audioMixer.SetFloat("EffectsVolume", MapFloatToVolume(value));
    }    
}
