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
        public GameObject AttractiveObjectModelPrefab;
        public InteractiveObjectType AttractiveInteractiveObjectPrefab;

        [Header("Animation")]

        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

        public AttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime, GameObject AttractiveObjectModelPrefab, InteractiveObjectType AttractiveInteractiveObjectPrefab)
        {
            this.Init(effectRange, effectiveTime, AttractiveObjectModelPrefab, AttractiveInteractiveObjectPrefab);
        }

        public void Init(float effectRange, float effectiveTime, GameObject AttractiveObjectModelPrefab, InteractiveObjectType AttractiveInteractiveObjectPrefab)
        {
            EffectRange = effectRange;
            EffectiveTime = effectiveTime;
            this.AttractiveObjectModelPrefab = AttractiveObjectModelPrefab;
            this.AttractiveInteractiveObjectPrefab = AttractiveInteractiveObjectPrefab;
        }

    }
}
