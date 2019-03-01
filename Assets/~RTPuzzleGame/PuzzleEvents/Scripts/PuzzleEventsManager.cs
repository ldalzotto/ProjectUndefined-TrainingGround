using System.Collections.Generic;
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

        public void OnThrowProjectileActionStart(ThrowProjectileActionStartEvent throwProjectileActionStartEvent)
        {
            GroundEffectsManager.OnThrowProjectileActionStart(throwProjectileActionStartEvent);
        }

        public void OnProjectileThrowedEvent()
        {
            GroundEffectsManager.OnProjectileThrowedEvent();
        }

        public void OnGameOver(LevelZonesID nextZone)
        {
            this.NPCAIManagerContainer.OnGameOver();
            SceneLoadHelper.LoadScene(Coroutiner.Instance, nextZone);
        }
    }
}
