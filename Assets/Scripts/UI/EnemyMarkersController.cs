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
    [SerializeField]
    [Tooltip("Sets minimum and maximum marker scale. " +
        "Minimum is used, when distance from enemy = Detection Range, " +
        "maximum, when distance = 0. Game needs to be restarted to apply changes")]
    private MinMax<float> markerScaleBounds;
    [SerializeField]
    [Tooltip("Display markers on enemies covered by an object")]
    private bool showHiddenEnemies;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private GameObject markerPrefab;

    private List<GameObject> enemies;
    private Dictionary<int, GameObject> arrows;
    private Dictionary<int, GameObject> markers;
    private float scaleFactor;
    private int raycastLayerMask;

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

    private bool EnemyHiddenOrTooFar(Vector3 enemyArmaturePosition, Vector3 cameraPos)
    {
        Debug.DrawRay(cameraPos, enemyArmaturePosition - cameraPos);
        if (Physics.Raycast(cameraPos, enemyArmaturePosition - cameraPos, out RaycastHit hit, detectionRange, raycastLayerMask))
        {
            if (!hit.collider.gameObject.CompareTag("Enemy") && !showHiddenEnemies)
            {
                print("Enemy hidden behind an object");
                return true;
            }
        }
        else
        {
            //Further than detection range
            print("Raycast hit nothing");
            return true;
        }
        return false;
    }

    private void Awake()
    {
        arrows = new Dictionary<int, GameObject>();
        markers = new Dictionary<int, GameObject>();
        scaleFactor = (markerScaleBounds.min - markerScaleBounds.max) / detectionRange;
        //Ignore layers: player, player projectile, enemy projectile
        raycastLayerMask = ~((1 << 6) | (1 << 8) | (1 << 9));
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
            Vector3 enemyArmaturePosition = enemy.transform.GetChild(0).position;
            float distanceToEnemy = (cameraPos - enemyArmaturePosition).magnitude;
            GameObject marker = markers[enemyId];
            GameObject arrow = arrows[enemyId];
            if (EnemyHiddenOrTooFar(enemyArmaturePosition, cameraPos))
            {
                arrow.SetActive(false);
                marker.SetActive(false);
                continue;
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(enemyArmaturePosition);
            float scale = scaleFactor * distanceToEnemy + markerScaleBounds.max;
            if (IsScreenPointInViewport(screenPos))
            {
                //On screen, display marker
                marker.transform.position = screenPos;
                marker.SetActive(true);
                arrow.SetActive(false);
                marker.transform.localScale = new Vector3(scale, scale, 1);
            }
            else
            {
                //Not on screen, display arrow
                marker.SetActive(false);
                arrow.SetActive(true);
                Transform cameraTransform = Camera.main.transform;
                Vector3 towardsEnemy = enemyArmaturePosition - cameraPos;
                Vector2 screenPosition = new Vector2(Vector3.Dot(towardsEnemy, cameraTransform.right), Vector3.Dot(towardsEnemy, cameraTransform.up));
                Quaternion rotation = Quaternion.Euler(0, 0, Mathf.Atan2(screenPosition.y, screenPosition.x) * Mathf.Rad2Deg);
                Vector2 cameraScreenCenter = new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight) / 2;
                arrow.transform.position = rotation * Vector3.right * arrowsDistanceFromCenter + (Vector3)cameraScreenCenter;
                arrow.transform.rotation = rotation;
                arrow.transform.localScale = new Vector3(scale, scale, 1);
            }
        }
    }
}
