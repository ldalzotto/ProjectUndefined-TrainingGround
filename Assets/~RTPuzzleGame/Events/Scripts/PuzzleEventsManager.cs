using CoreGame;
using GameConfigurationID;
using RangeObjects;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : GameSingleton<PuzzleEventsManager>
    {
        public PuzzleEventsManager()
        {
            PuzzleLevelTransitionManager = CoreGameSingletonInstances.LevelTransitionManager;
            TimelinesEventManager = CoreGameSingletonInstances.TimelinesEventManager;
            LevelManager = CoreGameSingletonInstances.LevelManager;
            TutorialManager = CoreGameSingletonInstances.TutorialManager;
            LevelMemoryManager = CoreGameSingletonInstances.LevelMemoryManager;
        }


        #region External Dependencies

        private LevelManager LevelManager;
        private LevelTransitionManager PuzzleLevelTransitionManager;
        private TimelinesEventManager TimelinesEventManager;
        private IDottedLineRendererManagerEvent IDottedLineRendererManagerEvent = DottedLineRendererManager.Get();
        private GroundEffectsManagerV2 GroundEffectsManagerV2 = GroundEffectsManagerV2.Get();

        private TutorialManager TutorialManager;
        private LevelMemoryManager LevelMemoryManager;

        #endregion

        #region Level Transition Events

        public void PZ_EVT_LevelCompleted()
        {
            Debug.Log(MyLog.Format("PZ_EVT_LevelCompleted"));
            TimelinesEventManager.OnScenarioActionExecuted(new LevelCompletedTimelineAction(LevelManager.GetCurrentLevel()));
            OnPuzzleToAdventureLevel(LevelMemoryManager.LastAdventureLevel);
        }

        public void PZ_EVT_LevelReseted()
        {
            Debug.Log(MyLog.Format("PZ_EVT_LevelReseted"));
            OnPuzzleToPuzzleLevel(LevelManager.LevelID);
        }

        private void OnPuzzleToAdventureLevel(LevelZonesID levelZonesID)
        {
            IDottedLineRendererManagerEvent.OnLevelExit();
            GroundEffectsManagerV2.OnLevelExit();
            PuzzleLevelTransitionManager.OnPuzzleToAdventureLevel(levelZonesID);
        }

        private void OnPuzzleToPuzzleLevel(LevelZonesID levelZonesID)
        {
            IDottedLineRendererManagerEvent.OnLevelExit();
            GroundEffectsManagerV2.OnLevelExit();
            PuzzleLevelTransitionManager.OnPuzzleToPuzzleLevel(levelZonesID);
        }

        #endregion
    }
}