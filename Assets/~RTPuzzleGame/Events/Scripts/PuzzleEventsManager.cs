using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour, IAIAttractiveObjectEventListener,
                                ILaunchProjectileAIEventListener, IAgentEscapeEventListener, IGameOverManagerEventListener,
                                ILevelCompletionManagerEventListener, IActionInteractableObjectModuleEventListener, IPlayerActionManagerEventListener
    {
        #region External Dependencies
        private InteractiveObjectContainer InteractiveObjectContainer;
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
            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
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

        #region Fear Events
        public void PZ_EVT_AI_FearedStunned_Start(AIObjectDataRetriever AIObjectDataRetriever)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedStunned_Start"));
            AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new FearedStartAIBehaviorEvent(
                eventProcessedCallback: () =>
                {
                    AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval().GetInteractiveObjectCutsceneControllerModule()
                        .IfNotNull(InteractiveObjectCutsceneControllerModule => InteractiveObjectCutsceneControllerModule.InteractiveObjectCutsceneController.Play(AnimationID.FEAR, 0f, false));
                }
            ));

        }

        public void PZ_EVT_AI_FearedForced(AIObjectDataRetriever AIObjectDataRetriever, float fearTime)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedForced"));
            AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new FearedForcedAIBehaviorEvent(fearTime));
        }

        public void PZ_EVT_AI_FearedStunned_Ended(AIObjectDataRetriever AIObjectDataRetriever)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedStunned_Ended"));
            AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new FearedStartAIBehaviorEvent(
                eventProcessedCallback: () =>
                {
                    AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval().GetInteractiveObjectCutsceneControllerModule()
                        .IfNotNull(InteractiveObjectCutsceneControllerModule => InteractiveObjectCutsceneControllerModule.InteractiveObjectCutsceneController.Play(AnimationID.POSE_OVERRIVE_LISTENING, 0f, false));
                }
            ));
        }
        #endregion

        #region IAIAttractiveObjectEventListener
        public void AI_AttractedObject_Start(IAttractiveObjectModuleDataRetriever InvolvedAttractiveObjectModuleDataRetriever, AIObjectDataRetriever AIObjectDataRetriever)
        {
            var involvedAIInteractiveObjectDataRetrieval = AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval();

            var ILineVisualFeedbackEvent = involvedAIInteractiveObjectDataRetrieval.GetILineVisualFeedbackEvent();
            if (ILineVisualFeedbackEvent != null)
            {
                ILineVisualFeedbackEvent.CreateLineFollowingModelObject(DottedLineID.ATTRACTIVE_OBJECT, InvolvedAttractiveObjectModuleDataRetriever.GetModelObjectModule(), (MonoBehaviour)AIObjectDataRetriever);
            }

            var IContextMarkVisualFeedbackEvent = involvedAIInteractiveObjectDataRetrieval.GetIContextMarkVisualFeedbackEvent();
            if (IContextMarkVisualFeedbackEvent != null)
            {
                IContextMarkVisualFeedbackEvent.CreateGenericMark(InvolvedAttractiveObjectModuleDataRetriever.GetModelObjectModule());
            }
        }

        public void AI_AttractedObject_End(IAttractiveObjectModuleDataRetriever InvolvedAttractiveObjectModuleDataRetriever, AIObjectDataRetriever AIObjectDataRetriever)
        {
            var involvedAIInteractiveObjectDataRetrieval = AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval();
            var ILineVisualFeedbackEvent = involvedAIInteractiveObjectDataRetrieval.GetILineVisualFeedbackEvent();
            if (ILineVisualFeedbackEvent != null)
            {
                ILineVisualFeedbackEvent.DestroyLine((MonoBehaviour)AIObjectDataRetriever);
            }

            var IContextMarkVisualFeedbackEvent = AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval().GetIContextMarkVisualFeedbackEvent();
            if (IContextMarkVisualFeedbackEvent != null)
            {
                IContextMarkVisualFeedbackEvent.Delete();
            }
        }
        #endregion

        #region ILaunchProjectileAIEventListener
        public virtual void PZ_EVT_AI_Projectile_Hitted(AIObjectDataRetriever AIObjectDataRetriever)
        {
            this.PZ_EVT_AI_EscapingStart(AIObjectDataRetriever, AnimationID.HITTED_BY_PROJECTILE_1ST);
        }

        public void PZ_EVT_AI_Projectile_NoMoreAffected(AIObjectDataRetriever AIObjectDataRetriever)
        {
            this.PZ_EVT_AI_NoMoreEscaping(AIObjectDataRetriever);
        }
        #endregion

        #region IAgentEscapeEvent
        public void PZ_EVT_AI_EscapingStart(AIObjectDataRetriever AIObjectDataRetriever, AnimationID playedAnimation)
        {
            var interactiveObjectTypeDataRetrieval = AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval();
            var IContextMarkVisualFeedbackEvent = interactiveObjectTypeDataRetrieval.GetIContextMarkVisualFeedbackEvent();
            if (IContextMarkVisualFeedbackEvent != null)
            {
                IContextMarkVisualFeedbackEvent.CreateDoubleExclamationMark();
            }

            var InteractiveObjectCutsceneControllerModule = interactiveObjectTypeDataRetrieval.GetInteractiveObjectCutsceneControllerModule();
            if (InteractiveObjectCutsceneControllerModule != null)
            {
                InteractiveObjectCutsceneControllerModule.InteractiveObjectCutsceneController.Play(playedAnimation, 0f, false);
            }
        }

        public void PZ_EVT_AI_NoMoreEscaping(AIObjectDataRetriever AIObjectDataRetriever)
        {
            var interactiveObjectTypeDataRetrieval = AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval();
            var IContextMarkVisualFeedbackEvent = interactiveObjectTypeDataRetrieval.GetIContextMarkVisualFeedbackEvent();
            if (IContextMarkVisualFeedbackEvent != null)
            {
                IContextMarkVisualFeedbackEvent.Delete();
            }
        }
        #endregion

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
            this.InteractiveObjectContainer.OnGameOver();
            this.IDottedLineRendererManagerEvent.OnLevelExit();
            this.GroundEffectsManagerV2.OnLevelExit();
            this.PuzzleLevelTransitionManager.OnPuzzleToAdventureLevel(levelZonesID);
        }

        private void OnPuzzleToPuzzleLevel(LevelZonesID levelZonesID)
        {
            this.InteractiveObjectContainer.OnGameOver();
            this.IDottedLineRendererManagerEvent.OnLevelExit();
            this.GroundEffectsManagerV2.OnLevelExit();
            this.PuzzleLevelTransitionManager.OnPuzzleToPuzzleLevel(levelZonesID);
        }
        #endregion

    }
}
