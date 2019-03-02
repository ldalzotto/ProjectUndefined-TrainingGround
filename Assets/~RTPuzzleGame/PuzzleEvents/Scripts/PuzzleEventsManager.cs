using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleEventsManager : MonoBehaviour
    {
        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        private GroundEffectsManager GroundEffectsManager;
        #endregion

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
            this.GroundEffectsManager = GameObject.FindObjectOfType<GroundEffectsManager>();
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
    }
}
