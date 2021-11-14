using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemMarkersController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    [Tooltip("Maximum distance in which marker is being displayed")]
    private float detectionRange;
    [SerializeField]
    [Tooltip("Sets minimum and maximum marker scale. " +
        "Minimum is used, when distance from item = Detection Range, " +
        "maximum, when distance = 0. Game needs to be restarted to apply changes")]
    private MinMax<float> markerScaleBounds;
    [SerializeField]
    [Tooltip("Display markers on items covered by an object")]
    private bool showHiddenItems;
    [SerializeField]
    [Tooltip("Scale of targeted item's marker is multiplied by that factor")]
    private float targetItemScaleMultipler;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject markerPrefab;

    [Header("References")]
    [SerializeField]
    private InteractionTipController interactionTip;

    private List<GameObject> items;
    private Dictionary<int, GameObject> markers;
    private float scaleFactor;
    private int targetId;

    public void OnItemTargeted(object sender, PickableItem item)
    {
        if (item != null)
        {
            targetId = item.gameObject.GetInstanceID();
        }
        else
        {
            targetId = -1;
        }
    }

    private void OnItemListChange(object sender, List<GameObject> itemsList)
    {
        UpdateMarkers(itemsList);
    }

    private void UpdateMarkers(List<GameObject> itemsList)
    {
        items = itemsList;
        var itemsIds = itemsList.Select(e => e.GetInstanceID());
        AddNewKeysOnList(itemsIds);
        RemoveKeysNotOnList(markers, itemsIds);
    }

    private void AddNewKeysOnList(IEnumerable<int> itemsIds)
    {
        foreach (int id in itemsIds)
        {
            if (!markers.ContainsKey(id))
            {
                markers.Add(id, Instantiate(markerPrefab, transform));
            }
        }
    }

    private void RemoveKeysNotOnList(Dictionary<int, GameObject> dict, IEnumerable<int> itemsIds)
    {
        List<int> toRemove = dict.Where(e => !itemsIds.Contains(e.Key)).Select(e => e.Key).ToList();
        foreach (int key in toRemove)
        {
            Destroy(dict[key]);
            dict.Remove(key);
            if(key == targetId)
            {
                targetId = -1;
            }
        }
    }

    private bool IsScreenPointInViewport(Vector3 screenPos)
    {
        bool onScreenX = screenPos.x > 0 && screenPos.x < Camera.main.pixelWidth;
        bool onScreenY = screenPos.y > 0 && screenPos.y < Camera.main.pixelHeight;
        bool onScreenZ = screenPos.z > 0;
        return onScreenX && onScreenY && onScreenZ;
    }

    private bool ItemHiddenOrTooFar(GameObject item, Vector3 cameraPos)
    {
        Vector3 itemPosition = item.transform.position;
        Debug.DrawRay(cameraPos, itemPosition - cameraPos);
        if (Physics.Raycast(cameraPos, itemPosition - cameraPos, out RaycastHit hit, detectionRange, KappaLayerMask.PlayerVisionMask))
        {
            bool isItemPart = hit.collider.gameObject == item || hit.collider.transform.IsChildOf(item.transform);
            if (!isItemPart && !showHiddenItems)
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
        markers = new Dictionary<int, GameObject>();
        scaleFactor = (markerScaleBounds.min - markerScaleBounds.max) / detectionRange;
    }

    private void Start()
    {
        ItemsManager.Instance.itemsListChangedEvent += OnItemListChange;
        ItemsManager.Instance.targetItemChanged += OnItemTargeted;
        UpdateMarkers(ItemsManager.Instance.ItemsList);
    }

    private void Update()
    {
        Vector3 cameraPos = Camera.main.gameObject.transform.position;
        foreach (GameObject item in items)
        {
            int itemId = item.GetInstanceID();
            Vector3 itemPosition = item.transform.position;
            float distanceToItem = (cameraPos - itemPosition).magnitude;
            GameObject marker = markers[itemId];
            if (item.layer == LayerMask.NameToLayer("Enemy") || ItemHiddenOrTooFar(item, cameraPos))
            {
                marker.SetActive(false);
                continue;
            }
            Vector3 screenPos = Camera.main.WorldToScreenPoint(itemPosition);
            float scale = scaleFactor * distanceToItem + markerScaleBounds.max;
            if (IsScreenPointInViewport(screenPos))
            {
                //On screen, display marker
                marker.transform.position = screenPos;
                marker.SetActive(true);
                if(itemId == targetId)
                {
                    scale *= targetItemScaleMultipler;
                    interactionTip.DisplayTip(screenPos, InteractionTipController.TipType.Pickup);
                }
                marker.transform.localScale = new Vector3(scale, scale, 1);
            }
            else
            {
                //Not on screen, hide
                marker.SetActive(false);
            }
        }
        if(targetId == -1)
        {
            interactionTip.HideTip(InteractionTipController.TipType.Pickup);
        }
    }
}
