using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    public void Interact()
    {
        Debug.LogFormat("Player interacted with {0}", gameObject.name);
    }
}
