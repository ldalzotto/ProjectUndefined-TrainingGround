using ConfigurationEditor;
using GameConfigurationID;
using UnityEngine;

namespace RangeObjects
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeConfiguration", menuName = "Configuration/PuzzleGame/RangeTypeConfiguration/RangeTypeConfiguration", order = 1)]
    public class RangeTypeConfiguration : ConfigurationSerialization<RangeTypeID, RangeTypeInherentConfigurationData>
    {
    }
}