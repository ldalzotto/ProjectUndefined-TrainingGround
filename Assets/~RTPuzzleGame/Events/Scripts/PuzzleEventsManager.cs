using CoreGame;
using LevelManagement;
using RangeObjects;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : GameSingleton<PuzzleEventsManager>
    {
        public PuzzleEventsManager()
        {
            PuzzleLevelTransitionManager = LevelTransitionManager.Get();
            TimelinesEventManager = CoreGameSingletonInstances.TimelinesEventManager;
            LevelManager = LevelManager.Get();
            TutorialManager = CoreGameSingletonInstances.TutorialManager;
        }


        #region External Dependencies

        private LevelManager LevelManager;
        private LevelTransitionManager PuzzleLevelTransitionManager;
        private TimelinesEventManager TimelinesEventManager;
        private IDottedLineRendererManagerEvent IDottedLineRendererManagerEvent = DottedLineRendererManager.Get();
        private GroundEffectsManagerV2 GroundEffectsManagerV2 = GroundEffectsManagerV2.Get();

        private TutorialManager TutorialManager;

        #endregion

        #region Level Transition Events

        public void PZ_EVT_LevelCompleted()
        {
            Debug.Log(MyLog.Format("PZ_EVT_LevelCompleted"));
            TimelinesEventManager.OnScenarioActionExecuted(new LevelCompletedTimelineAction(LevelManager.GetCurrentLevel()));
            //     OnPuzzleToAdventureLevel(LevelMemoryManager.LastAdventureLevel);
        }

        public void PZ_EVT_LevelReseted()
        {
            Debug.Log(MyLog.Format("PZ_EVT_LevelReseted"));
            OnPuzzleToPuzzleLevel(LevelManager.LevelID);
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