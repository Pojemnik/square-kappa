using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionsManager : MonoBehaviour
{
    public List<Mission> missions;

    private List<List<List<int>>> ids;
    private Dictionary<int, bool> objectiveStates;
    private int missionNumber = 0;
    private int groupNumber = 0;

    private void Start()
    {
        Init();
        //Maybe clear references
    }

    private void Init()
    {
        objectiveStates = new Dictionary<int, bool>();
        ids = new List<List<List<int>>>();
        foreach (Mission mission in missions)
        {
            List<List<int>> temp = new List<List<int>>();
            foreach (ObjectivesGroup group in mission.groups)
            {
                List<int> tmp = new List<int>();
                foreach (Objective objective in group.objectives)
                {
                    tmp.Add(objective.Id);
                    objectiveStates.Add(objective.Id, objective.defaultState);
                    objective.Completed.AddListener(OnObjectiveCompleted);
                    objective.Uncompleted.AddListener(OnObjectiveUncompleted);
                }
                temp.Add(tmp);
            }
            ids.Add(temp);
        }
    }

    private void OnObjectiveCompleted(int id)
    {
        if (!enabled)
        {
            return;
        }
        objectiveStates[id] = true;
        if(CheckForMissionCompletion())
        {
            ProceedToNextMission();
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

    private void ProceedToNextMission()
    {
        groupNumber++;
        Debug.Log("Objectives group finished");
        if (groupNumber == ids[missionNumber].Count)
        {
            missionNumber++;
            Debug.Log("Mission finished");
            if (missionNumber == ids.Count)
            {
                Debug.Log("Game finished or something");
                enabled = false;
            }
        }
    }

    private bool CheckForMissionCompletion()
    {
        foreach (int id in ids[missionNumber][groupNumber])
        {
            if(!objectiveStates[id])
            {
                return false;
            }
        }
        return true;
    }
}
