using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RagdollPart : MonoBehaviour
{
    private new Collider collider;
    private bool collidesWithPlayer;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        collider.isTrigger = true;
    }

    private void Start()
    {
        StartCoroutine(CheckCollisionCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 10)
        {
            collidesWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            collidesWithPlayer = false;
            ChangeTriggerStateAndDestroy();
        }
    }

    private IEnumerator CheckCollisionCoroutine()
    {
        yield return new WaitForSeconds(0.1F);
        if (!collidesWithPlayer)
        {
            ChangeTriggerStateAndDestroy();
        }
    }

    private void ChangeTriggerStateAndDestroy()
    {
        collider.isTrigger = false;
        Destroy(this);
    }
}
