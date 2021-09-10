using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupObjective : Objective
{
    public List<PickableItem> items;

    private int pickedItemsCounter;

    protected override void Awake()
    {
        base.Awake();
        foreach(PickableItem item in items)
        {
            item.PickedUp += OnItemPickup;
            item.Dropped += OnItemDrop;
        }
        pickedItemsCounter = 0;
    }

    private void OnItemPickup(object sender, PickableItem item)
    {
        if(displayDebugInfo)
        {
            Debug.LogFormat("Item {0}, one of the triggers of {1} objective picked up", item.name, name);
        }
        if(pickedItemsCounter == 0)
        {
            Complete();
        }
        pickedItemsCounter++;
    }

    private void OnItemDrop(object sender, PickableItem item)
    {
        if(this == null)
        {
            return;
        }
        if (pickedItemsCounter == 1)
        {
            Uncomplete();
        }
        if (pickedItemsCounter <= 0)
        {
            Debug.LogErrorFormat("Critical error. Picked items count less than zero in objective {0}", name);
        }
        pickedItemsCounter--;
        if (item != null)
        {
            if (displayDebugInfo)
            {
                Debug.LogFormat("Item {0}, one of the triggers of {1} objective dropped", item.name, name);
            }
        }
    }
}
