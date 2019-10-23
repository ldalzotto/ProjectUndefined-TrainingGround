using System;
using Timelines;
using UnityEngine;

namespace LevelManagement
{
    [Serializable]
    public class LevelUnlockWorkflowActionV2 : TimelineNodeWorkflowActionV2<LevelAvailabilityTimelineNodeID>
    {
        [NonSerialized] private LevelAvailabilityManager LevelAvailabilityManager = LevelAvailabilityManager.Get();
        [SerializeField] private LevelZoneChunkID levelZoneChunkToUnlock;

        public override void Execute(TimelineNodeV2<LevelAvailabilityTimelineNodeID> timelineNodeRefence)
        {
            LevelAvailabilityManager.Get().UnlockLevel(this.levelZoneChunkToUnlock);
        }
    }
}