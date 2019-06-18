using UnityEngine;
using System.Collections;
using NodeGraph;
using CoreGame;
using GameConfigurationID;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelAvailabilityNodeEditorProfile", menuName = "Configuration/CoreGame/LevelConfiguration/LevelAvailability/LevelAvailabilityNodeEditorProfile", order = 1)]
    public class LevelAvailabilityNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineID TimelineID => TimelineID.LEVEL_AVAILABILITY_TIMELINE;
    }
}