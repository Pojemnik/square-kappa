using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetpackController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMoveUp()
    {
        animator.SetTrigger("MoveUpDown");
    }

    public void OnMoveDown()
    {
        animator.SetTrigger("MoveUpDown");
    }

    public void OnMoveLeft()
    {
        animator.SetTrigger("Stop");
    }

    public void OnMoveRight()
    {
        animator.SetTrigger("Stop");
    }

    public void OnMoveForward()
    {
        animator.SetTrigger("MoveForward");
    }

    public void OnMoveBackward()
    {
        animator.SetTrigger("MoveBackward");
    }
    public void OnStop()
    {
        animator.SetTrigger("Stop");
    }
}
