using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : GameSingleton<PuzzleEventsManager>, IGameOverManagerEventListener
    {
        #region External Dependencies
        private LevelManager LevelManager;
        private LevelTransitionManager PuzzleLevelTransitionManager;
        private TimelinesEventManager TimelinesEventManager;
        private IDottedLineRendererManagerEvent IDottedLineRendererManagerEvent = DottedLineRendererManager.Get();
        private GroundEffectsManagerV2 GroundEffectsManagerV2 = GroundEffectsManagerV2.Get();

        private TutorialManager TutorialManager;
        private LevelMemoryManager LevelMemoryManager;
        private InteractiveObjectSelectionManager InteractiveObjectSelectionManager = InteractiveObjectSelectionManager.Get();
        #endregion

        public PuzzleEventsManager()
        {
            this.PuzzleLevelTransitionManager = CoreGameSingletonInstances.LevelTransitionManager;
            this.TimelinesEventManager = CoreGameSingletonInstances.TimelinesEventManager;
            this.LevelManager = CoreGameSingletonInstances.LevelManager;
            this.TutorialManager = CoreGameSingletonInstances.TutorialManager;
            this.LevelMemoryManager = CoreGameSingletonInstances.LevelMemoryManager;
        }

        #region IActionInteractableObjectModuleEventListener
        public void PZ_EVT_OnActionInteractableEnter(ISelectableModule actionInteractableObjectModule)
        {
            this.InteractiveObjectSelectionManager.OnSelectableEnter(actionInteractableObjectModule);
        }
        public void PZ_EVT_OnActionInteractableExit(ISelectableModule actionInteractableObjectModule)
        {
            this.InteractiveObjectSelectionManager.OnSelectableExit(actionInteractableObjectModule);
        }
        #endregion

        #region IPlayerActionManagerEventListener
        public void PZ_EVT_OnPlayerActionWheelRefresh()
        {
            this.PZ_EVT_OnPlayerActionWheelSleep(true);
            this.PZ_EVT_OnPlayerActionWheelAwake();
        }
        #endregion

        #region Player Action Wheel Event
        public void PZ_EVT_OnPlayerActionWheelAwake()
        {
            PlayerActionManager.Get().OnSelectionWheelAwake();
            this.TutorialManager.SendEventToTutorialGraph(TutorialGraphEventType.PUZZLE_ACTION_WHEEL_AWAKE);
        }
        public void PZ_EVT_OnPlayerActionWheelSleep(bool destroyImmediate = false)
        {
            PlayerActionManager.Get().OnSelectionWheelSleep(destroyImmediate);
        }

        public void PZ_EVT_OnPlayerActionWheelNodeSelected()
        {
            var selectedAction = PlayerActionManager.Get().GetCurrentSelectedAction();
            if (selectedAction.CanBeExecuted())
            {
                this.PZ_EVT_OnPlayerActionWheelSleep(false);
                PlayerActionManager.Get().ExecuteAction(selectedAction);
            }
        }
        #endregion

        #region IGameOverManagerEventListener
        public void PZ_EVT_GameOver()
        {
            Debug.Log(MyLog.Format("PZ_EVT_GameOver"));
            this.OnPuzzleToAdventureLevel(this.LevelMemoryManager.LastAdventureLevel);
        }
        #endregion

        #region Level Transition Events
        public void PZ_EVT_LevelCompleted()
        {
            Debug.Log(MyLog.Format("PZ_EVT_LevelCompleted"));
            this.TimelinesEventManager.OnScenarioActionExecuted(new LevelCompletedTimelineAction(this.LevelManager.GetCurrentLevel()));
            this.OnPuzzleToAdventureLevel(this.LevelMemoryManager.LastAdventureLevel);
        }

        public void PZ_EVT_LevelReseted()
        {
            Debug.Log(MyLog.Format("PZ_EVT_LevelReseted"));
            this.OnPuzzleToPuzzleLevel(this.LevelManager.LevelID);
        }

        private void OnPuzzleToAdventureLevel(LevelZonesID levelZonesID)
        {
            this.IDottedLineRendererManagerEvent.OnLevelExit();
            this.GroundEffectsManagerV2.OnLevelExit();
            this.PuzzleLevelTransitionManager.OnPuzzleToAdventureLevel(levelZonesID);
        }

        private void OnPuzzleToPuzzleLevel(LevelZonesID levelZonesID)
        {
            this.IDottedLineRendererManagerEvent.OnLevelExit();
            this.GroundEffectsManagerV2.OnLevelExit();
            this.PuzzleLevelTransitionManager.OnPuzzleToPuzzleLevel(levelZonesID);
        }
        #endregion

    }
}
