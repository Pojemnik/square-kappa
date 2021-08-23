using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaticAIConfig", menuName = "ScriptableObjects/AI/StaticConfig")]
public class StaticAIConfig : ScriptableObject
{
    public float chaseTimeout;
}
