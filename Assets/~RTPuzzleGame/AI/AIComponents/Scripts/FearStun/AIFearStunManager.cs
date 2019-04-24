﻿using UnityEngine;
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
                    this.fearedTimer = 0f;
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
