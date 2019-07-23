using UnityEngine;
using System.Collections;
using System;
using CoreGame;

namespace AdventureGame
{
    public class AdventureEventManager : MonoBehaviour
    {
        #region External dependencies
        private AdventureLevelChunkFXTransitionManager AdventureLevelChunkFXTransitionManager;
        private LevelManager LevelManager;
        #endregion

        public void Init()
        {
            this.AdventureLevelChunkFXTransitionManager = GameObject.FindObjectOfType<AdventureLevelChunkFXTransitionManager>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
        }

        public void AD_EVT_OnChunkLevelEnter(LevelChunkTracker NextLevelChunkTracker)
        {
            this.AdventureLevelChunkFXTransitionManager.OnChunkLevelEnter(NextLevelChunkTracker);
            this.LevelManager.OnChunkLevelEnter(NextLevelChunkTracker.AssociatedLevelChunkType);
        }

        internal void AD_EVT_OnChunkLevelExit(LevelChunkTracker levelChunkTracker)
        {
            this.AdventureLevelChunkFXTransitionManager.OnChunkLevelExit(levelChunkTracker);
        }
    }
}

