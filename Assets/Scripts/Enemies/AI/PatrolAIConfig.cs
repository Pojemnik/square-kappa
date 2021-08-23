using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAIConfig", menuName = "ScriptableObjects/AI/PatrolConfig")]

public class PatrolAIConfig : ScriptableObject
{
    public float speedEpsilon;
    public float maxMovementSpeed;
    public float acceleration;
    public float minDistance;
    [HideInInspector]
    public float actualMovementSpeed;
    public float chaseTimeout;
}
