using UnityEngine;

namespace RTPuzzle
{
    public class AIEventsManager : MonoBehaviour
    {

        #region External Dependencies
        private NPCAIManagerContainer NPCAIManagerContainer;
        #endregion

        public void Init()
        {
            this.NPCAIManagerContainer = GameObject.FindObjectOfType<NPCAIManagerContainer>();
        }

        public void OnHittedByProjectileFirstTime(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectileFirstTime();
        }

        public void OnHittedByProjectile2InARow(AiID aiID)
        {
            this.NPCAIManagerContainer.GetNPCAiManager(aiID).OnHittedByProjectile2InARow();
        }
    }

}
