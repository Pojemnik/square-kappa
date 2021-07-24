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
    [SerializeField]
    [Tooltip("Distace between enemy marker and distance display")]
    private Vector2 distanceDisplayOffset;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject arrowPrefab;
    [SerializeField]
    private GameObject markerPrefab;
    [SerializeField]
    private GameObject distanceDisplayPrefab;

    private List<GameObject> enemies;
    private Dictionary<int, GameObject> arrows;
    private Dictionary<int, GameObject> markers;
    private Dictionary<int, VectorDisplayController> distanceDisplays;
    private float scaleFactor;
    private int raycastLayerMask;
    private MarkersDisplayMode displayMode;

    private enum MarkersDisplayMode
    {
        ChangeSize,
        ShowDistance
    }

    public void ChangeDisplayMode()
    {
        if (displayMode == MarkersDisplayMode.ChangeSize)
        {
            displayMode = MarkersDisplayMode.ShowDistance;
            foreach (VectorDisplayController display in distanceDisplays.Values)
            {
                display.transform.parent.gameObject.SetActive(true);
            }
            foreach (GameObject marker in markers.Values)
            {
                marker.transform.localScale = Vector3.one;
            }
        }
        else if (displayMode == MarkersDisplayMode.ShowDistance)
        {
            displayMode = MarkersDisplayMode.ChangeSize;
            foreach (VectorDisplayController display in distanceDisplays.Values)
            {
                display.transform.parent.gameObject.SetActive(false);
            }
        }
        print(string.Format("Display mode: {0}", displayMode));
    }

    private void OnEnemyListChange()
    {
        enemies = enemyManager.EnemiesList;
        var enemiesIds = enemies.Select(e => e.GetInstanceID());
        AddNewKeysOnList(enemiesIds);
        RemoveKeysNotOnList(arrows, enemiesIds);
        RemoveKeysNotOnList(markers, enemiesIds);
        RemoveKeysNotOnList(distanceDisplays, enemiesIds);
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
            if (!distanceDisplays.ContainsKey(id))
            {
                GameObject display = Instantiate(distanceDisplayPrefab, markers[id].transform);
                display.transform.localPosition = distanceDisplayOffset;
                distanceDisplays.Add(id, display.transform.GetChild(0).gameObject.GetComponent<VectorDisplayController>());
            }
        }
    }

    private void RemoveKeysNotOnList(Dictionary<int, GameObject> dict, IEnumerable<int> enemiesIds)
    {
        List<int> toRemove = dict.Where(e => !enemiesIds.Contains(e.Key)).Select(e => e.Key).ToList();
        foreach (int key in toRemove)
        {
            Destroy(dict[key]);
            dict.Remove(key);
        }
    }

    private void RemoveKeysNotOnList(Dictionary<int, VectorDisplayController> dict, IEnumerable<int> enemiesIds)
    {
        List<int> toRemove = dict.Where(e => !enemiesIds.Contains(e.Key)).Select(e => e.Key).ToList();
        foreach (int key in toRemove)
        {
            Destroy(dict[key].transform.parent.gameObject);
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
                return true;
            }
        }
        else
        {
            //Further than detection range
            return true;
        }
        return false;
    }

    private void Awake()
    {
        arrows = new Dictionary<int, GameObject>();
        markers = new Dictionary<int, GameObject>();
        distanceDisplays = new Dictionary<int, VectorDisplayController>();
        scaleFactor = (markerScaleBounds.min - markerScaleBounds.max) / detectionRange;
        //Ignore layers: player, player projectile, enemy projectile
        raycastLayerMask = ~((1 << 6) | (1 << 8) | (1 << 9));
        displayMode = MarkersDisplayMode.ChangeSize;
    }

    private void Start()
    {
        enemyManager.enemiesListChangedEvent.AddListener(OnEnemyListChange);
        OnEnemyListChange();
        foreach (VectorDisplayController display in distanceDisplays.Values)
        {
            display.transform.parent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Vector3 cameraPos = Camera.main.gameObject.transform.position;
        foreach (GameObject enemy in enemies)
        {
            int enemyId = enemy.GetInstanceID();
            Vector3 enemyArmaturePosition = enemy.transform.GetChild(0).GetChild(0).GetChild(0).position;
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
                if (displayMode == MarkersDisplayMode.ChangeSize)
                {
                    marker.transform.localScale = new Vector3(scale, scale, 1);
                }
                else if (displayMode == MarkersDisplayMode.ShowDistance)
                {
                    //Display distance
                    VectorDisplayController distanceDisplay = distanceDisplays[enemyId];
                    distanceDisplay.UpdateValue((int)distanceToEnemy);
                }
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
                if (displayMode == MarkersDisplayMode.ChangeSize)
                {
                    arrow.transform.localScale = new Vector3(scale, scale, 1);
                }
            }
        }
    }
}
