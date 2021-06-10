using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum AIShootingMode
{
    Continous,
    Burst,
    OneShot,
    NoShooting,
    Error
}

public class AIShootingRules : ScriptableObject
{
    [System.Serializable]
    public struct AIShootingRule
    {
        readonly public float minDistace;
        readonly public float maxDistace;
        readonly public AIShootingMode shootingMode;
    }

    public List<AIShootingRule> rules;
}

public class AIShootingRuleCalculator
{
    static AIShootingMode GetShootingMode(float distanceToTarget, AIShootingRules rules)
    {
        foreach(AIShootingRules.AIShootingRule rule in rules.rules)
        {
            if(distanceToTarget >= rule.minDistace && distanceToTarget < rule.maxDistace)
            {
                return rule.shootingMode;
            }
        }
        return AIShootingMode.Error;
    }
}
