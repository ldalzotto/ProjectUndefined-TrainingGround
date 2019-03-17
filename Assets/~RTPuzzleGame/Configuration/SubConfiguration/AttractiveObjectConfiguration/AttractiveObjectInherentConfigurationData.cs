using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectInherentConfigurationData", menuName = "Configuration/PuzzleGame/AttractiveObjectConfiguration/AttractiveObjectInherentConfigurationData", order = 1)]
    public class AttractiveObjectInherentConfigurationData : ScriptableObject
    {
        public float EffectRange;
        public float EffectiveTime;

        public AttractiveObjectInherentConfigurationData(float effectRange, float effectiveTime)
        {
            this.Init(effectRange, effectiveTime);
        }

        public void Init(float effectRange, float effectiveTime)
        {
            EffectRange = effectRange;
            EffectiveTime = effectiveTime;
        }
    }
}
