using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class LevelAvailabilityTimelineManagerV2 : TimelineNodeManagerV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        protected override LevelAvailabilityManager workflowActionPassedDataStruct => new LevelAvailabilityManager();

        protected override bool isPersisted => false;

        #region //TODO DEBUG TO REMOVE
        public LevelCompletedTimelineAction LevelCompletedTimelineAction;
        public bool sendEvent;
        private void Start()
        {
            this.Init();
        }
        private void Update()
        {
            if (this.sendEvent)
            {
                this.IncrementGraph(this.LevelCompletedTimelineAction);
                this.sendEvent = false;
            }
        }
        #endregion
    }

}
