using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RagdollPart : MonoBehaviour
{
    private new Collider collider;
    private bool collidesWithPlayer;
    private bool collides;

    private void Awake()
    {
        //collider = GetComponent<Collider>();
        //collider.isTrigger = true;
        //collides = false;
    }

    private void Start()
    {
        //StartCoroutine(CheckCollisionCoroutine());
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0, 100), Random.Range(0, 100), Random.Range(0, 100)));
    }

    private void OnTriggerEnter(Collider other)
    {
        collides = true;
        if (other.gameObject.layer == 10)
        {
            collidesWithPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        collides = false;
        if (other.gameObject.layer == 10)
        {
            collidesWithPlayer = false;
            ChangeTriggerStateAndDestroy();
        }
    }

    private IEnumerator CheckCollisionCoroutine()
    {
        yield return new WaitForSeconds(0.1F);
        while (true)
        {
            if (collides)
            {
                yield return new WaitForSeconds(0.1F);
            }
            else
            {
                ChangeTriggerStateAndDestroy();
                break;
            }
        }
    }

    private void ChangeTriggerStateAndDestroy()
    {
        collider.isTrigger = false;
        Destroy(this);
    }
}
