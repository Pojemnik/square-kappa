using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class MissionsManager : Singleton<MissionsManager>
{
    [SerializeField]
    private List<Mission> missions;

    private UnityEvent<Mission> missionChangeEvent;
    private UnityEvent<ObjectivesGroup> objectiveGroupChangeEvent;

    protected MissionsManager() { }

    private Dictionary<string, Objective> objectiveNames;
    private HashSet<string> usedObjectivesTracker;
    private Dictionary<int, bool> objectiveStates;
    private HashSet<int> currentGroupIds;
    private int missionIndex = 0;
    private int groupIndex = 0;

    public UnityEvent<Mission> MissionChangeEvent
    {
        get
        {
            if (missionChangeEvent == null)
            {
                missionChangeEvent = new UnityEvent<Mission>();
            }
            return missionChangeEvent;
        }
    }

    public UnityEvent<ObjectivesGroup> ObjectiveGroupChangeEvent
    {
        get
        {
            if (objectiveGroupChangeEvent == null)
            {
                objectiveGroupChangeEvent = new UnityEvent<ObjectivesGroup>();
            }
            return objectiveGroupChangeEvent;
        }
    }

    private void Awake()
    {
        RegisterInstance(this);
        objectiveStates = new Dictionary<int, bool>();
        if (objectiveNames == null)
        {
            objectiveNames = new Dictionary<string, Objective>();
        }
        usedObjectivesTracker = new HashSet<string>();
        currentGroupIds = new HashSet<int>();
    }

    private void Start()
    {
        if (missions.Count == 0)
        {
            enabled = false;
            return;
        }
        RegisterObjectives();
        SetObjectives();
        CheckForUnusedObjectives();
        UpdateCurrentGroupIds();
        ObjectiveGroupChangeEvent.Invoke(missions[missionIndex].groups[groupIndex]);
        MissionChangeEvent.Invoke(missions[missionIndex]);
        EventManager.Instance.AddListener("GameReloaded", OnGameReload);
        //Debug.Log(string.Format("New mission: {0}", missions[missionIndex].label));
        //Debug.Log(string.Format("New objectives group: {0}", missions[missionIndex].groups[groupIndex].label));
    }

    private void RegisterObjectives()
    {
        Objective[] objectives = (Objective[])FindObjectsOfType(typeof(Objective));
        foreach (Objective objective in objectives)
        {
            objectiveNames.Add(objective.objectiveName, objective);
        }
    }

    private void SetObjectives()
    {
        foreach (Mission mission in missions)
        {
            foreach (ObjectivesGroup group in mission.groups)
            {
                foreach (string name in group.objectiveNames)
                {
                    if (objectiveNames.ContainsKey(name))
                    {
                        SetObjective(name);
                    }
                    else
                    {
                        Debug.LogWarningFormat("No objective named {0}", name);
                    }
                }
            }
        }
    }

    private void SetObjective(string name)
    {
        Objective objective = objectiveNames[name];
        if (usedObjectivesTracker.Contains(name))
        {
            Debug.LogWarningFormat("Objective named {0} used more than once. This can lead to incorrect behaviour", name);
            return;
        }
        objectiveStates.Add(objective.Id, objective.defaultState);
        objective.Completed.AddListener(OnObjectiveCompleted);
        objective.Uncompleted.AddListener(OnObjectiveUncompleted);
        usedObjectivesTracker.Add(name);
    }

    private void CheckForUnusedObjectives()
    {
        foreach (string name in objectiveNames.Keys)
        {
            if (!usedObjectivesTracker.Contains(name))
            {
                Debug.LogWarningFormat("Objective named {0} unused", name);
            }
        }
    }

    private void OnGameReload()
    {
        enabled = true;
        missionIndex = 0;
        groupIndex = 0;
        objectiveNames.Clear();
        objectiveStates.Clear();
        usedObjectivesTracker.Clear();
        currentGroupIds.Clear();
        if (missions.Count == 0)
        {
            enabled = false;
            return;
        }
        RegisterObjectives();
        SetObjectives();
        CheckForUnusedObjectives();
        UpdateCurrentGroupIds();
        ObjectiveGroupChangeEvent.Invoke(missions[missionIndex].groups[groupIndex]);
        MissionChangeEvent.Invoke(missions[missionIndex]);
    }

    public void UnregisterObjective(Objective objective)
    {
        objectiveNames.Remove(objective.objectiveName);
        usedObjectivesTracker.Remove(objective.objectiveName);
        objective.Completed.RemoveListener(OnObjectiveCompleted);
        objective.Uncompleted.RemoveListener(OnObjectiveUncompleted);
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
        //Debug.Log(string.Format("Objectives group {0} finished", missions[missionIndex].groups[groupIndex].label));
        missions[missionIndex].groups[groupIndex].objectivesGroupCompleteEvent.Invoke();
        groupIndex++;
        if (groupIndex == missions[missionIndex].groups.Count)
        {
            ProceedToNextMission();
        }
        if (enabled)
        {
            UpdateCurrentGroupIds();
            ObjectiveGroupChangeEvent.Invoke(missions[missionIndex].groups[groupIndex]);
            //Debug.Log(string.Format("New objectives group: {0}", missions[missionIndex].groups[groupIndex].label));
        }
    }

    private void UpdateCurrentGroupIds()
    {
        currentGroupIds.Clear();
        foreach (Objective objective in missions[missionIndex].groups[groupIndex].objectives)
        {
            currentGroupIds.Add(objective.Id);
        }
    }

    private void ProceedToNextMission()
    {
        //Debug.Log(string.Format("Mission {0} finished", missions[missionIndex].label));
        missions[missionIndex].missionCompleteEvent.Invoke();
        missionIndex++;
        groupIndex = 0;
        if (missionIndex != missions.Count)
        {
            MissionChangeEvent.Invoke(missions[missionIndex]);
            //Debug.Log(string.Format("New mission: {0}", missions[missionIndex].label));
        }
        else
        {
            MissionChangeEvent.Invoke(null);
            ObjectiveGroupChangeEvent.Invoke(null);
            EventManager.Instance.TriggerEvent("Victory");
            enabled = false;
        }
    }

    private bool CheckForObjectiveGroupCompletion()
    {
        foreach (int id in currentGroupIds)
        {
            if (!objectiveStates[id])
            {
                return false;
            }
        }
        return true;
    }
}
