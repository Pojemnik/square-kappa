using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LevelBoundaryController : MonoBehaviour
{
    [SerializeField]
    private float killTime;
    [SerializeField]
    private GameObject volume;

    private float counterValue;
    private bool inBounds;
    private int currentlyCheckedTriggers;

    private void Awake()
    {
        SetInBounds(true);
        currentlyCheckedTriggers = 0;
    }

    private void Update()
    {
        if(!inBounds)
        {
            counterValue += Time.deltaTime;
            if(counterValue >= killTime)
            {
                SetInBounds(true);
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
                SetInBounds(false);
                counterValue = 0;
                EventManager.Instance.TriggerEvent("PlayerOutWarning");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetInBounds(true);
            counterValue = 0;
            currentlyCheckedTriggers++;
        }
    }

    private void SetInBounds(bool value)
    {
        inBounds = value;
        volume.SetActive(value);
    }
}
