using System;
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

        #region Projectile Events

        public void OnAiHittedByProjectile(AiID aiID, int timesInARow)
        {
            if(timesInARow == 1)
            {
                this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectileFirstTime();
            } else
            {
                this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectile2InARow();
            }
        }

        #endregion

        #region Fear Events
        public void OnAIFearedStunned(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunned();
        }
        public void OnAIFearedStunnedEnded(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnAIFearedStunnedEnded();
        }
        #endregion

        #region Attractive Object Events
        internal void OnAttractiveObjectActionStart(AttractiveObjectInherentConfigurationData attractiveObjectConfigurationData, Transform playerTransform)
        {
            this.GroundEffectsManager.OnAttractiveObjectActionStart(attractiveObjectConfigurationData, playerTransform);
        }
        internal void OnAttractiveObjectActionEnd()
        {
            this.GroundEffectsManager.OnAttractiveObjectActionEnd();
        }
        #endregion

        #region Projectile throw action events

        public void OnThrowProjectileActionStart(ThrowProjectileActionStartEvent throwProjectileActionStartEvent)
        {
            GroundEffectsManager.OnThrowProjectileActionStart(throwProjectileActionStartEvent);
        }

        public void OnThrowProjectileCursorAvailable()
        {
            GroundEffectsManager.OnThrowProjectileCursorAvailable();
        }
        public void OnThrowProjectileCursorNotAvailable()
        {
            GroundEffectsManager.OnThrowProjectileCursorNotAvailable();
        }

        public void OnThrowProjectileCursorOnProjectileRange()
        {
            this.PlayerActionPuzzleEventsManager.OnThrowProjectileCursorOnProjectileRange();
            GroundEffectsManager.OnThrowProjectileCursorOnProjectileRange();
        }
        public void OnThrowProjectileCursorOutOfProjectileRange()
        {
            this.PlayerActionPuzzleEventsManager.OnThrowProjectileCursorOutOfProjectileRange();
            GroundEffectsManager.OnThrowProjectileCursorOutOfProjectileRange();
        }

        public void OnProjectileThrowedEvent()
        {
            GroundEffectsManager.OnProjectileThrowedEvent();
        }

        #endregion

        public void OnGameOver(LevelZonesID nextZone)
        {
            this.NPCAIManagerContainer.OnGameOver();
            SceneLoadHelper.LoadScene(Coroutiner.Instance, nextZone);
        }

        public void OnLevelCompleted(LevelZonesID nextZone)
        {
            this.NPCAIManagerContainer.OnGameOver();
            //TODO -> not place particles here but in dedicated level completion condition module
            var fxContainerManager = GameObject.FindObjectOfType<FXContainerManager>();
            fxContainerManager.TriggerFX(PrefabContainer.Instance.LevelCompletedParticleEffect);
            
          //  SceneLoadHelper.LoadScene(Coroutiner.Instance, nextZone);
        }

    }
}
