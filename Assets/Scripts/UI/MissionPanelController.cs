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
    [SerializeField]
    private InfoTextController missionTextController;

    private float missionTextDefaultHeight;

    private void Awake()
    {
        missionText.text = "";
        objectiveText.text = "";
        MissionsManager.Instance.MissionChangeEvent.AddListener(OnMissionChange);
        MissionsManager.Instance.ObjectiveGroupChangeEvent.AddListener(OnObjectiveGroupChange);
        missionTextController.hideAfterDisplayEnd = false;
        missionText.ForceMeshUpdate();
        missionTextDefaultHeight = missionText.rectTransform.rect.height;
    }

    private void Start()
    {
        EventManager.Instance.AddListener("PlayerDeath", HideMissionsPanel);
        EventManager.Instance.AddListener("Victory", HideMissionsPanel);
        EventManager.Instance.AddListener("GameReloaded", ShowMissionsPanel);
    }

    private void HideMissionsPanel()
    {
        missionText.enabled = false;
        objectiveText.enabled = false;
    }

    private void ShowMissionsPanel()
    {
        missionText.enabled = true;
        objectiveText.enabled = true;
    }

    private void OnObjectiveGroupChange(string groupLabel)
    {
        if (groupLabel == null)
        {
            objectiveText.text = "";
        }
        else
        {
            objectiveText.text = groupLabel;
        }
        //UpdateObjectiveTextHeight(groupLabel);
    }

    /*
    private void UpdateObjectiveTextHeight(string groupLabel)
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
                Debug.LogErrorFormat("Objective {0} label is to long - it doesn't fit on panel", groupLabel);
            }
        }
        else
        {
            objectiveText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, objectiveTextDefaultHeight);
        }
    }
    */

    private void OnMissionChange(string missionLabel)
    {
        if (missionLabel == null)
        {
            missionText.text = "";
        }
        else
        {
            int linesRequired = missionTextController.TypeText(missionLabel, 0);
            UpdateMissionTextHeight(linesRequired);
        }
    }

    private void UpdateMissionTextHeight(int linesRequired)
    {
        if (missionText.rectTransform.rect.height == linesRequired * missionTextDefaultHeight)
        {
            return;
        }
        if (linesRequired < 3)
        {
            missionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, missionTextDefaultHeight * linesRequired);
            UpdateObjectiveTextPosition(missionTextDefaultHeight * linesRequired);
        }
        else
        {
            Debug.LogError("Mission label is to long - it doesn't fit on panel");
        }
    }

    private void UpdateObjectiveTextPosition(float top)
    {
        objectiveText.rectTransform.anchoredPosition = new Vector2(objectiveText.rectTransform.anchoredPosition.x, -top);
    }
}
