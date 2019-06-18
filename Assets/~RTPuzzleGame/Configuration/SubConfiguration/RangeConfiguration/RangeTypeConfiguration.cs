using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeConfiguration", menuName = "Configuration/PuzzleGame/RangeTypeConfiguration/RangeTypeConfiguration", order = 1)]
    public class RangeTypeConfiguration : ConfigurationSerialization<RangeTypeID, RangeTypeInherentConfigurationData>
    {

    }

}
