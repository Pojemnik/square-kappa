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

[CreateAssetMenu(fileName = "AIRules", menuName = "ScriptableObjects/AI/ShootingRules")]
public class AIShootingRules : ScriptableObject
{
    [System.Serializable]
    public struct AIShootingRule
    {
        public float minDistace;
        public float maxDistace;
        public AIShootingMode shootingMode;
    }

    public List<AIShootingRule> rules;
    public float spottedToShootingDelay;
}

public static class AIShootingRuleCalculator
{
    public static AIShootingMode GetShootingMode(float distanceToTarget, AIShootingRules rules, float timeSinceSpotted)
    {
        if(timeSinceSpotted < rules.spottedToShootingDelay)
        {
            return AIShootingMode.NoShooting;
        }
        foreach(AIShootingRules.AIShootingRule rule in rules.rules)
        {
            bool closerThanMax = distanceToTarget < rule.maxDistace || rule.maxDistace == -1;
            if (distanceToTarget >= rule.minDistace && closerThanMax)
            {
                return rule.shootingMode;
            }
        }
        return AIShootingMode.Error;
    }
}
