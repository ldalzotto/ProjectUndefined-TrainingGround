using System;
using Timelines;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    public class LevelUnlockWorkflowActionV2 : TimelineNodeWorkflowActionV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        [SerializeField] private LevelZoneChunkID levelZoneChunkToUnlock;

        public LevelUnlockWorkflowActionV2(LevelZoneChunkID levelZoneChunkToUnlock)
        {
            this.levelZoneChunkToUnlock = levelZoneChunkToUnlock;
        }

        public override void Execute(LevelAvailabilityManager levelAvailabilityManager, TimelineNodeV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID> timelineNodeRefence)
        {
            levelAvailabilityManager.UnlockLevel(this.levelZoneChunkToUnlock);
        }
    }
}