using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakableObjectSettings", menuName = "ScriptableObjects/BreakableObjectSettings")]
public class BreakableObjectSettings : ScriptableObject
{
    public FragmentsSet fragmentsSet;
    [Tooltip("Maximum distance from object's center in which fragments can spawn")]
    public float fragmentsSpawnRange;
    public MinMax<int> fragmentsNumber;
    [Tooltip("Force applied to created fragment")]
    public MinMax<float> fragmentsInitialForce;
    public MinMax<float> fragmentsScale;
    public MinMax<float> fragmentsAngularForce;
}

[System.Serializable]
public class MinMax<T>
{
    public T min;
    public T max;
}