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

#if UNITY_EDITOR
    [CustomEditor(typeof(AIFearStunComponent))]
    public class AIFearStunComponentEditor : AbstractAIComponentEditor<AIFearStunComponent>
    { }
#endif

    public abstract class AbstractAIFearStunManager
    {
        #region State
        protected bool isFeared;
        #endregion

        public bool IsFeared { get => isFeared; }

        public abstract Vector3? TickComponent(AIFOVManager aIFOVManager);
        public abstract void TickWhileFeared(float d, float timeAttenuationFactor);
    }

    
}
