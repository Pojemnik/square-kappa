using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class MissionsManager : Singleton<MissionsManager>
{
    class MissionData
    {
        public MissionData nextMission;

        private readonly string label;
        private readonly MissionEvent completed;
        private readonly List<ObjectiveGroupData> groups;
        public List<ObjectiveGroupData> Groups { get => groups; }
        public MissionEvent Completed { get => completed; }
        public string Label { get => label; }

        public MissionData(string missionLabel, MissionEvent completedEvent)
        {
            groups = new List<ObjectiveGroupData>();
            label = missionLabel;
            completed = completedEvent;
        }

        public void AddObjectivesGroup(ObjectiveGroupData group)
        {
            Groups.Add(group);
        }
    }

    class ObjectiveGroupData
    {
        private readonly string label;
        private readonly HashSet<int> objectives;
        private readonly MissionEvent completed;
        private ObjectiveGroupData nextGroup;
        private readonly MissionData mission;

        public ObjectiveGroupData NextGroup { get => nextGroup; set => nextGroup = value; }
        public MissionData Mission { get => mission; }
        public string Label { get => label; }
        public HashSet<int> Objectives { get => objectives; }
        public MissionEvent Completed { get => completed; }

        public ObjectiveGroupData(string objectiveGroupLabel, MissionEvent completedEvent, MissionData ownerMission)
        {
            objectives = new HashSet<int>();
            label = objectiveGroupLabel;
            completed = completedEvent;
            mission = ownerMission;
        }
    }

    [System.Serializable]
    class MissionListWrapper
    {
        public List<Mission> list;
    }

    [SerializeField]
    private MissionListWrapper mainMisions;
    [SerializeField]
    private List<MissionListWrapper> otherMissions;

    private UnityEvent<string> missionChangeEvent;
    private UnityEvent<string> objectiveGroupChangeEvent;

    protected MissionsManager() { }

    private Dictionary<string, Objective> objectiveNames;
    private HashSet<string> usedObjectivesTracker;
    private Dictionary<int, bool> objectiveStates;

    private List<MissionData> mainMissionsData;
    private List<List<MissionData>> otherMissionsData;

    private ObjectiveGroupData currentMainObjectiveGroup;
    private List<ObjectiveGroupData> currentOtherObjectiveGroups;

    public UnityEvent<string> MissionChangeEvent
    {
        get
        {
            if (missionChangeEvent == null)
            {
                missionChangeEvent = new UnityEvent<string>();
            }
            return missionChangeEvent;
        }
    }

    public UnityEvent<string> ObjectiveGroupChangeEvent
    {
        get
        {
            if (objectiveGroupChangeEvent == null)
            {
                objectiveGroupChangeEvent = new UnityEvent<string>();
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
    }

    private void Start()
    {
        Init();
        EventManager.Instance.AddListener("GameReloaded", OnGameReload);
        //Debug.Log(string.Format("New mission: {0}", missions[missionIndex].label));
        //Debug.Log(string.Format("New objectives group: {0}", missions[missionIndex].groups[groupIndex].label));
    }

    private void Init()
    {
        if (mainMisions.list.Count == 0)
        {
            enabled = false;
            return;
        }
        RegisterObjectives();
        SetObjectives();
        CheckForUnusedObjectives();
        mainMissionsData = CreateMissionDataList(mainMisions.list);
        currentMainObjectiveGroup = mainMissionsData[0].Groups[0];
        otherMissionsData = new List<List<MissionData>>();
        currentOtherObjectiveGroups = new List<ObjectiveGroupData>();
        foreach (MissionListWrapper missions in otherMissions)
        {
            List<MissionData> missionData = CreateMissionDataList(missions.list);
            otherMissionsData.Add(missionData);
            currentOtherObjectiveGroups.Add(missionData[0].Groups[0]);
        }
        ObjectiveGroupChangeEvent.Invoke(currentMainObjectiveGroup.Label);
        MissionChangeEvent.Invoke(currentMainObjectiveGroup.Mission.Label);
    }

    private List<MissionData> CreateMissionDataList(List<Mission> missions)
    {
        List<MissionData> missionsStructs = new List<MissionData>();
        foreach (Mission mission in missions)
        {
            MissionData missionData = new MissionData(mission.label, mission.missionCompleteEvent);
            foreach (ObjectivesGroup group in mission.groups)
            {
                ObjectiveGroupData objectiveGroupData = new ObjectiveGroupData(group.label, group.objectivesGroupCompleteEvent, missionData);
                foreach (string name in group.objectiveNames)
                {
                    Objective objective = objectiveNames[name];
                    objectiveGroupData.Objectives.Add(objective.Id);
                }
                missionData.AddObjectivesGroup(objectiveGroupData);
            }
            for (int i = 0; i < missionData.Groups.Count - 1; i++)
            {
                missionData.Groups[i].NextGroup = missionData.Groups[i + 1];
            }
            missionData.Groups[missionsStructs.Count - 1] = null;
            mainMissionsData.Add(missionData);
        }
        for (int i = 0; i < missionsStructs.Count - 1; i++)
        {
            missionsStructs[i].nextMission = missionsStructs[i + 1];
        }
        missionsStructs[missionsStructs.Count - 1] = null;
        return missionsStructs;
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
        foreach (Mission mission in mainMisions.list)
        {
            foreach (ObjectivesGroup group in mission.groups)
            {
                for (int i = 0; i < group.objectiveNames.Count; i++)
                {
                    string name = group.objectiveNames[i];
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
        objectiveNames.Clear();
        objectiveStates.Clear();
        usedObjectivesTracker.Clear();
        Init();
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
        List<int> completed = GetOtherComptetedGroups(id);
        foreach (int completedGroupIndex in completed)
        {
            _ = ProceedToNextObjectiveGroup(currentOtherObjectiveGroups[completedGroupIndex], out ObjectiveGroupData nextGroup);
            if (nextGroup != null)
            {
                currentOtherObjectiveGroups[completedGroupIndex] = nextGroup;
            }
            else
            {
                currentOtherObjectiveGroups.RemoveAt(completedGroupIndex);
            }
        }
        if (IsMainObjectiveGroupCompleted(id))
        {
            bool missionChanged = ProceedToNextObjectiveGroup(currentMainObjectiveGroup, out ObjectiveGroupData nextGroup);
            if (nextGroup != null)
            {
                currentMainObjectiveGroup = nextGroup;
                ObjectiveGroupChangeEvent.Invoke(nextGroup.Label);
                if(missionChanged)
                {
                    MissionChangeEvent.Invoke(nextGroup.Mission.Label);
                }
            }
            else
            {
                EventManager.Instance.TriggerEvent("Victory");
                enabled = false;
                ObjectiveGroupChangeEvent.Invoke(null);
                MissionChangeEvent.Invoke(null);
            }
        }
    }

    private bool ProceedToNextObjectiveGroup(ObjectiveGroupData currentGroup, out ObjectiveGroupData nextGroup)
    {
        currentGroup.Completed.Raise();
        nextGroup = currentGroup.NextGroup;
        if (nextGroup != null)
        {
            //Proceed to next objective group
            return false;
        }
        else
        {
            currentGroup.Mission.Completed.Raise();
            MissionData nextMission = currentGroup.Mission.nextMission;
            if (nextMission != null)
            {
                //Proceed to next mission
                nextGroup = nextMission.Groups[0];
            }
            else
            {
                nextGroup = null;
            }
            return true;
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

    private bool IsMainObjectiveGroupCompleted(int currentlyCompletedObjectiveId)
    {
        if (currentMainObjectiveGroup.Objectives.Contains(currentlyCompletedObjectiveId))
        {
            if (CheckForGroupCompletion(currentMainObjectiveGroup.Objectives))
            {
                return true;
            }
        }
        return false;
    }

    private List<int> GetOtherComptetedGroups(int currentlyCompletedObjectiveId)
    {
        List<int> completedGroups = new List<int>();
        for (int i = 0; i < currentOtherObjectiveGroups.Count; i++)
        {
            HashSet<int> group = currentOtherObjectiveGroups[i].Objectives;
            if (group.Contains(currentlyCompletedObjectiveId))
            {
                if (CheckForGroupCompletion(group))
                {
                    completedGroups.Add(i);
                }
            }
        }
        return completedGroups;
    }

    private bool CheckForGroupCompletion(HashSet<int> group)
    {
        foreach (int id in group)
        {
            if (!objectiveStates[id])
            {
                return false;
            }
        }
        return true;
    }
}
