using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PatrolAIConfig", menuName = "ScriptableObjects/PatrolAIConfig")]

public class PatrolAIConfig : ScriptableObject
{
    public float speedEpsilon;
    public float rotationalSpeed;
    public float movementSpeed;
    public float acceleration;
}
