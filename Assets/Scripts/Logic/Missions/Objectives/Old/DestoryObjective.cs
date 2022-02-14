using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BreakableObject))]
public class DestoryObjective : Objective
{
    private BreakableObject breakable;
    protected override void Awake()
    {
        base.Awake();
        breakable = GetComponent<BreakableObject>();
        breakable.broken += (s, a) => ObjectDestoryed();
    }
    private void ObjectDestoryed()
    {
        if (defaultState)
        {
            Uncomplete();
        }
        else
        {
            Complete();
        }
    }
}
