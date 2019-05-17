using UnityEngine;
using System.Collections;

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

        public void AD_EVT_OnChunkLevelSwitch(LevelChunkTracker NextLevelChunkTracker)
        {
            this.AdventureLevelChunkFXTransitionManager.OnChunkLevelSwitch(NextLevelChunkTracker);
        }
    }
}

