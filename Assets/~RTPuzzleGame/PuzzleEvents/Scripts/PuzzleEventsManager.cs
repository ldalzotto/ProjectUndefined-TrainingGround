using UnityEngine;
using CoreGame;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour
    {
        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        private LevelCompletionManager LevelCompletionManager;
        private AbstractLevelTransitionManager PuzzleLevelTransitionManager;
        #endregion

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.PlayerActionPuzzleEventsManager = GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>();
            this.LevelCompletionManager = GameObject.FindObjectOfType<LevelCompletionManager>();
            this.PuzzleLevelTransitionManager = GameObject.FindObjectOfType<AbstractLevelTransitionManager>();
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
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunned();
        }
        public void PZ_EVT_AI_FearedForced(AiID aiID, float fearTime)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedForced(fearTime);
        }
        public void PZ_EVT_AI_FearedStunned_Ended(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunnedEnded();
        }
        public void PZ_EVT_AI_Attracted_Start(AttractiveObjectType attractiveObject, AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIAttractedStart(attractiveObject);
        }
        internal void PZ_EVT_AI_Attracted_End(AttractiveObjectType involvedAttractiveObject, AiID aiID)
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

        public void PZ_EVT_GameOver(LevelZonesID nextZone)
        {
            this.NPCAIManagerContainer.OnGameOver();
            this.PuzzleLevelTransitionManager.OnPuzzleToAdventureLevel(LevelZonesID.SEWER_ADVENTURE);
        }

        public void PZ_EVT_LevelCompleted(LevelZonesID nextZone)
        {
            this.NPCAIManagerContainer.OnGameOver();
            this.PuzzleLevelTransitionManager.OnPuzzleToAdventureLevel(LevelZonesID.SEWER_ADVENTURE);
        }

        #region Level Completion Events
        public void PZ_EVT_LevelCompletion_ConditionRecalculationEvaluate()
        {
            this.LevelCompletionManager.ConditionRecalculationEvaluate();
        }
        #endregion

    }
}
