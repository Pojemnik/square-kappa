using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("Fragments settings")]
    [SerializeField]
    private FragmentsSet fragments;
    [SerializeField]
    private BreakableObjectSettings settings;

    private Vector3 RandomVector(float minValue, float maxValue)
    {
        return new Vector3(Random.Range(minValue, maxValue), Random.Range(minValue, maxValue), Random.Range(minValue, maxValue));
    }

    private Vector3 RandomVector(MinMax<float> minMax)
    {
        return new Vector3(Random.Range(minMax.min, minMax.max), Random.Range(minMax.min, minMax.max), Random.Range(minMax.min, minMax.max));
    }

    private void CreateFragment()
    {
        GameObject prefab = fragments.GetRandomFragment();
        Vector3 positionDelta = Random.insideUnitSphere * settings.fragmentsSpawnRange;
        Vector3 scale = RandomVector(settings.fragmentsScale);
        Vector3 startRotation = RandomVector(0, 360);
        GameObject fragment = Instantiate(prefab, transform.position + positionDelta, Quaternion.Euler(startRotation));
        fragment.transform.localScale = scale;
        Rigidbody fragmentsRB = fragment.GetComponent<Rigidbody>();
        float velocity = Random.Range(settings.fragmentsVelocity.min, settings.fragmentsVelocity.max);
        fragmentsRB.AddExplosionForce(velocity, transform.position, settings.fragmentsSpawnRange * 10);
        fragmentsRB.AddTorque(RandomVector(settings.fragmentsAngularForce));
    }

    public void OnBreak()
    {
        int fragmentsNumber = Random.Range(settings.fragmentsNumber.min, settings.fragmentsNumber.max);
        for (int i = 0; i < fragmentsNumber; i++)
        {
            CreateFragment();
        }
        Destroy(gameObject);
    }
}
