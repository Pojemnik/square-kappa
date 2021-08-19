using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectiveZone : Objective
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            print(string.Format("Objective zone {0} entered", gameObject.name));
            Complete();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print(string.Format("Objective zone {0} left", gameObject.name));
            Uncomplete();
        }
    }
}
