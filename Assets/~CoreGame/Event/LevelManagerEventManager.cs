using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class LevelManagerEventManager : MonoBehaviour
    {
        public List<AsyncOperation> CORE_EVT_OnAdventureToPuzzleLevel(LevelZonesID nextLevel)
        {
            CoreGameSingletonInstances.PlayerAdventurePositionManager.OnAdventureToPuzzleLevel();
            return CoreGameSingletonInstances.LevelManager.OnAdventureToPuzzleLevel(nextLevel);
        }
        public List<AsyncOperation> CORE_EVT_OnPuzzleToAdventureLevel(LevelZonesID nextLevel)
        {
            return CoreGameSingletonInstances.LevelManager.OnPuzzleToAdventureLevel(nextLevel);
        }

        public void CORE_EVT_OnLevelChunkLoaded(LevelZoneChunkID levelZoneChunkID)
        {
            LevelChunkType.DestroyAllDestroyOnStartObjects();
            CoreGameSingletonInstances.LevelManager.OnLevelChunkLoaded(levelZoneChunkID);
        }

        public void CORE_EVT_OnChunkLevelEnter(LevelChunkTracker enteredLevelChunkTracker)
        {
            CoreGameSingletonInstances.LevelChunkFXTransitionManager.OnChunkLevelEnter(enteredLevelChunkTracker);
            CoreGameSingletonInstances.LevelManager.OnChunkLevelEnter(enteredLevelChunkTracker.AssociatedLevelChunkType);
        }

        public void CORE_EVT_OnChunkLevelExit(LevelChunkTracker exitedLevelChunkTracker)
        {
            CoreGameSingletonInstances.LevelChunkFXTransitionManager.OnChunkLevelExit(exitedLevelChunkTracker);
        }

    }

}
