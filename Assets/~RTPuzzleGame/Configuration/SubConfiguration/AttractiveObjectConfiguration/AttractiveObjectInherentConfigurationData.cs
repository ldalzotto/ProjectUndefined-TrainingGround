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
        public AttractiveObjectType AttractiveObjectPrefab;

        [Header("Animation")]

        [CustomEnum]
        public AnimationID PreActionAnimation;
        [CustomEnum]
        public AnimationID PostActionAnimation;

        public AttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime, GameObject AttractiveObjectModelPrefab, AttractiveObjectType AttractiveObjectPrefab)
        {
            this.Init(effectRange, effectiveTime, AttractiveObjectModelPrefab, AttractiveObjectPrefab);
        }

        public void Init(float effectRange, float effectiveTime, GameObject AttractiveObjectModelPrefab, AttractiveObjectType AttractiveObjectPrefab)
        {
            EffectRange = effectRange;
            EffectiveTime = effectiveTime;
            this.AttractiveObjectModelPrefab = AttractiveObjectModelPrefab;
            this.AttractiveObjectPrefab = AttractiveObjectPrefab;
        }

    }
}
