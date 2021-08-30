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
    public UnityEvent enemiesListChangedEvent;
    public GameObject target;

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy.GetInstanceID(), enemy);
        enemiesListChangedEvent.Invoke();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        enemies.Remove(enemy.GetInstanceID());
        enemiesListChangedEvent.Invoke();
    }

    private void Awake()
    {
        RegisterInstance(this);
        enemies = new Dictionary<int, GameObject>();
    }
}
