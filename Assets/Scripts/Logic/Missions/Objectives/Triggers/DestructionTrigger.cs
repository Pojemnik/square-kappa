using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BreakableObject))]
public class DestructionTrigger : Trigger
{
    private BreakableObject breakable;

    private void Awake()
    {
        breakable = GetComponent<BreakableObject>();
        breakable.broken += (s, a) => ObjectDestoryed();
    }
    private void ObjectDestoryed()
    {
        triggerEvent();
    }
}
