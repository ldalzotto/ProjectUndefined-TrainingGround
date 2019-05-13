using CoreGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class AdventureLevelTransitionManager : AbstractLevelTransitionManager
    {
        #region External Dependencies
        private PointOfInterestManager PointOfInterestManager;

        public override void Init()
        {
            base.Init();
            this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        }

        protected override void OnLevelChange_IMPL()
        {
            PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        }
        #endregion
    }

}