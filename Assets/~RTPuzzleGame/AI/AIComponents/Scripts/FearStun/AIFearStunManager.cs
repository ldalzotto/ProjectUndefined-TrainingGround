using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIFearStunManager : AbstractAIFearStunManager
    {
        #region External Dependencies
        private AIFOVManager aiFovManager;
        #endregion

        private NavMeshAgent currentAgent;
        private AIFearStunComponent AIFearStunComponent;
        private PuzzleEventsManager PuzzleEventsManager;
        private AiID AiID;

        public AIFearStunManager(NavMeshAgent currentAgent, AIFearStunComponent aIFearStunComponent, PuzzleEventsManager PuzzleEventsManager, AIFOVManager aiFovManager, AiID AiID)
        {
            this.currentAgent = currentAgent;
            AIFearStunComponent = aIFearStunComponent;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.aiFovManager = aiFovManager;
            this.AiID = AiID;
        }

        private float fearedTimer;

        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            if (!this.isFeared)
            {
                if (this.aiFovManager.GetFOVAngleSum() <= this.AIFearStunComponent.FOVSumThreshold)
                {
                    this.SetIsFeared(true);
                    //because BeforeManagersUpdate is called before OnManagerTick, we anticipate the fact that the mananger will be called.
                    //this, we set the current timer so that after OnManagerTick will be called, fearedTimer = 0f
                    this.fearedTimer = -(d * timeAttenuationFactor);
                }
            }
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            fearedTimer += (d * timeAttenuationFactor);
            if (fearedTimer >= AIFearStunComponent.TimeWhileBeginFeared)
            {
                this.aiFovManager.ResetFOV();
                fearedTimer -= AIFearStunComponent.TimeWhileBeginFeared;
                this.SetIsFeared(false);
            }
            if (this.isFeared)
            {
                return this.currentAgent.transform.position;
            }
            return null;
        }

        private void SetIsFeared(bool newIsFearedValue)
        {
            if (newIsFearedValue)
            {
                if (!this.isFeared)
                {
                    this.PuzzleEventsManager.PZ_EVT_AI_FearedStunned_Start(this.AiID);
                }
            }
            else
            {
                if (this.isFeared)
                {
                    this.PuzzleEventsManager.PZ_EVT_AI_FearedStunned_Ended(this.AiID);
                }
            }
            this.isFeared = newIsFearedValue;
        }
    }
}
