using UnityEngine;
using System.Collections;

namespace CoreGame
{
    [System.Serializable]
    public class LevelUnlockWorkflowAction : TimelineNodeWorkflowAction<LevelAvailabilityManager>
    {
        [SerializeField]
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
