using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneSoundSource : MonoBehaviour
{
    [SerializeField]
    private SerializableDictionary<string, AudioClip> clipsToPlayOnEvent;
    private AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        foreach(var pair in clipsToPlayOnEvent)
        {
            if (pair.Value != null)
            {
                EventManager.Instance.AddListener(pair.Key, () => source.PlayOneShot(pair.Value));
            }
        }
    }

}
