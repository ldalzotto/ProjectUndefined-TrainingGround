using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour
    {
        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        private GroundEffectsManager GroundEffectsManager;
        private PlayerActionPuzzleEventsManager PlayerActionPuzzleEventsManager;
        #endregion

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.GroundEffectsManager = GameObject.FindObjectOfType<GroundEffectsManager>();
            this.PlayerActionPuzzleEventsManager = GameObject.FindObjectOfType<PlayerActionPuzzleEventsManager>();
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

        public virtual void PZ_EVT_AI_Projectile_Hitted(AiID aiID, int timesInARow)
        {
            if (timesInARow == 1)
            {
                this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectileFirstTime();
            }
            else
            {
                this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectile2InARow();
            }
        }

        public void PZ_EVT_AI_Projectile_NoMoreAffected(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAiAffectedByProjectileEnd();
        }
        #endregion

        #region Attractive Object Events
        internal void PZ_EVT_AttractiveObject_PlayerAction_Start(AttractiveObjectInherentConfigurationData attractiveObjectConfigurationData, Transform playerTransform)
        {
            this.GroundEffectsManager.OnAttractiveObjectActionStart(attractiveObjectConfigurationData, playerTransform);
        }
        internal void PZ_EVT_AttractiveObject_PlayerAction_End()
        {
            this.GroundEffectsManager.OnAttractiveObjectActionEnd();
        }
        #endregion

        #region Projectile throw action events

        public void PZ_EVT_ThrowProjectile_PlayerAction_Start(ThrowProjectileActionStartEvent throwProjectileActionStartEvent)
        {
            GroundEffectsManager.OnThrowProjectileActionStart(throwProjectileActionStartEvent);
        }

        public void PZ_EVT_ThrowProjectile_PlayerAction_End()
        {
            GroundEffectsManager.OnThrowProjectileActionEnd();
        }

        public void PZ_EVT_ThrowProjectileCursor_Positionable()
        {
            GroundEffectsManager.OnThrowProjectileCursorPositionable();
        }
        public void PZ_EVT_ThrowProjectileCursor_NotPositionable()
        {
            GroundEffectsManager.OnThrowProjectileCursorNotPositionable();
        }

        public void PZ_EVT_ThrowProjectileCursor_OnProjectileRange()
        {
            this.PlayerActionPuzzleEventsManager.OnThrowProjectileCursorOnProjectileRange();
            GroundEffectsManager.OnThrowProjectileCursorOnProjectileRange();
        }
        public void PZ_EVT_ThrowProjectileCursor_OutOfProjectileRange()
        {
            this.PlayerActionPuzzleEventsManager.OnThrowProjectileCursorOutOfProjectileRange();
            GroundEffectsManager.OnThrowProjectileCursorOutOfProjectileRange();
        }
        #endregion
        
        public void PZ_EVT_GameOver(LevelZonesID nextZone)
        {
            this.NPCAIManagerContainer.OnGameOver();
            SceneLoadHelper.LoadScene(Coroutiner.Instance, nextZone);
        }

        public void PZ_EVT_LevelCompleted(LevelZonesID nextZone)
        {
            this.NPCAIManagerContainer.OnGameOver();
            //TODO -> not place particles here but in dedicated level completion condition module
            var fxContainerManager = GameObject.FindObjectOfType<FXContainerManager>();
            fxContainerManager.TriggerFX(PrefabContainer.Instance.LevelCompletedParticleEffect);

            //  SceneLoadHelper.LoadScene(Coroutiner.Instance, nextZone);
        }

    }
}
