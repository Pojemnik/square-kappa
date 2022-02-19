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

    [System.Serializable]
    private enum DoorState
    {
        Opening,
        Closing,
        Closed,
        Opened
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
    [SerializeField]
    private DoorState defaultState;

    private float time;
    private DoorState state;

    private void Start()
    {
        state = defaultState;
    }

    public void Open()
    {
        switch (state)
        {
            case DoorState.Opened:
            case DoorState.Opening:
                return;
            case DoorState.Closed:
                time = 0;
                break;
            case DoorState.Closing:
                time = (1 - InverseLerp(upperPartConfig.openedPosition, upperPartConfig.closedPosition, upperPart.transform.localPosition)) * openingTime;
                break;
        }
        state = DoorState.Opening;
    }

    public void Close()
    {
        switch (state)
        {
            case DoorState.Closed:
            case DoorState.Closing:
                return;
            case DoorState.Opened:
                time = 0;
                break;
            case DoorState.Opening:
                time = (1 - InverseLerp(upperPartConfig.closedPosition, upperPartConfig.openedPosition, upperPart.transform.localPosition)) * openingTime;
                break;
        }
        state = DoorState.Closing;
    }

    public void ChangeState()
    {
        if (state == DoorState.Closing || state == DoorState.Closed)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }

    private void Update()
    {
        if (state == DoorState.Opening)
        {
            time += Time.deltaTime;
            if (upperPart != null)
            {
                upperPart.transform.localPosition = Vector3.Lerp(upperPartConfig.closedPosition, upperPartConfig.openedPosition, time / openingTime);
                upperPart.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(upperPartConfig.closedRotation), Quaternion.Euler(upperPartConfig.openedRotation), time / openingTime);
            }
            if (lowerPart != null)
            {
                lowerPart.transform.localPosition = Vector3.Lerp(lowerPartConfig.closedPosition, lowerPartConfig.openedPosition, time / openingTime);
                lowerPart.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(lowerPartConfig.closedRotation), Quaternion.Euler(lowerPartConfig.openedRotation), time / openingTime);
            }
            if (time >= openingTime)
            {
                state = DoorState.Opened;
                if (upperPart != null)
                {
                    upperPart.transform.localPosition = upperPartConfig.openedPosition;
                    upperPart.transform.localRotation = Quaternion.Euler(upperPartConfig.openedRotation);
                }
                if (lowerPart != null)
                {
                    lowerPart.transform.localPosition = lowerPartConfig.openedPosition;
                    lowerPart.transform.localRotation = Quaternion.Euler(lowerPartConfig.openedRotation);
                }
            }
        }
        if (state == DoorState.Closing)
        {
            time += Time.deltaTime;
            if (upperPart != null)
            {
                upperPart.transform.localPosition = Vector3.Lerp(upperPartConfig.openedPosition, upperPartConfig.closedPosition, time / openingTime);
                upperPart.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(upperPartConfig.openedRotation), Quaternion.Euler(upperPartConfig.closedRotation), time / openingTime);
            }
            if (lowerPart != null)
            {
                lowerPart.transform.localPosition = Vector3.Lerp(lowerPartConfig.openedPosition, lowerPartConfig.closedPosition, time / openingTime);
                lowerPart.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(lowerPartConfig.openedRotation), Quaternion.Euler(lowerPartConfig.closedRotation), time / openingTime);
            }
            if (time >= openingTime)
            {
                state = DoorState.Closed;
                if (upperPart != null)
                {
                    upperPart.transform.localPosition = upperPartConfig.closedPosition;
                    upperPart.transform.localRotation = Quaternion.Euler(upperPartConfig.closedRotation);
                }
                if (lowerPart != null)
                {
                    lowerPart.transform.localPosition = lowerPartConfig.closedPosition;
                    lowerPart.transform.localRotation = Quaternion.Euler(lowerPartConfig.closedRotation);
                }
            }
        }
    }
}
