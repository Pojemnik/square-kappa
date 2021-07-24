using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AIRulesInterpretation", menuName = "ScriptableObjects/AI/ShootingRulesInterpretation")]
public class AIShootingRulesInterpretation : ScriptableObject
{
    [Header("One shoot mode")]
    public float TimeBetweenShoots;

    [Header("Burst mode")]
    public float TimeBetweenBursts;
    public float BurstDuration;
}
