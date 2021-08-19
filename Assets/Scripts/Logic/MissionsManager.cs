using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    public List<Mission> missions;

    private Dictionary<int, bool> objectiveStates;
    private HashSet<int> curentGroupIds;
    private int missionIndex = 0;
    private int groupIndex = 0;

    private void Start()
    {
        objectiveStates = new Dictionary<int, bool>();
        foreach (Mission mission in missions)
        {
            foreach (ObjectivesGroup group in mission.groups)
            {
                foreach (Objective objective in group.objectives)
                {
                    objectiveStates.Add(objective.Id, objective.defaultState);
                    objective.Completed.AddListener(OnObjectiveCompleted);
                    objective.Uncompleted.AddListener(OnObjectiveUncompleted);
                }
            }
        }
        curentGroupIds = new HashSet<int>();
        UpdateCurrentGroupIds();
        Debug.Log(string.Format("New mission: {0}", missions[missionIndex].label));
        Debug.Log(string.Format("New objectives group: {0}", missions[missionIndex].groups[groupIndex].label));
    }

    private void OnObjectiveCompleted(int id)
    {
        if (!enabled)
        {
            return;
        }
        objectiveStates[id] = true;
        if (CheckForObjectiveGroupCompletion())
        {
            ProceedToNextObjectivesGroup();
        }
    }

    private void OnObjectiveUncompleted(int id)
    {
        if (!enabled)
        {
            return;
        }
        objectiveStates[id] = false;
    }

    private void ProceedToNextObjectivesGroup()
    {
        Debug.Log(string.Format("Objectives group {0} finished", missions[missionIndex].groups[groupIndex].label));
        groupIndex++;
        if (groupIndex == missions[missionIndex].groups.Count)
        {
            ProceedToNextMission();
        }
        if (enabled)
        {
            UpdateCurrentGroupIds();
            Debug.Log(string.Format("New objectives group: {0}", missions[missionIndex].groups[groupIndex].label));
        }
    }

    private void UpdateCurrentGroupIds()
    {
        curentGroupIds.Clear();
        foreach (Objective objective in missions[missionIndex].groups[groupIndex].objectives)
        {
            curentGroupIds.Add(objective.Id);
        }
    }

    private void ProceedToNextMission()
    {
        Debug.Log(string.Format("Mission {0} finished", missions[missionIndex].label));
        missionIndex++;
        if (missionIndex != missions.Count)
        {
            Debug.Log(string.Format("New mission: {0}", missions[missionIndex].label));
        }
        else
        {
            Debug.Log("Game finished or something");
            enabled = false;
        }
    }

    private bool CheckForObjectiveGroupCompletion()
    {
        foreach (int id in curentGroupIds)
        {
            if (!objectiveStates[id])
            {
                return false;
            }
        }
        return true;
    }
}
