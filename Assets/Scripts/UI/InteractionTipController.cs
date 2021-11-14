using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTipController : MonoBehaviour
{
    public enum TipType
    {
        Pickup,
        Interact
    }

    private class DisplayStatus
    {
        private bool pickup;
        private bool interact;

        public DisplayStatus()
        {
            pickup = false;
            interact = false;
        }

        public bool DisplayNone => !pickup && !interact;

        public bool this[TipType type]
        {
            get { return (type == TipType.Pickup) ? pickup : interact; }
            private set
            {
                if (type == TipType.Pickup)
                {
                    pickup = value;
                }
                else
                {
                    interact = value;
                }
            }
        }

        public bool TrySet(TipType type, bool value)
        {
            if(value)
            {
                if(type == TipType.Interact && pickup)
                {
                    return false;
                }
                if(type == TipType.Pickup && interact)
                {
                    return false;
                }
            }
            this[type] = value;
            return true;
        }
    }

    private UnityEngine.UI.Image image;
    private DisplayStatus displayStatus;

    private void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        displayStatus = new DisplayStatus();
        image.enabled = false;
    }

    public void DisplayTip(Vector3 screenSpacePosition, TipType type)
    {
        if (displayStatus.TrySet(type, true))
        {
            image.transform.position = screenSpacePosition;
            image.enabled = true;
        }
        else
        {
            //Debug.LogWarning("Interaction tip error: tried to display more than one tip at once");
        }
    }

    public void DisplayTip(Vector3 worldSpacePisition, Camera camera, TipType type)
    {
        DisplayTip(camera.WorldToScreenPoint(worldSpacePisition), type);
    }

    public void HideTip(TipType type)
    {
        displayStatus.TrySet(type, false);
        if(displayStatus.DisplayNone)
        {
            image.enabled = false;
        }
    }
}
