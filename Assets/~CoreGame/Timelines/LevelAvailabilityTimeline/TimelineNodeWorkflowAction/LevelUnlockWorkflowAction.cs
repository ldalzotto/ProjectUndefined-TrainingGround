using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreGame
{
    [System.Serializable]
    public class LevelUnlockWorkflowActionV2 : TimelineNodeWorkflowActionV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        [SerializeField]
        private LevelZoneChunkID levelZoneChunkToUnlock;

        public LevelUnlockWorkflowActionV2(LevelZoneChunkID levelZoneChunkToUnlock)
        {
            this.levelZoneChunkToUnlock = levelZoneChunkToUnlock;
        }

        public override void Execute(LevelAvailabilityManager levelAvailabilityManager, TimelineNodeV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID> timelineNodeRefence)
        {
            levelAvailabilityManager.UnlockLevel(this.levelZoneChunkToUnlock);
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.levelZoneChunkToUnlock = (LevelZoneChunkID)EditorGUILayout.EnumPopup(this.levelZoneChunkToUnlock);
        }
#endif
    }

}
