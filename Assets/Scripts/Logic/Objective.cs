using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Objective : MonoBehaviour
{
    private static int idCounter = 0;
    private int id;

    public IntEvent Completed;
    public IntEvent Uncompleted;
    public int Id { get => id; }

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
