using GameConfigurationID;
using System;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIFearStunComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIFearStunComponent", order = 1)]
    public class AIFearStunComponent : AbstractAIComponent
    {
        public float FOVSumThreshold;
        public float TimeWhileBeginFeared;

        public override InterfaceAIManager BuildManager()
        {
            return new AIFearStunManager(this);
        }
    }

    public abstract class AbstractAIFearStunManager : AbstractAIManager<AIFearStunComponent>, InterfaceAIManager
    {
        protected AIObjectID AiID;

        #region State
        protected bool isFeared;
        #endregion

        #region External Dependencies
        protected PuzzleEventsManager PuzzleEventsManager;
        #endregion
        protected AbstractAIFearStunManager(AIFearStunComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public virtual void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            AiID = AIBheaviorBuildInputData.aiID;
            PuzzleEventsManager = AIBheaviorBuildInputData.PuzzleEventsManager;
        }

        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public bool IsManagerEnabled()
        {
            return this.isFeared;
        }

        public virtual void OnDestinationReached() { }

        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);

        public void OnStateReset()
        {
            this.isFeared = false;
        }
        
        internal abstract void OnFearStarted(FearedStartAIBehaviorEvent fearedStartAIBehaviorEvent);
        internal abstract void OnFearedForced(FearedForcedAIBehaviorEvent fearedForcedAIBehaviorEvent);

        protected void SetIsFeared(bool newIsFearedValue)
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
