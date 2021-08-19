using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Objective : MonoBehaviour
{
    private static int idCounter = 0;
    private int id;

    [HideInInspector]
    public IntEvent Completed;
    [HideInInspector]
    public IntEvent Uncompleted;
    public int Id { get => id; }

    public bool defaultState = false;

    protected void Complete()
    {
        Completed.Invoke(id);
    }

    protected void Uncomplete()
    {
        Uncompleted.Invoke(id);
    }

    protected virtual void Awake()
    {
        id = idCounter++;
    }
}
