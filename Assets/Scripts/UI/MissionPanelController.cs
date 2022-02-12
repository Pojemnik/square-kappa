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
    [SerializeField]
    private TextHighlightController missionHighlight;
    [SerializeField]
    private TextHighlightController objectiveHighlight;

    private float missionTextDefaultHeight;
    private float objectiveTextDefaultHeight;
    private string nextObjectiveName;
    private string nextMissionName;

    private void Awake()
    {
        missionText.text = "";
        objectiveText.text = "";
        MissionsManager.Instance.MissionChangeEvent.AddListener(OnMissionChange);
        MissionsManager.Instance.ObjectiveGroupChangeEvent.AddListener(OnObjectiveGroupChange);
        missionTextController.hideAfterDisplayEnd = false;
        missionTextDefaultHeight = missionText.rectTransform.rect.height;
        objectiveTextController.hideAfterDisplayEnd = false;
        objectiveTextDefaultHeight = objectiveText.rectTransform.rect.height;
    }

    private void Start()
    {
        EventManager.Instance.AddListener("PlayerDeath", HideMissionsPanel);
        EventManager.Instance.AddListener("Victory", HideMissionsPanel);
        EventManager.Instance.AddListener("GameReloaded", ShowMissionsPanel);
        objectiveHighlight.animationFinished += (_, _) => UpdateObjectiveText();
        missionHighlight.animationFinished += (_, _) => UpdateMissionText();
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

    private float TestCurve(float t)
    {
        return -t * (t - 1.2f) * (1f / 0.36f);
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
            objectiveHighlight.PlayAnimation(TestCurve, 1.2f);
        }
    }

    private void UpdateObjectiveText()
    {
        int linesRequired = objectiveTextController.TypeText(nextObjectiveName, 0);
        UpdateObjectiveTextHeight(linesRequired);
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
        missionText.text = "";
        if (missionLabel == null)
        {
            nextMissionName = "";
        }
        else
        {
            nextMissionName = missionLabel;
            missionHighlight.PlayAnimation(TestCurve, 1.2f);
        }
    }

    private void UpdateMissionText()
    {
        int linesRequired = missionTextController.TypeText(nextMissionName, 0);
        UpdateMissionTextHeight(linesRequired);
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
