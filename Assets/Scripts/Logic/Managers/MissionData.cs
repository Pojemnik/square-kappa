using System.Collections.Generic;
using UnityEngine.Localization;

public partial class MissionsManager
{
    class MissionData
    {
        private readonly LocalizedString localizedLabel;
        private readonly MissionEvent completed;
        private readonly List<ObjectiveGroupData> groups;
        private string label;

        public MissionData nextMission;
        public List<ObjectiveGroupData> Groups { get => groups; }
        public MissionEvent Completed { get => completed; }
        public string Label { get => label; }
        public event System.EventHandler<string> LabelChanged;

        public MissionData(LocalizedString missionLabel, MissionEvent completedEvent)
        {
            groups = new List<ObjectiveGroupData>();
            localizedLabel = missionLabel;
            label = localizedLabel.GetLocalizedString();
            localizedLabel.StringChanged += OnLabelChanged;
            //label = "xd";
            completed = completedEvent;
        }

        private void OnLabelChanged(string value)
        {
            label = value;
            if (LabelChanged != null)
            {
                LabelChanged(this, label);
            }
        }

        public void AddObjectivesGroup(ObjectiveGroupData group)
        {
            Groups.Add(group);
        }

    }
}
