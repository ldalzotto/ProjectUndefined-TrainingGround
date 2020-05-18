using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour, IAIAttractiveObjectEventListener
    {
        #region External Dependencies
        private AIManagerContainer NPCAIManagerContainer;
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        private LevelCompletionManager LevelCompletionManager;
        private LevelManager LevelManager;
        private LevelTransitionManager PuzzleLevelTransitionManager;
        private TimelinesEventManager TimelinesEventManager;
        private DottedLineRendererManager DottedLineRendererManager;
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private PlayerActionEventManager PlayerActionEventManager;
        private TutorialManager TutorialManager;
        private LevelMemoryManager LevelMemoryManager;
        private InteractiveObjectSelectionManager InteractiveObjectSelectionManager;
        #endregion

        public void Init()
        {
            this.NPCAIManagerContainer = PuzzleGameSingletonInstances.AIManagerContainer;
            this.PlayerActionPuzzleEventsManager = PuzzleGameSingletonInstances.PlayerActionPuzzleEventsManager;
            this.LevelCompletionManager = PuzzleGameSingletonInstances.LevelCompletionManager;
            this.PuzzleLevelTransitionManager = CoreGameSingletonInstances.LevelTransitionManager;
            this.TimelinesEventManager = CoreGameSingletonInstances.TimelinesEventManager;
            this.LevelManager = CoreGameSingletonInstances.LevelManager;
            this.DottedLineRendererManager = PuzzleGameSingletonInstances.DottedLineRendererManager;
            this.GroundEffectsManagerV2 = PuzzleGameSingletonInstances.GroundEffectsManagerV2;
            this.PlayerActionEventManager = PuzzleGameSingletonInstances.PlayerActionEventManager;
            this.TutorialManager = CoreGameSingletonInstances.TutorialManager;
            this.LevelMemoryManager = CoreGameSingletonInstances.LevelMemoryManager;
            this.InteractiveObjectSelectionManager = PuzzleGameSingletonInstances.InteractiveObjectSelectionManager;
        }

        #region AI related events
        public virtual void PZ_EVT_AI_DestinationReached(AIObjectID aiID)
        {
            this.NPCAIManagerContainer.OnDestinationReached(aiID);
        }
        #endregion

        #region Fear Events
        public void PZ_EVT_AI_FearedStunned_Start(AIObjectID aiID)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedStunned_Start"));
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunned();
        }
        public void PZ_EVT_AI_FearedForced(AIObjectID aiID, float fearTime)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedForced"));
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedForced(fearTime);
        }
        public void PZ_EVT_AI_FearedStunned_Ended(AIObjectID aiID)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedStunned_Ended"));
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunnedEnded();
        }
        #endregion

        #region IAIAttractiveObjectEventListener
        public void AI_AttractedObject_Start(IAttractiveObjectModuleDataRetriever InvolvedAttractiveObjectModuleDataRetriever, AIObjectDataRetriever AIObjectDataRetriever)
        {
            var involvedAIInteractiveObjectDataRetrieval = AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval();

            var ILineVisualFeedbackEvent = involvedAIInteractiveObjectDataRetrieval.GetILineVisualFeedbackEvent();
            if (ILineVisualFeedbackEvent != null)
            {
                ILineVisualFeedbackEvent.CreateLine(DottedLineID.ATTRACTIVE_OBJECT, InvolvedAttractiveObjectModuleDataRetriever.GetModelObjectModule());
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
                ILineVisualFeedbackEvent.DestroyLine();
            }
            
            var IContextMarkVisualFeedbackEvent = AIObjectDataRetriever.GetInteractiveObjectTypeDataRetrieval().GetIContextMarkVisualFeedbackEvent();
            if (IContextMarkVisualFeedbackEvent != null)
            {
                IContextMarkVisualFeedbackEvent.Delete();
            }
        }
        #endregion

        public virtual void PZ_EVT_AI_Projectile_Hitted(AIObjectID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectileFirstTime();
        }

        public void PZ_EVT_AI_Projectile_NoMoreAffected(AIObjectID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAiAffectedByProjectileEnd();
        }

        #region Escape without target zone events
        public void PZ_EVT_AI_EscapeWithoutTarget_Start(AIObjectID aiID)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_EscapeWithoutTarget_Start"));
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnEscapeWithoutTargetStart();
        }
        public void PZ_EVT_AI_EscapeWithoutTarget_End(AIObjectID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnEscapeWithoutTargetEnd();
        }
        #endregion

        #region Projectile throw action events
        public void PZ_EVT_ThrowProjectileCursor_OnProjectileRange()
        {
            this.PlayerActionPuzzleEventsManager.OnThrowProjectileCursorOnProjectileRange();
        }
        public void PZ_EVT_ThrowProjectileCursor_OutOfProjectileRange()
        {
            this.PlayerActionPuzzleEventsManager.OnThrowProjectileCursorOutOfProjectileRange();
        }
        #endregion

        #region Attractive Objects Event
        public void PZ_EVT_AttractiveObject_TpeDestroyed(AttractiveObjectModule attractiveObjectToDestroy)
        {
            this.NPCAIManagerContainer.OnAttractiveObjectDestroyed(attractiveObjectToDestroy);
        }

        public void PZ_EVT_AttractiveObject_OnPlayerActionExecuted(RaycastHit attractiveObjectWorldPositionHit, InteractiveObjectType attractiveObject,
                            PuzzleGameConfigurationManager puzzleGameConfigurationManager)
        {
            AttractiveObjectTypeModuleEventHandling.OnAttractiveObjectActionExecuted(attractiveObjectWorldPositionHit, attractiveObject, puzzleGameConfigurationManager);
        }
        #endregion

        #region RepelableObject Events
        public void PZ_EVT_RepelableObject_OnObjectRepelled(ObjectRepelModule objectRepelType, Vector3 targetWorldPosition)
        {
            ObjectRepelTypeModuleEventHandling.OnObjectRepelRepelled(objectRepelType, targetWorldPosition);
        }
        #endregion

        #region Player action management event
        public void PZ_EVT_OnActionInteractableEnter(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.InteractiveObjectSelectionManager.OnActionInteractableEnter(actionInteractableObjectModule);
        }
        public void PZ_EVT_OnActionInteractableExit(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.InteractiveObjectSelectionManager.OnActionInteractableExit(actionInteractableObjectModule);
        }
        #endregion

        #region GrabObject Events
        public void PZ_EVT_OnGrabObjectEnter(GrabObjectModule grabObjectModule)
        {
            this.InteractiveObjectSelectionManager.OnGrabObjectEnter(grabObjectModule);
        }
        public void PZ_EVT_OnGrabObjectExit(GrabObjectModule grabObjectModule)
        {
            this.InteractiveObjectSelectionManager.OnGrabObjectExit(grabObjectModule);
        }
        #endregion

        #region Player Action Wheel Event
        public void PZ_EVT_OnPlayerActionWheelAwake()
        {
            this.PlayerActionEventManager.OnWheelAwake();
            this.TutorialManager.SendEventToTutorialGraph(TutorialGraphEventType.PUZZLE_ACTION_WHEEL_AWAKE);
        }
        public void PZ_EVT_OnPlayerActionWheelSleep(bool destroyImmediate = false)
        {
            this.PlayerActionEventManager.OnWheelSleep(destroyImmediate);
        }
        public void PZ_EVT_OnPlayerActionWheelRefresh()
        {
            this.PZ_EVT_OnPlayerActionWheelSleep(true);
            this.PZ_EVT_OnPlayerActionWheelAwake();
        }
        public void PZ_EVT_OnPlayerActionWheelNodeSelected()
        {
            this.PlayerActionEventManager.OnCurrentNodeSelected();
        }
        #endregion

        #region Disarm object event
        public void PZ_EVT_DisarmObject_Start(AIObjectID aiID, DisarmObjectModule disarmObjectModule)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnDisarmObjectStart(disarmObjectModule);
            disarmObjectModule.IfNotNull(a => disarmObjectModule.OnDisarmObjectStart());
        }

        public void PZ_EVT_DisarmObject_End(AIObjectID aiID, DisarmObjectModule disarmObjectModule)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnDisarmObjectEnd();
            disarmObjectModule.IfNotNull(a => disarmObjectModule.OnDisarmObjectEnd());
        }
        #endregion

        public void PZ_EVT_GameOver()
        {
            Debug.Log(MyLog.Format("PZ_EVT_GameOver"));
            this.OnPuzzleToAdventureLevel(this.LevelMemoryManager.LastAdventureLevel);
        }

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
            this.NPCAIManagerContainer.OnGameOver();
            this.DottedLineRendererManager.OnLevelExit();
            this.GroundEffectsManagerV2.OnLevelExit();
            this.PuzzleLevelTransitionManager.OnPuzzleToAdventureLevel(levelZonesID);
        }

        private void OnPuzzleToPuzzleLevel(LevelZonesID levelZonesID)
        {
            this.NPCAIManagerContainer.OnGameOver();
            this.DottedLineRendererManager.OnLevelExit();
            this.GroundEffectsManagerV2.OnLevelExit();
            this.PuzzleLevelTransitionManager.OnPuzzleToPuzzleLevel(levelZonesID);
        }

        #region Level Completion Events
        public void PZ_EVT_LevelCompletion_ConditionRecalculationEvaluate()
        {
            this.LevelCompletionManager.ConditionRecalculationEvaluate();
        }
        #endregion

    }
}
