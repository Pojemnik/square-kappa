using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class ItemsManager : Singleton<ItemsManager>
{
    private Dictionary<int, GameObject> items;

    protected ItemsManager() { }

    public List<GameObject> ItemsList { get => items.Values.ToList(); }
    public UnityEvent itemsListChangedEvent;

    public void AddItem(GameObject item)
    {
        items.Add(item.GetInstanceID(), item);
        itemsListChangedEvent.Invoke();
    }

    public void RemoveItem(GameObject item)
    {
        items.Remove(item.GetInstanceID());
        itemsListChangedEvent.Invoke();
    }

    private void Awake()
    {
        items = new Dictionary<int, GameObject>();
    }
}
