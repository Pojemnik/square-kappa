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
    [SerializeField]
    private InfoTextController objectiveTextController;

    private float missionTextDefaultHeight;
    private float objectiveTextDefaultHeight;
    private string nextObjectiveName;

    private void Awake()
    {
        missionText.text = "";
        objectiveText.text = "";
        MissionsManager.Instance.MissionChangeEvent.AddListener(OnMissionChange);
        MissionsManager.Instance.ObjectiveGroupChangeEvent.AddListener(OnObjectiveGroupChange);
        missionTextController.hideAfterDisplayEnd = false;
        missionTextDefaultHeight = missionText.rectTransform.rect.height;
        missionTextController.displayEndEvent += OnMissionTypeEnd;
        objectiveTextController.hideAfterDisplayEnd = false;
        objectiveTextDefaultHeight = objectiveText.rectTransform.rect.height;
    }

    private void OnMissionTypeEnd(object sender, EventArgs e)
    {
        int linesRequired = objectiveTextController.TypeText(nextObjectiveName, 0);
        UpdateObjectiveTextHeight(linesRequired);
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
        objectiveText.text = "";
        if (groupLabel == null)
        {
            nextObjectiveName = "";
        }
        else
        {
            nextObjectiveName = groupLabel;
        }
    }

    private void UpdateObjectiveTextHeight(int linesRequired)
    {
        if (objectiveText.rectTransform.rect.height == linesRequired * objectiveTextDefaultHeight)
        {
            return;
        }
        if (linesRequired < 3)
        {
            objectiveText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, objectiveTextDefaultHeight * linesRequired);
        }
        else
        {
            Debug.LogError("Objective label is to long - it doesn't fit on panel");
        }
    }

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
