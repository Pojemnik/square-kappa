using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KappaLayerMask
{
    private static int enemyVisionMask = ~LayerMask.GetMask("Enemy", "PlayerProjectile", "EnemyProjectile",
                                                            "PlayerEnvironmentalCollision", "Objectives", "Transparent", "EnemyEnvironmentalCollision");
    private static int enemyMovementMask = ~LayerMask.GetMask("Enemy", "PlayerProjectile", "EnemyProjectile",
                                                              "PlayerEnvironmentalCollision", "Objectives", "Transparent", "EnemyEnvironmentalCollision");
    private static int playerVisionMask = ~LayerMask.GetMask("Player", "PlayerProjectile", "EnemyProjectile",
                                                             "PlayerEnvironmentalCollision", "Objectives", "Transparent", "EnemyEnvironmentalCollision");
    private static int playerItemTargetingMask = ~LayerMask.GetMask("Player", "PlayerProjectile", "EnemyProjectile",
                                                             "PlayerEnvironmentalCollision", "Objectives", "EnemyEnvironmentalCollision");
    private static int playerMeleAttackMask = ~LayerMask.GetMask("Player", "PlayerProjectile", "EnemyProjectile",
                                                             "PlayerEnvironmentalCollision", "Objectives", "EnemyEnvironmentalCollision");

    public static int EnemyVisionMask { get => enemyVisionMask; }
    public static int EnemyMovementMask { get => enemyMovementMask; }
    public static int PlayerVisionMask { get => playerVisionMask; }
    public static int PlayerItemTargetingMask { get => playerItemTargetingMask; }
    public static int PlayerMeleAttackMask { get => playerMeleAttackMask; }
}
