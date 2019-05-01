using UnityEngine;
using System.Collections;
using System;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeInherentConfigurationData", menuName = "Configuration/PuzzleGame/RangeTypeConfiguration/RangeTypeInherentConfigurationData", order = 1)]
    public class RangeTypeInherentConfigurationData : ScriptableObject
    {
        public Material GoundEffectMaterial;
        public float RangeAnimationSpeed;

        #region Runtime Methods
        private Func<Color> rangeColorProvider;
        public Func<Color> RangeColorProvider { get => rangeColorProvider; set => rangeColorProvider = value; }
        #endregion
    }
}
