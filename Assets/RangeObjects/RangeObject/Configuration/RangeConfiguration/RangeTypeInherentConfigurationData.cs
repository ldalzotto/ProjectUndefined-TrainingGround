using System;
using UnityEngine;

namespace RangeObjects
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeInherentConfigurationData", menuName = "Configuration/PuzzleGame/RangeTypeConfiguration/RangeTypeInherentConfigurationData", order = 1)]
    public class RangeTypeInherentConfigurationData : ScriptableObject
    {
        public Material InRangeEffectMaterial;
        public float RangeAnimationSpeed;
        public Color RangeBaseColor;

        #region Runtime Methods

        private Func<Color> rangeColorProvider;

        public Func<Color> RangeColorProvider
        {
            get => rangeColorProvider;
            set => rangeColorProvider = value;
        }

        #endregion
    }
}