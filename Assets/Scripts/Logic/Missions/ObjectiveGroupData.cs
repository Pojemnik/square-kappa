using System.Collections.Generic;
using UnityEngine.Localization;

public partial class MissionsManager
{
    private class ObjectiveGroupData
    {
        private readonly LocalizedString localizedLabel;
        private ObjectiveGroupData nextGroup;
        public event System.EventHandler<string> LabelChanged;

        public ObjectiveGroupData NextGroup { get => nextGroup; set => nextGroup = value; }
        public MissionData Mission { get; private set; }
        public string Label { get; private set; }
        public HashSet<int> Objectives { get; private set; }
        public MissionEvent Completed { get; private set; }
        public ObjectivesGroup.ObjectivesGroupCompletionMode CompletionMode { get; private set; }
        public int partsNeededToComplete { get; private set; }

        public ObjectiveGroupData(ObjectivesGroup group, MissionData ownerMission)
        {
            Objectives = new HashSet<int>();
            localizedLabel = group.label;
            localizedLabel.StringChanged += OnLabelChanged;
            Label = localizedLabel.GetLocalizedString();
            Completed = group.objectivesGroupCompleteEvent;
            Mission = ownerMission;
            CompletionMode = group.completionMode;
            partsNeededToComplete = group.objectivesNeededToComplete;
        }

        private void OnLabelChanged(string value)
        {
            Label = value;
            if (LabelChanged != null)
            {
                LabelChanged(this, Label);
            }
        }
    }
}
