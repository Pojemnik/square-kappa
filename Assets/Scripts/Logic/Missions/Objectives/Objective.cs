using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Objective : MonoBehaviour
{
    private static int idCounter = 0;
    private int id;
    private bool isCompleted;

    [HideInInspector]
    public IntEvent Completed;
    [HideInInspector]
    public IntEvent Uncompleted;
    [HideInInspector]
    public UnityEvent<Objective> Disabled;

    public int Id { get => id; }

    public string objectiveName;
    public bool defaultState = false;
    public bool isPartial;
    [Header("Debug options")]
    [SerializeField]
    protected bool displayDebugInfo;

    public void Complete()
    {
        Completed.Invoke(id);
        isCompleted = true;
        if(displayDebugInfo)
        {
            Debug.LogFormat("Objective {0} completed", objectiveName);
        }
    }

    public void Uncomplete()
    {
        Uncompleted.Invoke(id);
        isCompleted = false;
        if (displayDebugInfo)
        {
            Debug.LogFormat("Objective {0} uncompleted", objectiveName);
        }
    }

    public void ChangeState()
    {
        if(isCompleted)
        {
            Uncomplete();
        }
        else
        {
            Complete();
        }
    }

    protected virtual void Awake()
    {
        id = idCounter++;
        isCompleted = defaultState;
    }

    protected virtual void OnDisable()
    {
        Disabled.Invoke(this);
    }
}
