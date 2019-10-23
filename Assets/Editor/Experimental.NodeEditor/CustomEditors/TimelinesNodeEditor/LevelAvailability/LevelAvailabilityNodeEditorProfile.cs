using System;
using Timelines;
using UnityEngine;

namespace Editor_LevelAvailabilityNodeEditor
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelAvailabilityNodeEditorProfile", menuName = "Configuration/CoreGame/LevelConfiguration/LevelAvailability/LevelAvailabilityNodeEditorProfile", order = 1)]
    public class LevelAvailabilityNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineID TimelineID => TimelineID.LEVEL_AVAILABILITY_TIMELINE;
    }
}