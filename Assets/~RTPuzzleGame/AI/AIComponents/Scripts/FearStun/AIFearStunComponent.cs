using System;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIFearStunComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIFearStunComponent", order = 1)]
    public class AIFearStunComponent : AbstractAIComponent
    {
        public float FOVSumThreshold;
        public float TimeWhileBeginFeared;

        protected override Type abstractManagerType => typeof(AbstractAIFearStunManager);
    }

    public abstract class AbstractAIFearStunManager : InterfaceAIManager
    {
        #region State
        protected bool isFeared;
        #endregion
        
        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);

        public bool IsManagerEnabled()
        {
            return this.isFeared;
        }

        public virtual void OnDestinationReached() { }

        public abstract Vector3? OnManagerTick(float d, float timeAttenuationFactor);

        public void OnStateReset()
        {
            this.isFeared = false;
        }
    }

    
}
