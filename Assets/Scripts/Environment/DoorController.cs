using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [System.Serializable]
    public struct DoorPartConfig
    {
        public Vector3 closedPosition;
        public Vector3 openedPosition;
        public Vector3 closedRotation;
        public Vector3 openedRotation;
    }

    [SerializeField]
    private GameObject lowerPart;
    [SerializeField]
    private GameObject upperPart;
    [SerializeField]
    private DoorPartConfig upperPartConfig;
    [SerializeField]
    private DoorPartConfig lowerPartConfig;
    [SerializeField]
    private float openingTime;

    private float time;
    private bool isOpening;

    public void Open()
    {
        time = 0;
        isOpening = true;
    }

    public void Close()
    {
        throw new System.NotImplementedException("Closing doors is not supported yet");
    }

    private void Update()
    {
        if (isOpening)
        {
            time += Time.deltaTime;
            lowerPart.transform.localPosition = Vector3.Lerp(lowerPartConfig.closedPosition, lowerPartConfig.openedPosition, time / openingTime);
            upperPart.transform.localPosition = Vector3.Lerp(upperPartConfig.closedPosition, upperPartConfig.openedPosition, time / openingTime);
            lowerPart.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(lowerPartConfig.closedRotation), Quaternion.Euler(lowerPartConfig.openedRotation), time / openingTime);
            upperPart.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(upperPartConfig.closedRotation), Quaternion.Euler(upperPartConfig.openedRotation), time / openingTime);
            if(time >= openingTime)
            {
                isOpening = false;
            }
        }
    }
}
