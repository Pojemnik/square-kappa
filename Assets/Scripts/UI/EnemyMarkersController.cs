using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyMarkersController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private EnemyManager enemyManager;

    [Header("Settings")]
    [SerializeField]
    private float detectionRange;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private GameObject markerPrefab;

    private List<GameObject> enemies;
    private Dictionary<int, GameObject> arrows;
    private Dictionary<int, GameObject> markers;

    private void OnEnemyListChange()
    {
        enemies = enemyManager.EnemiesList;
        var enemiesIds = enemies.Select(e => e.GetInstanceID());
        AddNewKeysOnList(enemiesIds);
        RemoveKeysNotOnList(arrows, enemiesIds);
        RemoveKeysNotOnList(markers, enemiesIds);
    }

    private void AddNewKeysOnList(IEnumerable<int> enemiesIds)
    {
        foreach (int id in enemiesIds)
        {
            if (!arrows.ContainsKey(id))
            {
                arrows.Add(id, Instantiate(arrowPrefab, transform));
            }
            if (!markers.ContainsKey(id))
            {
                markers.Add(id, Instantiate(markerPrefab, transform));
            }
        }
    }

    private void RemoveKeysNotOnList(Dictionary<int, GameObject> dict, IEnumerable<int> enemiesIds)
    {
        var toRemove = dict.Where(e => !enemiesIds.Contains(e.Key)).Select(e => e.Key);
        foreach (int key in toRemove)
        {
            dict.Remove(key);
        }
    }

    private void Awake()
    {
        arrows = new Dictionary<int, GameObject>();
        markers = new Dictionary<int, GameObject>();
    }

    private void Start()
    {
        enemyManager.enemiesListChangedEvent.AddListener(OnEnemyListChange);
        OnEnemyListChange();
    }

    private void Update()
    {
        Vector3 cameraPos = Camera.main.gameObject.transform.position;
        foreach (GameObject enemy in enemies)
        {
            if ((cameraPos - enemy.transform.position).sqrMagnitude > detectionRange * detectionRange)
            {
                int enemyId = enemy.GetInstanceID();
                markers[enemyId].SetActive(false);
                arrows[enemyId].SetActive(false);
                continue;
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            //print(string.Format("Enemy: {0} in position {1}", enemy.name, screenPos));
            bool onScreenX = screenPos.x > 0 && screenPos.x < Camera.main.pixelWidth;
            bool onScreenY = screenPos.y > 0 && screenPos.y < Camera.main.pixelHeight;
            bool onScreenZ = screenPos.x > 0;
            if (onScreenX && onScreenY && onScreenZ)
            {
                int enemyId = enemy.GetInstanceID();
                markers[enemyId].transform.position = screenPos;
                markers[enemyId].SetActive(true);
                arrows[enemyId].SetActive(false);
            }
            else
            {
                int enemyId = enemy.GetInstanceID();
                markers[enemyId].SetActive(false);
                arrows[enemyId].SetActive(false);
                //Not on screen, display arrow
            }
        }
    }
}
