using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeManagerConfig", menuName = "ScriptableObjects/Managers/TimeManagerConfig")]
public class TimeManagerConfig : ScriptableObject
{
    public float slowTime;
    public float uiStopTimeDuration;
}
