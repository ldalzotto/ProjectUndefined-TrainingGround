using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class LevelAvailabilityTimelineManagerV2 : TimelineNodeManagerV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        protected override LevelAvailabilityManager workflowActionPassedDataStruct => this.LevelAvailabilityManager;

        private LevelAvailabilityManager LevelAvailabilityManager;
        protected override bool isPersisted => true;

        #region //TODO DEBUG TO REMOVE
        public LevelCompletedTimelineAction LevelCompletedTimelineAction;
        public bool sendEvent;
        private void Start()
        {
            this.LevelAvailabilityManager = new LevelAvailabilityManager();
            this.LevelAvailabilityManager.Init();
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
