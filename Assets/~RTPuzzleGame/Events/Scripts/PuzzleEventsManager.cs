using System;
using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour
    {
        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        private LevelCompletionManager LevelCompletionManager;
        private LevelManager LevelManager;
        private LevelTransitionManager PuzzleLevelTransitionManager;
        private TimelinesEventManager TimelinesEventManager;
        private DottedLineRendererManager DottedLineRendererManager;
        private GroundEffectsManagerV2 GroundEffectsManagerV2;
        private PlayerActionEventManager PlayerActionEventManager;
        private TutorialManager TutorialManager;
        #endregion

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.PlayerActionPuzzleEventsManager = GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>();
            this.LevelCompletionManager = GameObject.FindObjectOfType<LevelCompletionManager>();
            this.PuzzleLevelTransitionManager = GameObject.FindObjectOfType<LevelTransitionManager>();
            this.TimelinesEventManager = GameObject.FindObjectOfType<TimelinesEventManager>();
            this.LevelManager = GameObject.FindObjectOfType<LevelManager>();
            this.DottedLineRendererManager = GameObject.FindObjectOfType<DottedLineRendererManager>();
            this.GroundEffectsManagerV2 = GameObject.FindObjectOfType<GroundEffectsManagerV2>();
            this.PlayerActionEventManager = GameObject.FindObjectOfType<PlayerActionEventManager>();
            this.TutorialManager = GameObject.FindObjectOfType<TutorialManager>();
        }

        #region AI related events
        public virtual void PZ_EVT_AI_DestinationReached(AiID aiID)
        {
            this.NPCAIManagerContainer.OnDestinationReached(aiID);
        }
        #endregion

        #region Fear Events
        public void PZ_EVT_AI_FearedStunned_Start(AiID aiID)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedStunned_Start"));
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunned();
        }
        public void PZ_EVT_AI_FearedForced(AiID aiID, float fearTime)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedForced"));
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedForced(fearTime);
        }
        public void PZ_EVT_AI_FearedStunned_Ended(AiID aiID)
        {
            Debug.Log(MyLog.Format("PZ_EVT_AI_FearedStunned_Ended"));
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunnedEnded();
        }
        public void PZ_EVT_AI_Attracted_Start(AttractiveObjectModule attractiveObject, AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIAttractedStart(attractiveObject);
        }
        internal void PZ_EVT_AI_Attracted_End(AttractiveObjectModule involvedAttractiveObject, AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIAttractedEnd();
        }

        public virtual void PZ_EVT_AI_Projectile_Hitted(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectileFirstTime();
        }

        public void PZ_EVT_AI_Projectile_NoMoreAffected(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAiAffectedByProjectileEnd();
        }

        public void PZ_EVT_AI_DisarmObject_Start(AiID aiID, DisarmObjectModule disarmObjectModule)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnDisarmObjectStart(disarmObjectModule);
        }

        public void PZ_EVT_AI_DisarmObject_End(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnDisarmObjectEnd();
        }
        #endregion

        #region Escape without target zone events
        public void PZ_EVT_AI_EscapeWithoutTarget_Start(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnEscapeWithoutTargetStart();
        }
        public void PZ_EVT_AI_EscapeWithoutTarget_End(AiID aiID)
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
                            PuzzleGameConfigurationManager puzzleGameConfigurationManager, AttractiveObjectsInstanciatedParent AttractiveObjectsInstanciatedParent)
        {
            AttractiveObjectTypeModuleEventHandling.OnAttractiveObjectActionExecuted(attractiveObjectWorldPositionHit, attractiveObject, puzzleGameConfigurationManager, AttractiveObjectsInstanciatedParent);
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
            this.PlayerActionPuzzleEventsManager.OnActionInteractableEnter(actionInteractableObjectModule);
        }
        public void PZ_EVT_OnActionInteractableExit(ActionInteractableObjectModule actionInteractableObjectModule)
        {
            this.PlayerActionPuzzleEventsManager.OnActionInteractableExit(actionInteractableObjectModule);
        }
        #endregion

        #region Player Action Wheel Event
        public void PZ_EVT_OnPlayerActionWheelAwake()
        {
            this.PlayerActionEventManager.OnWheelAwake();
            this.TutorialManager.SendEventToTutorialGraph(TutorialGraphEventType.PUZZLE_ACTION_WHEEL_AWAKE);
        }
        public void PZ_EVT_OnPlayerActionWheelSleep()
        {
            this.PlayerActionEventManager.OnWheelSleep();
        }
        public void PZ_EVT_OnPlayerActionWheelNodeSelected()
        {
            this.PlayerActionEventManager.OnCurrentNodeSelected();
        }
        #endregion

        public void PZ_EVT_GameOver()
        {
            Debug.Log(MyLog.Format("PZ_EVT_GameOver"));
            this.OnPuzzleToAdventureLevel(LevelZonesID.SEWER_ADVENTURE);
        }

        public void PZ_EVT_LevelCompleted()
        {
            Debug.Log(MyLog.Format("PZ_EVT_LevelCompleted"));
            this.TimelinesEventManager.OnScenarioActionExecuted(new LevelCompletedTimelineAction(this.LevelManager.GetCurrentLevel()));
            this.OnPuzzleToAdventureLevel(LevelZonesID.SEWER_ADVENTURE);
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
