using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class LevelAvailabilityTimelineManager : ATimelineNodeManager<LevelAvailabilityManager>
    {
        protected override LevelAvailabilityManager workflowActionPassedDataStruct => this.LevelAvailabilityManager;

        #region External Dependencies
        private LevelAvailabilityManager LevelAvailabilityManager;
        #endregion

        public override void Init()
        {
            base.Init();
            this.LevelAvailabilityManager = GameObject.FindObjectOfType<LevelAvailabilityManager>();
        }
    }

}
