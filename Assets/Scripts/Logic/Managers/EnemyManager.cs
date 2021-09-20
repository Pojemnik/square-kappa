using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : Singleton<EnemyManager>
{
    private Dictionary<int, GameObject> enemies;

    protected EnemyManager() { }

    public List<GameObject> EnemiesList { get => enemies.Values.ToList(); }
    public event System.EventHandler<List<GameObject>> enemiesListChangedEvent;
    public GameObject target;

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy.GetInstanceID(), enemy);
        if (enemiesListChangedEvent != null)
        {
            enemiesListChangedEvent(this, enemies.Values.ToList());
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy.GetInstanceID());
        if(enemiesListChangedEvent != null)
        {
            enemiesListChangedEvent(this, enemies.Values.ToList());
        }
    }

    private void Awake()
    {
        RegisterInstance(this);
        enemies = new Dictionary<int, GameObject>();
    }
}
