using UnityEngine;

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
