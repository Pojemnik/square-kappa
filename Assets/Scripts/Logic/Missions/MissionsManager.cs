using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.Localization;

[RequireComponent(typeof(AudioSource))]
public partial class MissionsManager : Singleton<MissionsManager>
{
    [HideInInspector]
    public bool initFinished { get; private set; }
    [HideInInspector]
    public event System.EventHandler initFinishedEvent;

    [SerializeField]
    private List<LevelMissionGroup> levelsMissions;

    private LevelMissionGroup currentMissionGroup;

    [Header("Debug options")]
    [SerializeField]
    private bool warnAboutUnusedObjectices;
    [SerializeField]
    private bool warnAboutObjectivesUsedMoreThanOnce;
    [SerializeField]
    private bool warnAboutIncorrectObjectiveNamesInMissions;
    [SerializeField]
    private bool informAboutMissionChange;

    protected MissionsManager() { }

    private UnityEvent<string> missionChangeEvent;
    private UnityEvent<string> objectiveGroupChangeEvent;
    private Dictionary<string, Objective> objectiveNames;
    private HashSet<string> usedObjectivesTracker;
    private Dictionary<int, bool> objectiveStates;
    private List<MissionData> mainMissionsData;
    private List<List<MissionData>> otherMissionsData;
    private ObjectiveGroupData currentMainObjectiveGroup;
    private List<ObjectiveGroupData> currentOtherObjectiveGroups;
    private HashSet<int> partialObjectives;

    private AudioSource audioSource;

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
        partialObjectives = new HashSet<int>();
        initFinished = false;
        EventManager.Instance.AddListener("GameStart", Init);
        EventManager.Instance.AddListener("GameQuit", OnGameQuit);
        audioSource = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        if (currentMainObjectiveGroup != null)
        {
            currentMainObjectiveGroup.LabelChanged -= OnMainObjectiveGroupLabelChanged;
            if (currentMainObjectiveGroup.Mission != null)
            {
                currentMainObjectiveGroup.Mission.LabelChanged -= OnMainMissionLabelChanged;
            }
        }
    }

    private void Init()
    {
        int currentLevel = (int)SceneLoadingManager.Instance.CurrentLevel;
        if (currentLevel > levelsMissions.Count || currentLevel < 0)
        {
            enabled = false;
            return;
        }
        currentMissionGroup = levelsMissions[currentLevel];
        if (currentMissionGroup.mainMisions.list.Count == 0)
        {
            enabled = false;
            return;
        }
        RegisterObjectives();
        SetObjectives();
        CheckForUnusedObjectives();
        mainMissionsData = CreateMissionDataList(currentMissionGroup.mainMisions.list);
        currentMainObjectiveGroup = mainMissionsData[0].Groups[0];
        currentMainObjectiveGroup.Mission.LabelChanged += OnMainMissionLabelChanged;
        currentMainObjectiveGroup.LabelChanged += OnMainObjectiveGroupLabelChanged;
        otherMissionsData = new List<List<MissionData>>();
        currentOtherObjectiveGroups = new List<ObjectiveGroupData>();
        foreach (LevelMissionGroup.MissionListWrapper missions in currentMissionGroup.otherMissions)
        {
            List<MissionData> missionData = CreateMissionDataList(missions.list);
            otherMissionsData.Add(missionData);
            currentOtherObjectiveGroups.Add(missionData[0].Groups[0]);
        }
        if (currentMainObjectiveGroup.CompletionMode == ObjectivesGroup.ObjectivesGroupCompletionMode.Some)
        {
            ObjectiveGroupChangeEvent.Invoke(string.Format("{0} ({1}/{2})", currentMainObjectiveGroup.Label, 0, currentMainObjectiveGroup.partsNeededToComplete));
        }
        else
        {
            ObjectiveGroupChangeEvent.Invoke(currentMainObjectiveGroup.Label);
        }
        MissionChangeEvent.Invoke(currentMainObjectiveGroup.Mission.Label);
        initFinished = true;
        initFinishedEvent?.Invoke(this, null);
        Debug.Log("Init finished");
    }

    private void OnMainObjectiveGroupLabelChanged(object sender, string label)
    {
        ObjectiveGroupChangeEvent.Invoke(label);
    }

    private void OnMainMissionLabelChanged(object sender, string label)
    {
        MissionChangeEvent.Invoke(label);
    }

    private List<MissionData> CreateMissionDataList(List<Mission> missions)
    {
        List<MissionData> missionsDataList = new List<MissionData>();
        foreach (Mission mission in missions)
        {
            MissionData missionData = new MissionData(mission.label, mission.missionCompleteEvent);
            foreach (ObjectivesGroup group in mission.groups)
            {
                ObjectiveGroupData objectiveGroupData = new ObjectiveGroupData(group, missionData);
                foreach (string name in group.objectiveNames)
                {
                    if (objectiveNames.ContainsKey(name))
                    {
                        Objective objective = objectiveNames[name];
                        objectiveGroupData.Objectives.Add(objective.Id);
                    }
                    else
                    {
                        Debug.LogErrorFormat("No objective called {0} found in current scenes", name);
                    }
                }
                missionData.AddObjectivesGroup(objectiveGroupData);
            }
            for (int i = 0; i < missionData.Groups.Count - 1; i++)
            {
                missionData.Groups[i].NextGroup = missionData.Groups[i + 1];
            }
            missionData.Groups[missionData.Groups.Count - 1].NextGroup = null;
            missionsDataList.Add(missionData);
        }
        for (int i = 0; i < missionsDataList.Count - 1; i++)
        {
            missionsDataList[i].nextMission = missionsDataList[i + 1];
        }
        missionsDataList[missionsDataList.Count - 1].nextMission = null;
        return missionsDataList;
    }

    private void RegisterObjectives()
    {
        Objective[] objectives = (Objective[])FindObjectsOfType(typeof(Objective));
        foreach (Objective objective in objectives)
        {
            if (objective.name != "")
            {
                objectiveNames.Add(objective.objectiveName, objective);
            }
            else
            {
                Debug.LogWarningFormat("Unnamed mission objective in object {0}. It won't be used", objective.gameObject.name);
            }
            if (objective.isPartial)
            {
                partialObjectives.Add(objective.Id);
            }
        }
    }

    private void SetObjectives()
    {
        SetObjectivesFromList(currentMissionGroup.mainMisions);
        foreach (LevelMissionGroup.MissionListWrapper missionList in currentMissionGroup.otherMissions)
        {
            SetObjectivesFromList(missionList);
        }
    }

    private void SetObjectivesFromList(LevelMissionGroup.MissionListWrapper missionList)
    {
        foreach (Mission mission in missionList.list)
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
                        if (warnAboutIncorrectObjectiveNamesInMissions)
                        {
                            Debug.LogWarningFormat(
                                "No objective named {0} (mission {1}, group {2})", name, mission.label, group.label);
                        }
                    }
                }
            }
        }
    }

    private void SetObjective(string name)
    {
        if (usedObjectivesTracker.Contains(name))
        {
            if (warnAboutObjectivesUsedMoreThanOnce)
            {
                Debug.LogWarningFormat("Objective named {0} used more than once. Is that correct?", name);
            }
            return;
        }
        Objective objective = objectiveNames[name];
        objectiveStates.Add(objective.Id, objective.defaultState);
        objective.Completed.AddListener(OnObjectiveCompleted);
        objective.Uncompleted.AddListener(OnObjectiveUncompleted);
        objective.Disabled.AddListener(OnObjectiveDisabled);
        usedObjectivesTracker.Add(name);
    }

    private void CheckForUnusedObjectives()
    {
        foreach (string name in objectiveNames.Keys)
        {
            if (!usedObjectivesTracker.Contains(name))
            {
                if (warnAboutUnusedObjectices)
                {
                    Debug.LogWarningFormat("Objective named {0} unused", name);
                }
            }
        }
    }

    private void OnGameQuit()
    {
        initFinished = false;
        enabled = true;
        objectiveNames.Clear();
        objectiveStates.Clear();
        usedObjectivesTracker.Clear();
        partialObjectives.Clear();
    }

    public void OnObjectiveDisabled(Objective objective)
    {
        objectiveNames.Remove(objective.objectiveName);
        usedObjectivesTracker.Remove(objective.objectiveName);
        objective.Completed.RemoveListener(OnObjectiveCompleted);
        objective.Uncompleted.RemoveListener(OnObjectiveUncompleted);
        objective.Disabled.RemoveListener(OnObjectiveDisabled);
    }

    private void OnObjectiveCompleted(int id)
    {
        if (!enabled)
        {
            return;
        }
        objectiveStates[id] = true;
        List<int> completed = GetOtherComptetedGroups(id);
        HashSet<ObjectiveGroupData> toRemove = new HashSet<ObjectiveGroupData>();
        foreach (int completedGroupIndex in completed)
        {
            if (informAboutMissionChange)
            {
                Debug.LogFormat("Other missions objective group {0} form mision {1} completed",
                          currentOtherObjectiveGroups[completedGroupIndex].Label,
                          currentOtherObjectiveGroups[completedGroupIndex].Mission.Label);
            }
            _ = ProceedToNextObjectiveGroup(currentOtherObjectiveGroups[completedGroupIndex], out ObjectiveGroupData nextGroup);
            if (nextGroup != null)
            {
                currentOtherObjectiveGroups[completedGroupIndex] = nextGroup;
            }
            else
            {
                toRemove.Add(currentOtherObjectiveGroups[completedGroupIndex]);
            }
        }
        currentOtherObjectiveGroups.RemoveAll((ObjectiveGroupData data) => { return toRemove.Contains(data); });
        if (partialObjectives.Contains(id))
        {
            if (currentMainObjectiveGroup.Objectives.Contains(id))
            {
                //Update label of partial objective group
                if (currentMainObjectiveGroup.CompletionMode == ObjectivesGroup.ObjectivesGroupCompletionMode.Some)
                {
                    int count = GetCounterObjectiveCount(currentMainObjectiveGroup);
                    ObjectiveGroupChangeEvent.Invoke(string.Format("{0} ({1}/{2})", currentMainObjectiveGroup.Label, count, currentMainObjectiveGroup.partsNeededToComplete));
                }
                else
                {
                    Debug.LogErrorFormat("Partial objective in regular objective group {0}. This should never happen", currentMainObjectiveGroup.Label);
                }
            }
        }
        if (IsMainObjectiveGroupCompleted(id))
        {
            audioSource.Play();
            currentMainObjectiveGroup.LabelChanged -= OnMainObjectiveGroupLabelChanged;
            bool missionChanged = ProceedToNextObjectiveGroup(currentMainObjectiveGroup, out ObjectiveGroupData nextGroup);
            if (nextGroup != null)
            {
                if (missionChanged)
                {
                    currentMainObjectiveGroup.Mission.LabelChanged -= OnMainMissionLabelChanged;
                    MissionChangeEvent.Invoke(nextGroup.Mission.Label);
                    nextGroup.Mission.LabelChanged += OnMainMissionLabelChanged;
                }
                currentMainObjectiveGroup = nextGroup;
                currentMainObjectiveGroup.LabelChanged += OnMainObjectiveGroupLabelChanged;
                if (currentMainObjectiveGroup.CompletionMode == ObjectivesGroup.ObjectivesGroupCompletionMode.Some)
                {
                    ObjectiveGroupChangeEvent.Invoke(string.Format("{0} ({1}/{2})", currentMainObjectiveGroup.Label, 0, currentMainObjectiveGroup.partsNeededToComplete));
                }
                else
                {
                    ObjectiveGroupChangeEvent.Invoke(nextGroup.Label);
                }
            }
            else
            {
                currentMainObjectiveGroup.Mission.LabelChanged -= OnMainMissionLabelChanged;
                int currentLevel = (int)SceneLoadingManager.Instance.CurrentLevel;
                if (currentLevel + 1 == levelsMissions.Count)
                {
                    EventManager.Instance.TriggerEvent("Victory");
                    enabled = false;
                    ObjectiveGroupChangeEvent.Invoke(null);
                    MissionChangeEvent.Invoke(null);
                }
                else
                {
                    SceneLoadingManager.Instance.StartLevel(currentLevel + 1, true);
                }
            }
        }
    }

    private int GetCounterObjectiveCount(ObjectiveGroupData group)
    {
        int count = 0;
        foreach (int id in group.Objectives)
        {
            if (objectiveStates[id])
            {
                count++;
            }
        }
        return count;
    }

    private bool ProceedToNextObjectiveGroup(ObjectiveGroupData currentGroup, out ObjectiveGroupData nextGroup)
    {
        currentGroup.Completed?.Raise();
        nextGroup = currentGroup.NextGroup;
        if (nextGroup != null)
        {
            //Proceed to next objective group
            if (informAboutMissionChange)
            {
                Debug.LogFormat("Next objective group {0} form mission {1}", nextGroup.Label, nextGroup.Mission.Label);
            }
            return false;
        }
        else
        {
            MissionData nextMission = currentGroup.Mission.nextMission;
            if (nextMission != null)
            {
                //Proceed to next mission
                nextGroup = nextMission.Groups[0];
                if (informAboutMissionChange)
                {
                    Debug.LogFormat("Next mission {0}", nextGroup.Mission.Label);
                }
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
        if (partialObjectives.Contains(id))
        {
            if (currentMainObjectiveGroup.Objectives.Contains(id))
            {
                //Update label of partial objective group
                if (currentMainObjectiveGroup.CompletionMode == ObjectivesGroup.ObjectivesGroupCompletionMode.Some)
                {
                    int count = GetCounterObjectiveCount(currentMainObjectiveGroup);
                    if (count < 0)
                    {
                        Debug.LogErrorFormat("Objective group's counter value is less than zero {0}. This should never happen", currentMainObjectiveGroup.Label);
                    }
                    ObjectiveGroupChangeEvent.Invoke(string.Format("{0} ({1}/{2})", currentMainObjectiveGroup.Label, count, currentMainObjectiveGroup.partsNeededToComplete));
                }
                else
                {
                    Debug.LogErrorFormat("Partial objective in regular objective group {0}. This should never happen", currentMainObjectiveGroup.Label);
                }
            }
        }
    }

    private bool IsMainObjectiveGroupCompleted(int currentlyCompletedObjectiveId)
    {
        if (currentMainObjectiveGroup.Objectives.Contains(currentlyCompletedObjectiveId))
        {
            if (CheckForGroupCompletion(currentMainObjectiveGroup))
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
            if (currentOtherObjectiveGroups[i].Objectives.Contains(currentlyCompletedObjectiveId))
            {
                if (CheckForGroupCompletion(currentOtherObjectiveGroups[i]))
                {
                    completedGroups.Add(i);
                }
            }
        }
        return completedGroups;
    }

    private bool CheckForGroupCompletion(ObjectiveGroupData group)
    {
        switch (group.CompletionMode)
        {
            case ObjectivesGroup.ObjectivesGroupCompletionMode.All:
                {
                    foreach (int id in group.Objectives)
                    {
                        if (!objectiveStates[id])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            case ObjectivesGroup.ObjectivesGroupCompletionMode.One:
                {
                    foreach (int id in group.Objectives)
                    {
                        if (objectiveStates[id])
                        {
                            return true;
                        }
                    }
                    return false;
                }
            case ObjectivesGroup.ObjectivesGroupCompletionMode.Some:
                int count = GetCounterObjectiveCount(group);
                if (count >= group.partsNeededToComplete)
                {
                    return true;
                }
                return false;
            default:
                throw new System.Exception("Critical error - incorrect ObjectivesGroupCompletionMode");
        }
    }
}
