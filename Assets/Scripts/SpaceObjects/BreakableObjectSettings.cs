using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BreakableObjectSettings", menuName = "ScriptableObjects/BreakableObjectSettings")]
public class BreakableObjectSettings : ScriptableObject
{
    

    public FragmentsSet fragmentsSet;
    public float fragmentsSpawnRange;
    public MinMax<int> fragmentsNumber;
    public MinMax<float> fragmentsVelocity;
    public MinMax<float> fragmentsScale;
    public MinMax<float> fragmentsAngularForce;
}

[System.Serializable]
public class MinMax<T>
{
    public T min;
    public T max;
}