using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIFearStunManager : AbstractAIFearStunManager
    {
        private NavMeshAgent currentAgent;
        private AIFearStunComponent AIFearStunComponent;
        private PuzzleEventsManager PuzzleEventsManager;
        private AiID AiID;

        public AIFearStunManager(NavMeshAgent currentAgent, AIFearStunComponent aIFearStunComponent, PuzzleEventsManager PuzzleEventsManager, AiID AiID)
        {
            this.currentAgent = currentAgent;
            AIFearStunComponent = aIFearStunComponent;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.AiID = AiID;
        }

        private float fearedTimer;

        public override Vector3? TickComponent(AIFOVManager aIFOVManager)
        {
            if (!isFeared)
            {
                if (aIFOVManager.GetFOVAngleSum() <= this.AIFearStunComponent.FOVSumThreshold)
                {
                    this.SetIsFeared(true);
                    this.fearedTimer = 0f;
                    return this.currentAgent.transform.position;
                }
            }

            return null;
        }

        public override void TickWhileFeared(float d, float timeAttenuationFactor)
        {
            if (isFeared)
            {
                fearedTimer += (d * timeAttenuationFactor);
                if (fearedTimer >= AIFearStunComponent.TimeWhileBeginFeared)
                {
                    this.SetIsFeared(false);
                }
            }
        }

        private void SetIsFeared(bool newIsFearedValue)
        {
            if (newIsFearedValue)
            {
                if (!this.isFeared)
                {
                    this.PuzzleEventsManager.OnAIFearedStunned(this.AiID);
                }
            }
            else
            {
                if (this.isFeared)
                {
                    this.PuzzleEventsManager.OnAIFearedStunnedEnded(this.AiID);
                }
            }
            this.isFeared = newIsFearedValue;
        }
    }
}
