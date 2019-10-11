using System.Collections.Generic;
#if UNITY_EDITOR
#endif
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelConfigurationData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfigurationDataData", order = 1)]
    public class LevelConfigurationData : ScriptableObject
    {
        [SerializeField]
        private float availableTimeAmount = 20f;

        [SerializeField]
        public List<PlayerActionInherentData> ConfiguredPlayerActions;

        [SerializeField]
        public LevelRangeEffectInherentData LevelRangeEffectInherentData;

        public float AvailableTimeAmount { get => availableTimeAmount; set => availableTimeAmount = value; }

    }

    [System.Serializable]
    public class LevelRangeEffectInherentData
    {
        public float DeltaIntensity = 0;
        [Range(-0.5f, 0.5f)]
        public float DeltaMixFactor = 0;
    }


}
