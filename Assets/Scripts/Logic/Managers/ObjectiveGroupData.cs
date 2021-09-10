using System.Collections.Generic;
using UnityEngine.Localization;

public partial class MissionsManager
{
    class ObjectiveGroupData
    {
        private readonly LocalizedString localizedLabel;
        private string label;
        private readonly HashSet<int> objectives;
        private readonly MissionEvent completed;
        private ObjectiveGroupData nextGroup;
        private readonly MissionData mission;
        public event System.EventHandler<string> LabelChanged;

        public ObjectiveGroupData NextGroup { get => nextGroup; set => nextGroup = value; }
        public MissionData Mission { get => mission; }
        public string Label { get => label; }
        public HashSet<int> Objectives { get => objectives; }
        public MissionEvent Completed { get => completed; }

        public ObjectiveGroupData(LocalizedString objectiveGroupLabel, MissionEvent completedEvent, MissionData ownerMission)
        {
            objectives = new HashSet<int>();
            localizedLabel = objectiveGroupLabel;
            localizedLabel.StringChanged += OnLabelChanged;
            label = localizedLabel.GetLocalizedString();
            completed = completedEvent;
            mission = ownerMission;
        }

        private void OnLabelChanged(string value)
        {
            label = value;
            if (LabelChanged != null)
            {
                LabelChanged(this, label);
            }
        }
    }
}
