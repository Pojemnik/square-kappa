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
    [Tooltip("Maximum distance in which marker is being displayed")]
    private float detectionRange;
    [SerializeField]
    [Tooltip("Distance between center of the screen end arrowd of enemy marker")]
    private float arrowsDistanceFromCenter;

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
        var toRemove = dict.Where(e => !enemiesIds.Contains(e.Key)).Select(e => e.Key).ToList<int>();
        foreach (int key in toRemove)
        {
            Destroy(dict[key]);
            dict.Remove(key);
        }
    }

    private bool IsScreenPointInViewport(Vector3 screenPos)
    {
        bool onScreenX = screenPos.x > 0 && screenPos.x < Camera.main.pixelWidth;
        bool onScreenY = screenPos.y > 0 && screenPos.y < Camera.main.pixelHeight;
        bool onScreenZ = screenPos.z > 0;
        return onScreenX && onScreenY && onScreenZ;
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
            int enemyId = enemy.GetInstanceID();
            if ((cameraPos - enemy.transform.position).sqrMagnitude > detectionRange * detectionRange)
            {
                //Further than detection range
                markers[enemyId].SetActive(false);
                arrows[enemyId].SetActive(false);
                continue;
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemy.transform.position);
            if (IsScreenPointInViewport(screenPos))
            {
                //On screen, display marker
                markers[enemyId].transform.position = screenPos;
                markers[enemyId].SetActive(true);
                arrows[enemyId].SetActive(false);
            }
            else
            {
                //Not on screen, display arrow
                markers[enemyId].SetActive(false);
                arrows[enemyId].SetActive(true);
                Transform cameraTransform = Camera.main.transform;
                Vector3 towardsEnemy = enemy.transform.position - cameraPos;
                Vector2 screenPosition = new Vector2(Vector3.Dot(towardsEnemy, cameraTransform.right), Vector3.Dot(towardsEnemy, cameraTransform.up));
                Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(screenPosition.y, screenPosition.x) * Mathf.Rad2Deg);
                Vector2 cameraScreenCenter = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight) / 2;
                arrows[enemyId].transform.position = rotation * Vector3.right * arrowsDistanceFromCenter + (Vector3)cameraScreenCenter;
                arrows[enemyId].transform.rotation = rotation;
            }
        }
    }
}