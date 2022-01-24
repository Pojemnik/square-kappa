using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class AnimationSoundController : MonoBehaviour
{
    private AudioSource source;
    [SerializeField]
    private List<AudioClip> soundsToPlay;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    void PlaySound(int index)
    {
        if (index < 0 || index > soundsToPlay.Count || soundsToPlay[index] == null)
        {
            Debug.LogErrorFormat("No sound number {0} in AnimationSoundController {1}", index, gameObject.name);
            return;
        }
        source.PlayOneShot(soundsToPlay[index]);
    }
}
