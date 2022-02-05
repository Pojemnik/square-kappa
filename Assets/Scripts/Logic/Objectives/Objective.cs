using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Objective : MonoBehaviour
{
    private static int idCounter = 0;
    private int id;

    [HideInInspector]
    public IntEvent Completed;
    [HideInInspector]
    public IntEvent Uncompleted;
    [HideInInspector]
    public UnityEvent<Objective> Disabled;

    public int Id { get => id; }

    public string objectiveName;
    public bool defaultState = false;
    [Header("Debug options")]
    [SerializeField]
    protected bool displayDebugInfo;

    protected void Complete()
    {
        Completed.Invoke(id);
        if(displayDebugInfo)
        {
            Debug.LogFormat("Objective {0} completed", objectiveName);
        }
    }

    protected void Uncomplete()
    {
        Uncompleted.Invoke(id);
        if (displayDebugInfo)
        {
            Debug.LogFormat("Objective {0} uncompleted", objectiveName);
        }
    }

    protected virtual void Awake()
    {
        id = idCounter++;
    }

    protected virtual void OnDisable()
    {
        Disabled.Invoke(this);
    }
}
