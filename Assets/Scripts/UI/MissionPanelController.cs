using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPanelController : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI missionText;
    [SerializeField]
    private TMPro.TextMeshProUGUI objectiveText;

    private float missionTextDefaultHeight;
    private float objectiveTextDefaultHeight;

    private void Start()
    {
        missionText.text = "";
        objectiveText.text = "";
        MissionsManager missionsManager = FindObjectOfType<MissionsManager>();
        missionsManager.missionChangeEvent.AddListener(OnMissionChange);
        missionsManager.objectiveGroupChangeEvent.AddListener(OnObjectiveGroupChange);
        missionTextDefaultHeight = missionText.rectTransform.rect.height;
        objectiveTextDefaultHeight = objectiveText.rectTransform.rect.height;
    }

    private void OnObjectiveGroupChange(ObjectivesGroup group)
    {
        if (group == null)
        {
            objectiveText.text = "";
        }
        else
        {
            objectiveText.text = group.label;
        }
        UpdateObjectiveTextHeight(group);
    }

    private void UpdateObjectiveTextHeight(ObjectivesGroup group)
    {
        objectiveText.ForceMeshUpdate();
        if (objectiveText.preferredHeight > objectiveTextDefaultHeight)
        {
            if (objectiveText.preferredHeight < objectiveTextDefaultHeight * 2)
            {
                objectiveText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, objectiveTextDefaultHeight * 2);
            }
            else
            {
                Debug.LogErrorFormat("Objective {0} label is to long - it doesn't fit on panel", group.label);
            }
        }
        else
        {
            objectiveText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, objectiveTextDefaultHeight);
        }
    }

    private void OnMissionChange(Mission mission)
    {
        if (mission == null)
        {
            missionText.text = "";
        }
        else
        {
            missionText.text = mission.label;
        }
        missionText.ForceMeshUpdate();
        UpdateMissionTextHeight(mission);
    }

    private void UpdateMissionTextHeight(Mission mission)
    {
        missionText.ForceMeshUpdate();
        if (missionText.preferredHeight > missionTextDefaultHeight)
        {
            if (missionText.preferredHeight < missionTextDefaultHeight * 2)
            {
                missionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionTextDefaultHeight * 2);
                UpdateObjectiveTextPosition(missionTextDefaultHeight * 2);
            }
            else
            {
                Debug.LogErrorFormat("Mission {0} label is to long - it doesn't fit on panel", mission.label);
            }
        }
        else
        {
            missionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionTextDefaultHeight);
            UpdateObjectiveTextPosition(missionTextDefaultHeight);
        }
    }

    private void UpdateObjectiveTextPosition(float top)
    {
        objectiveText.rectTransform.anchoredPosition = new Vector2(objectiveText.rectTransform.anchoredPosition.x, -top);
    }
}
