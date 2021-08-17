using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestructor : MonoBehaviour
{
    [SerializeField]
    private float delay;
    [SerializeField]
    private bool particleMode;

    void Start()
    {
        if (particleMode)
        {
            delay = GetComponent<ParticleSystem>().main.duration;
            Destroy(gameObject, delay);
        }
        else
        {
            Destroy(gameObject, delay);
        }
    }
}
