using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class LevelAvailabilityTimelineManagerV2 : TimelineNodeManagerV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        protected override LevelAvailabilityManager workflowActionPassedDataStruct => this.LevelAvailabilityManager;

        private LevelAvailabilityManager LevelAvailabilityManager;
        protected override bool isPersisted => true;
        protected override TimelineIDs TimelineID => TimelineIDs.LEVEL_AVAILABILITY_TIMELINE;

        public override void Init()
        {
            this.LevelAvailabilityManager = GameObject.FindObjectOfType<LevelAvailabilityManager>();
            base.Init();
        }
    }

}
