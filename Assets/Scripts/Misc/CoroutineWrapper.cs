using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineWrapper
{
    private bool running;
    public bool Running { get => running; }

    public delegate IEnumerator CoroutineFunction();
    public event System.EventHandler onCoroutineEnd;

    private Coroutine coroutine;
    private CoroutineFunction coroutineFunction;

    public CoroutineWrapper(CoroutineFunction func)
    {
        coroutineFunction = func;
    }

    public void Run(MonoBehaviour context)
    {
        coroutine = context.StartCoroutine(CoroutineFunctionWrapper(context));
    }

    public void StopIfRunning(MonoBehaviour context)
    {
        if(running)
        {
            Stop(context);
        }
    }

    private void Stop(MonoBehaviour context)
    {
        context.StopCoroutine(coroutine);
        running = false;
    }

    public void Reset(MonoBehaviour context)
    {
        StopIfRunning(context);
        Run(context);
    }

    private IEnumerator CoroutineFunctionWrapper(MonoBehaviour context)
    {
        running = true;
        yield return context.StartCoroutine(coroutineFunction());
        running = false;
        onCoroutineEnd?.Invoke(this, null);
    }
}
