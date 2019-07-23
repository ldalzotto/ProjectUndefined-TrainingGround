using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class LevelManagerEventManager : MonoBehaviour
    {
        #region External Dependencies
        private LevelManager LevelManager;
        private PlayerAdventurePositionManager playerAdventurePositionManager;
        private LevelChunkFXTransitionManager LevelChunkFXTransitionManager;
        #endregion
    
        public void Init()
        {
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.playerAdventurePositionManager = GameObject.FindObjectOfType<PlayerAdventurePositionManager>();
            this.LevelChunkFXTransitionManager = GameObject.FindObjectOfType<LevelChunkFXTransitionManager>();
        }

        public List<AsyncOperation> CORE_EVT_OnAdventureToPuzzleLevel(LevelZonesID nextLevel)
        {
            this.playerAdventurePositionManager.OnAdventureToPuzzleLevel();
            return this.LevelManager.OnAdventureToPuzzleLevel(nextLevel);
        }
        public List<AsyncOperation> CORE_EVT_OnPuzzleToAdventureLevel(LevelZonesID nextLevel)
        {
            return this.LevelManager.OnPuzzleToAdventureLevel(nextLevel);
        }

        public void CORE_EVT_OnLevelChunkLoaded(LevelZoneChunkID levelZoneChunkID)
        {
            LevelChunkType.DestroyAllDestroyOnStartObjects();
            this.LevelManager.OnLevelChunkLoaded(levelZoneChunkID);
        }

        public void CORE_EVT_OnChunkLevelEnter(LevelChunkTracker enteredLevelChunkTracker)
        {
            this.LevelChunkFXTransitionManager.OnChunkLevelEnter(enteredLevelChunkTracker);
            this.LevelManager.OnChunkLevelEnter(enteredLevelChunkTracker.AssociatedLevelChunkType);
        }

        public void CORE_EVT_OnChunkLevelExit(LevelChunkTracker exitedLevelChunkTracker)
        {
            this.LevelChunkFXTransitionManager.OnChunkLevelExit(exitedLevelChunkTracker);
        }

    }

}
