using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour, IGameOverManagerEventListener
    {
        #region External Dependencies
        private LevelManager LevelManager;
        private LevelTransitionManager PuzzleLevelTransitionManager;
        private TimelinesEventManager TimelinesEventManager;
        private IDottedLineRendererManagerEvent IDottedLineRendererManagerEvent;
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private PlayerActionEventManager PlayerActionEventManager;

        private IPlayerActionManagerEvent IPlayerActionManagerEvent;
        private IPlayerActionManagerDataRetrieval IPlayerActionManagerDataRetrieval;

        private TutorialManager TutorialManager;
        private LevelMemoryManager LevelMemoryManager;
        private IInteractiveObjectSelectionEvent IInteractiveObjectSelectionEvent;
        #endregion

        public void Init()
        {
            this.PuzzleLevelTransitionManager = CoreGameSingletonInstances.LevelTransitionManager;
            this.TimelinesEventManager = CoreGameSingletonInstances.TimelinesEventManager;
            this.LevelManager = CoreGameSingletonInstances.LevelManager;
            this.IDottedLineRendererManagerEvent = PuzzleGameSingletonInstances.DottedLineRendererManager;
            this.GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
            this.PlayerActionEventManager = PuzzleGameSingletonInstances.PlayerActionEventManager;
            this.IPlayerActionManagerEvent = PuzzleGameSingletonInstances.PlayerActionManager;
            this.IPlayerActionManagerDataRetrieval = PuzzleGameSingletonInstances.PlayerActionManager;
            this.TutorialManager = CoreGameSingletonInstances.TutorialManager;
            this.LevelMemoryManager = CoreGameSingletonInstances.LevelMemoryManager;
            this.IInteractiveObjectSelectionEvent = PuzzleGameSingletonInstances.InteractiveObjectSelectionManager;
        }

        #region IActionInteractableObjectModuleEventListener
        public void PZ_EVT_OnActionInteractableEnter(ISelectableModule actionInteractableObjectModule)
        {
            this.IInteractiveObjectSelectionEvent.OnSelectableEnter(actionInteractableObjectModule);
        }
        public void PZ_EVT_OnActionInteractableExit(ISelectableModule actionInteractableObjectModule)
        {
            this.IInteractiveObjectSelectionEvent.OnSelectableExit(actionInteractableObjectModule);
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
            this.IPlayerActionManagerEvent.OnSelectionWheelAwake();
            this.TutorialManager.SendEventToTutorialGraph(TutorialGraphEventType.PUZZLE_ACTION_WHEEL_AWAKE);
        }
        public void PZ_EVT_OnPlayerActionWheelSleep(bool destroyImmediate = false)
        {
            this.IPlayerActionManagerEvent.OnSelectionWheelSleep(destroyImmediate);
        }

        public void PZ_EVT_OnPlayerActionWheelNodeSelected()
        {
            var selectedAction = this.IPlayerActionManagerDataRetrieval.GetCurrentSelectedAction();
            if (selectedAction.CanBeExecuted())
            {
                this.PZ_EVT_OnPlayerActionWheelSleep(false);
                this.IPlayerActionManagerEvent.ExecuteAction(selectedAction);
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
