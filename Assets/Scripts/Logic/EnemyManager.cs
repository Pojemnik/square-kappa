using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    private Dictionary<int, GameObject> enemies;

    public List<GameObject> EnemiesList { get => enemies.Values.ToList(); }
    public UnityEvent enemiesListChangedEvent;

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
        enemies = new Dictionary<int, GameObject>();
    }
}
