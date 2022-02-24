using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneSoundSource : MonoBehaviour
{
    [SerializeField]
    private SerializableDictionary<string, AudioClip> clipsToPlayOnEvent;
    private AudioSource source;
    private List<(string, UnityEngine.Events.UnityAction)> actions;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        actions = new List<(string, UnityEngine.Events.UnityAction)>();
        foreach(var pair in clipsToPlayOnEvent)
        {
            if (pair.Value != null && pair.Key != null)
            {
                actions.Add((pair.Key, () => source.PlayOneShot(pair.Value)));
                EventManager.Instance.AddListener(pair.Key, actions[actions.Count-1].Item2);
            }
        }
    }

    private void OnDestroy()
    {
        foreach((string, UnityEngine.Events.UnityAction) action in actions)
        {
            EventManager.Instance?.RemoveListener(action.Item1, action.Item2);
        }
    }

}
