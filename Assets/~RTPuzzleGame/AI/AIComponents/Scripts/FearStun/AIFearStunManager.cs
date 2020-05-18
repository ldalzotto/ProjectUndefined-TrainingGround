﻿using GameConfigurationID;
using UnityEngine;
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

        public AIFearStunManager(AIFearStunComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public void Init(NavMeshAgent currentAgent, PuzzleEventsManager PuzzleEventsManager, AIFOVManager aiFovManager, AIObjectID AiID)
        {
            this.BaseInit(AiID, PuzzleEventsManager);
            this.currentAgent = currentAgent;
            this.aiFovManager = aiFovManager;
            this.AIFearTimeCounterManager = new AIFearTimeCounterManager();
        }
        
        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            if (!this.isFeared)
            {
                if (this.aiFovManager.GetFOVAngleSum() <= this.AssociatedAIComponent.FOVSumThreshold)
                {
                    this.SetIsFeared(true);
                    //because BeforeManagersUpdate is called before OnManagerTick, we anticipate the fact that the mananger will be called.
                    //this, we set the current timer so that after OnManagerTick will be called, fearedTimer = 0f
                    this.AIFearTimeCounterManager.InitFearTimer(-(d * timeAttenuationFactor), this.AssociatedAIComponent.TimeWhileBeginFeared);
                }
            }
        }

        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            if (this.AIFearTimeCounterManager.Tick(d, timeAttenuationFactor))
            {
                this.aiFovManager.ResetFOV();
                this.SetIsFeared(false);
            }

            if (this.isFeared)
            {
                NPCAIDestinationContext.TargetPosition = this.currentAgent.transform.position;
            }
        }

        #region External Events
        internal override void OnFearStarted(FearedStartAIBehaviorEvent fearedStartAIBehaviorEvent)
        {
            this.isFeared = true;
        }

        internal override void OnFearedForced(FearedForcedAIBehaviorEvent fearedForcedAIBehaviorEvent)
        {
            this.SetIsFeared(true);
            this.AIFearTimeCounterManager.InitFearTimer(0, fearedForcedAIBehaviorEvent.FearedTime);
        }
        #endregion




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
