using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class LevelUnlockWorkflowAction : TimelineNodeWorkflowAction<LevelAvailabilityManager>
    {
        private LevelZoneChunkID levelZoneChunkToUnlock;

        public LevelUnlockWorkflowAction(LevelZoneChunkID levelZoneChunkToUnlock)
        {
            this.levelZoneChunkToUnlock = levelZoneChunkToUnlock;
        }

        public override void Execute(LevelAvailabilityManager levelAvailabilityManager, TimelineNode<LevelAvailabilityManager> timelineNodeRefence)
        {
            levelAvailabilityManager.UnlockLevel(this.levelZoneChunkToUnlock);
        }
    }

}
