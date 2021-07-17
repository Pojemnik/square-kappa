using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAIConfig", menuName = "ScriptableObjects/PatrolAIConfig")]

public class PatrolAIConfig : ScriptableObject
{
    public float speedEpsilon;
    public float rotationalSpeed;
    public float maxMovementSpeed;
    public float acceleration;
    [HideInInspector]
    public float actualMovementSpeed;
    public List<Vector3> lookAroundRotations;
}
