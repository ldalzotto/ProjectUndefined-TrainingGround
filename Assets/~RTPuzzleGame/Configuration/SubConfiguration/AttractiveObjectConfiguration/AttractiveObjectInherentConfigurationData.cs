using UnityEngine;
using GameConfigurationID;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectInherentConfigurationData", menuName = "Configuration/PuzzleGame/AttractiveObjectConfiguration/AttractiveObjectInherentConfigurationData", order = 1)]
    public class AttractiveObjectInherentConfigurationData : ScriptableObject
    {
        public float EffectRange;
        public float EffectiveTime;
        public InteractiveObjectType AttractiveInteractiveObjectPrefab;

        [Header("Animation")]

        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

        public AttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime, InteractiveObjectType AttractiveInteractiveObjectPrefab)
        {
            this.Init(effectRange, effectiveTime, AttractiveInteractiveObjectPrefab);
        }

        public void Init(float effectRange, float effectiveTime, InteractiveObjectType AttractiveInteractiveObjectPrefab)
        {
            EffectRange = effectRange;
            EffectiveTime = effectiveTime;
            this.AttractiveInteractiveObjectPrefab = AttractiveInteractiveObjectPrefab;
        }

    }
}
