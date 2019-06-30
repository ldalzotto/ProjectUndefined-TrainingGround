using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace CoreGame
{
    public class LevelAvailabilityTimelineManagerV2 : TimelineNodeManagerV2<LevelAvailabilityManager, LevelAvailabilityTimelineNodeID>
    {
        protected override LevelAvailabilityManager workflowActionPassedDataStruct => this.LevelAvailabilityManager;

        private LevelAvailabilityManager LevelAvailabilityManager;
        protected override bool isPersisted => true;
        protected override TimelineID TimelineID => TimelineID.LEVEL_AVAILABILITY_TIMELINE;

        public override void Init(TimelineInitializerScriptableObject providedTimelineInitializer = null)
        {
            this.LevelAvailabilityManager = GameObject.FindObjectOfType<LevelAvailabilityManager>();
            base.Init();
        }
    }

}
