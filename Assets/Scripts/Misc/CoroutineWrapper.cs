using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineWrapper
{
    private bool running;
    public bool Running { get => running; }

    public delegate IEnumerator CoroutineFunction();

    private CoroutineFunction coroutineFunction;
    private Coroutine coroutine;

    public CoroutineWrapper(CoroutineFunction func)
    {
        coroutineFunction = func;
    }

    public void Run(MonoBehaviour context)
    {
        running = true;
        coroutine = context.StartCoroutine(coroutineFunction());
    }

    public void StopIfRunning(MonoBehaviour context)
    {
        if(running)
        {
            context.StopCoroutine(coroutine);
        }
    }

    public void Reset(MonoBehaviour context)
    {
        StopIfRunning(context);
        Run(context);
    }
}
