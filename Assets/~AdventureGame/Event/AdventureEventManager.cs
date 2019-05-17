using UnityEngine;
using System.Collections;
using System;

namespace AdventureGame
{
    public class AdventureEventManager : MonoBehaviour
    {
        #region External dependencies
        private AdventureLevelChunkFXTransitionManager AdventureLevelChunkFXTransitionManager;
        #endregion

        public void Init()
        {
            this.AdventureLevelChunkFXTransitionManager = GameObject.FindObjectOfType<AdventureLevelChunkFXTransitionManager>();
        }

        public void AD_EVT_OnChunkLevelEnter(LevelChunkTracker NextLevelChunkTracker)
        {
            this.AdventureLevelChunkFXTransitionManager.OnChunkLevelEnter(NextLevelChunkTracker);
        }

        internal void AD_EVT_OnChunkLevelExit(LevelChunkTracker levelChunkTracker)
        {
            this.AdventureLevelChunkFXTransitionManager.OnChunkLevelExit(levelChunkTracker);
        }
    }
}

