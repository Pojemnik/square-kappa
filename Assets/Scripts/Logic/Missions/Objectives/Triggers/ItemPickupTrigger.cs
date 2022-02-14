using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickupTrigger : DoubleTrigger
{
    public List<PickableItem> items;

    private int pickedItemsCounter;

    private void Awake()
    {
        foreach (PickableItem item in items)
        {
            if (item == null)
            {
                Debug.LogWarningFormat("Null item in pickup objective {0}", gameObject.name);
                continue;
            }
            item.PickedUp += OnItemPickup;
            item.Dropped += OnItemDrop;
        }
        pickedItemsCounter = 0;
    }

    private void OnItemPickup(object sender, PickableItem item)
    {
        if (pickedItemsCounter == 0)
        {
            triggerFirstEvent();
        }
        pickedItemsCounter++;
    }

    private void OnItemDrop(object sender, PickableItem item)
    {
        if (this == null)
        {
            return;
        }
        if (pickedItemsCounter == 1)
        {
            triggerSecondEvent();
        }
        if (pickedItemsCounter <= 0)
        {
            Debug.LogErrorFormat("Critical error. Picked items count less than zero in objective {0}", name);
        }
        pickedItemsCounter--;
    }
}
