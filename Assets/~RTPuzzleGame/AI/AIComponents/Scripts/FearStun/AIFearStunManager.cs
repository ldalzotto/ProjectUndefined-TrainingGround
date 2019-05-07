﻿using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIFearStunManager : AbstractAIFearStunManager
    {
        #region External Dependencies
        private AIFOVManager aiFovManager;
        #endregion

        #region Internal Managers
        private AIFearTimeCounterManager AIFearTimeCounterManager;
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
            this.AIFearTimeCounterManager = new AIFearTimeCounterManager();
        }



        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            if (!this.isFeared)
            {
                if (this.aiFovManager.GetFOVAngleSum() <= this.AIFearStunComponent.FOVSumThreshold)
                {
                    this.SetIsFeared(true);
                    //because BeforeManagersUpdate is called before OnManagerTick, we anticipate the fact that the mananger will be called.
                    //this, we set the current timer so that after OnManagerTick will be called, fearedTimer = 0f
                    this.AIFearTimeCounterManager.InitFearTimer(-(d * timeAttenuationFactor), this.AIFearStunComponent.TimeWhileBeginFeared);
                }
            }
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            if (this.AIFearTimeCounterManager.Tick(d, timeAttenuationFactor))
            {
                this.aiFovManager.ResetFOV();
                this.SetIsFeared(false);
            }

            if (this.isFeared)
            {
                return this.currentAgent.transform.position;
            }
            return null;
        }

        #region External Events
        internal override void OnFearedForced(FearedForcedAIBehaviorEvent fearedForcedAIBehaviorEvent)
        {
            this.SetIsFeared(true);
            this.AIFearTimeCounterManager.InitFearTimer(0, fearedForcedAIBehaviorEvent.FearedTime);
        }
        #endregion

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

    class AIFearTimeCounterManager
    {

        private float fearedTimer;
        private float currentFearedTimerLimit;

        public bool Tick(float d, float timeAttenuationFactor)
        {
            fearedTimer += (d * timeAttenuationFactor);
            if (fearedTimer >= currentFearedTimerLimit)
            {
                fearedTimer -= currentFearedTimerLimit;
                return true;
            }
            return false;
        }

        public void InitFearTimer(float startFearTimer, float fearedTimerLimit)
        {
            this.fearedTimer = startFearTimer;
            this.currentFearedTimerLimit = fearedTimerLimit;
        }
    }
}
