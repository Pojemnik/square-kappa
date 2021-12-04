using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyMarkersController : MonoBehaviour
{
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

    private List<EnemyController> enemies;
    private Dictionary<int, GameObject> arrows;
    private Dictionary<int, GameObject> markers;
    private Dictionary<int, VectorDisplayController> distanceDisplays;
    private float scaleFactor;
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

    private void OnEnemyListChange(object sender, List<EnemyController> enemiesList)
    {
        UpdateMarkers(enemiesList);
    }

    private void UpdateMarkers(List<EnemyController> enemiesList)
    {
        enemies = enemiesList;
        var enemiesIds = enemiesList.Select(e => e.GetInstanceID());
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
                if (displayMode == MarkersDisplayMode.ChangeSize)
                {
                    display.SetActive(false);
                }
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

    private void Awake()
    {
        arrows = new Dictionary<int, GameObject>();
        markers = new Dictionary<int, GameObject>();
        distanceDisplays = new Dictionary<int, VectorDisplayController>();
        scaleFactor = (markerScaleBounds.min - markerScaleBounds.max) / detectionRange;
        displayMode = MarkersDisplayMode.ChangeSize;
    }

    private void Start()
    {
        EnemyManager.Instance.enemiesListChangedEvent += OnEnemyListChange;
        UpdateMarkers(EnemyManager.Instance.EnemiesList);
        foreach (VectorDisplayController display in distanceDisplays.Values)
        {
            display.transform.parent.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Vector3 cameraPos = Camera.main.gameObject.transform.position;
        foreach (EnemyController enemy in enemies)
        {
            int enemyId = enemy.GetInstanceID();
            Vector3 enemyArmaturePosition = enemy.core.transform.position;
            float distanceToEnemy = (cameraPos - enemyArmaturePosition).magnitude;
            GameObject marker = markers[enemyId];
            GameObject arrow = arrows[enemyId];
            bool covered = EnemyManager.Instance.IsEnemyCovered(enemyId);
            bool onScreen = EnemyManager.Instance.IsEnemyOnScreen(enemyId);
            if (covered || distanceToEnemy > detectionRange)
            {
                arrow.SetActive(false);
                marker.SetActive(false);
                continue;
            }
            float scale = scaleFactor * distanceToEnemy + markerScaleBounds.max;
            if (onScreen)
            {
                //On screen, display marker
                marker.transform.position = Camera.main.WorldToScreenPoint(enemyArmaturePosition);
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
                    distanceDisplay.SetValue((int)distanceToEnemy);
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
                float distanceScaleFactor = arrowsDistanceFromCenter * Camera.main.pixelHeight / 900;
                arrow.transform.position = rotation * Vector3.right * distanceScaleFactor + (Vector3)cameraScreenCenter;
                arrow.transform.rotation = rotation;
                if (displayMode == MarkersDisplayMode.ChangeSize)
                {
                    arrow.transform.localScale = new Vector3(scale, scale, 1);
                }
            }
        }
    }
}
