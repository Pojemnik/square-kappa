using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LevelBoundaryController : MonoBehaviour
{
    [SerializeField]
    private float killTime;

    private float counterValue;
    private bool inBounds;

    private void Awake()
    {
        inBounds = true;
    }

    private void Update()
    {
        if(!inBounds)
        {
            counterValue += Time.deltaTime;
            if(counterValue >= killTime)
            {
                inBounds = true;
                counterValue = 0;
                EventManager.Instance.TriggerEvent("PlayerOut");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            inBounds = false;
            counterValue = 0;
            EventManager.Instance.TriggerEvent("PlayerOutWarning");
        }
    }
}
