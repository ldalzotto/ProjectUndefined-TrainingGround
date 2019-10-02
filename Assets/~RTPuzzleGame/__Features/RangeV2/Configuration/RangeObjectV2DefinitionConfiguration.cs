using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeObjectV2DefinitionConfiguration", menuName = "Configuration/PuzzleGame/RangeTypeObjectDefinitionConfiguration/RangeObjectV2DefinitionConfiguration", order = 1)]
    public class RangeObjectV2DefinitionConfiguration : ConfigurationSerialization<RangeObjectDefinitionIDv2, RangeObjectV2DefinitionInherentData>
    {

    }

}
