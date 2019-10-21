using System.Collections.Generic;
using CoreGame;
using UnityEngine;

namespace LevelManagement
{
    public class LevelManagerEventManager : GameSingleton<LevelManagerEventManager>
    {
        public List<AsyncOperation> CORE_EVT_OnAdventureToPuzzleLevel(LevelZonesID nextLevel)
        {
            CoreGameSingletonInstances.PlayerAdventurePositionManager.OnAdventureToPuzzleLevel();
            StartLevelManager.Get().OnStartLevelChange(nextLevel);
            return LevelManager.Get().OnAdventureToPuzzleLevel(nextLevel);
        }

        public List<AsyncOperation> CORE_EVT_OnPuzzleToAdventureLevel(LevelZonesID nextLevel)
        {
            StartLevelManager.Get().OnStartLevelChange(nextLevel);
            return LevelManager.Get().OnPuzzleToAdventureLevel(nextLevel);
        }

        public List<AsyncOperation> CORE_EVT_OnPuzzleToPuzzleLevel(LevelZonesID nextLevel)
        {
            return LevelManager.Get().OnAdventureToPuzzleLevel(nextLevel);
        }

        public List<AsyncOperation> CORE_EVT_OnStartMenuToLevel(LevelZonesID nextLevel)
        {
            StartLevelManager.Get().OnStartLevelChange(nextLevel);
            return LevelManager.Get().OnStartMenuToLevel(nextLevel);
        }

        public void CORE_EVT_OnLevelChunkLoaded(LevelZoneChunkID levelZoneChunkID)
        {
            LevelChunkType.DestroyAllDestroyOnStartObjects();
            LevelManager.Get().OnLevelChunkLoaded(levelZoneChunkID);
        }

        public void CORE_EVT_OnChunkLevelEnter(LevelChunkTracker enteredLevelChunkTracker)
        {
            LevelChunkFXTransitionManager.Get().OnChunkLevelEnter(enteredLevelChunkTracker);
            LevelManager.Get().OnChunkLevelEnter(enteredLevelChunkTracker.AssociatedLevelChunkType);
        }

        public void CORE_EVT_OnChunkLevelExit(LevelChunkTracker exitedLevelChunkTracker)
        {
            LevelChunkFXTransitionManager.Get().OnChunkLevelExit(exitedLevelChunkTracker);
        }
    }
}