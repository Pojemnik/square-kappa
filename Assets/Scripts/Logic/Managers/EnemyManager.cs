using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : Singleton<EnemyManager>
{
    private Dictionary<int, EnemyController> enemies;
    private Dictionary<int,(bool covered, bool onScreen)> enemyVisibility;

    protected EnemyManager() { }

    public List<EnemyController> EnemiesList { get => enemies.Values.ToList(); }
    public event System.EventHandler<List<EnemyController>> enemiesListChangedEvent;
    public GameObject target;
    
    public bool IsEnemyCovered(int enemyId)
    {
        return enemyVisibility[enemyId].covered;
    }

    public bool IsEnemyOnScreen(int enemyId)
    {
        return enemyVisibility[enemyId].onScreen;
    }

    public void AddEnemy(EnemyController enemy)
    {
        enemies.Add(enemy.GetInstanceID(), enemy);
        enemyVisibility.Add(enemy.GetInstanceID(), (false, false));
        if (enemiesListChangedEvent != null)
        {
            enemiesListChangedEvent(this, enemies.Values.ToList());
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy.GetInstanceID());
        enemyVisibility.Remove(enemy.GetInstanceID());
        if(enemiesListChangedEvent != null)
        {
            enemiesListChangedEvent(this, enemies.Values.ToList());
        }
    }

    private void Awake()
    {
        RegisterInstance(this);
        enemies = new Dictionary<int, EnemyController>();
        enemyVisibility = new Dictionary<int, (bool covered, bool onScreen)>();
    }

    private void Update()
    {
        foreach(EnemyController enemy in enemies.Values)
        {
            AddEnemyToSetIfVisible(enemy);
        }
    }

    private void AddEnemyToSetIfVisible(EnemyController enemy)
    {
        Vector3 enemyArmaturePosition = enemy.core.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(enemyArmaturePosition);
        int enemyId = enemy.GetInstanceID();
        enemyVisibility[enemyId] = (IsEnemyHidden(enemy, enemyArmaturePosition, Camera.main.transform.position), IsScreenPointInViewport(screenPos));
    }

    private bool IsScreenPointInViewport(Vector3 screenPos)
    {
        bool onScreenX = screenPos.x > 0 && screenPos.x < Camera.main.pixelWidth;
        bool onScreenY = screenPos.y > 0 && screenPos.y < Camera.main.pixelHeight;
        bool onScreenZ = screenPos.z > 0;
        return onScreenX && onScreenY && onScreenZ;
    }

    private bool IsEnemyHidden(EnemyController enemy, Vector3 enemyArmaturePosition, Vector3 cameraPos)
    {
        Debug.DrawRay(cameraPos, enemyArmaturePosition - cameraPos);
        if (Physics.Raycast(cameraPos, enemyArmaturePosition - cameraPos, out RaycastHit hit, float.PositiveInfinity, KappaLayerMask.PlayerVisionMask))
        {
            bool isEnemyPart = hit.collider.gameObject == enemy.gameObject || hit.collider.transform.IsChildOf(enemy.transform);
            if (!isEnemyPart)
            {
                return true;
            }
        }
        else
        {
            //This should never happen
            Debug.LogWarningFormat("Enemy {0} not hit by raycast", enemy.gameObject.name);
            return true;
        }
        return false;
    }
}
