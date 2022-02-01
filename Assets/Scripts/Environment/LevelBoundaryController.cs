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
    private int currentlyCheckedTriggers;

    private void Awake()
    {
        inBounds = false;
        currentlyCheckedTriggers = 0;
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
            currentlyCheckedTriggers--;
            if (currentlyCheckedTriggers == 0)
            {
                inBounds = false;
                counterValue = 0;
                EventManager.Instance.TriggerEvent("PlayerOutWarning");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inBounds = true;
            counterValue = 0;
            currentlyCheckedTriggers++;
        }
    }
}
