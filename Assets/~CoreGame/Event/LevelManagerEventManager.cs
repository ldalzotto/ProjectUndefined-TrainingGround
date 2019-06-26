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
        #endregion

        public void Init()
        {
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.playerAdventurePositionManager = GameObject.FindObjectOfType<PlayerAdventurePositionManager>();
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
            foreach (var levelChunkType in GameObject.FindObjectsOfType<LevelChunkType>())
            {
                LevelChunkType.DestroyAllDestroyOnStartObjects();
            }
        }

    }

}
