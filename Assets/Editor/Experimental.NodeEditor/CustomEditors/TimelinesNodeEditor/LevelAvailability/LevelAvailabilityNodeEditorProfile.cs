using UnityEngine;
using System.Collections;
using NodeGraph;
using CoreGame;

namespace Editor_LevelAvailabilityNodeEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelAvailabilityNodeEditorProfile", menuName = "Configuration/CoreGame/LevelConfiguration/LevelAvailability/LevelAvailabilityNodeEditorProfile", order = 1)]
    public class LevelAvailabilityNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineIDs TimelineID => TimelineIDs.LEVEL_AVAILABILITY_TIMELINE;
    }
}