using System;
using System.Collections.Generic;
using RangeObjects;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfigurationData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfigurationDataData", order = 1)]
    public class LevelConfigurationData : ScriptableObject
    {
        [SerializeField] private float availableTimeAmount = 20f;

        [SerializeField] public List<PlayerActionInherentData> ConfiguredPlayerActions;

        [SerializeField] public LevelRangeEffectInherentData LevelRangeEffectInherentData;

        public float AvailableTimeAmount
        {
            get => availableTimeAmount;
            set => availableTimeAmount = value;
        }
    }
}