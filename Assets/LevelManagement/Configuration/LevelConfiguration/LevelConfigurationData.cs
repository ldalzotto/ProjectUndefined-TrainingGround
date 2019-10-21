using System;
using System.Collections.Generic;
using PlayerActions;
using RangeObjects;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "LevelConfigurationData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfigurationDataData", order = 1)]
    public class LevelConfigurationData : ScriptableObject
    {
        [SerializeField] public List<PlayerActionInherentData> ConfiguredPlayerActions;

        [SerializeField] public LevelRangeEffectInherentData LevelRangeEffectInherentData;
    }
}