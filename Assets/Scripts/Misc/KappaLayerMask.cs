using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class KappaLayerMask
{
    private static int enemyVisionMask = LayerMask.GetMask("Enemy", "PlayerProjectile", "EnemyProjectile", "PlayerEnvironmentalCollision", "Objectives");
    private static int enemyMovementMask = LayerMask.GetMask("Enemy", "PlayerProjectile", "EnemyProjectile", "PlayerEnvironmentalCollision", "Objectives");
    private static int playerVisionMask = LayerMask.GetMask("Enemy", "PlayerProjectile", "EnemyProjectile", "PlayerEnvironmentalCollision", "Objectives", "Transparent");

    public static int EnemyVisionMask { get => enemyVisionMask; }
    public static int EnemyMovementMask { get => enemyMovementMask; }
    public static int PlayerVisionMask { get => playerVisionMask; }
}
