using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RangeTypeObjectDefinitionConfiguration", menuName = "Configuration/PuzzleGame/RangeTypeObjectDefinitionConfiguration/RangeTypeObjectDefinitionConfiguration", order = 1)]
    public class RangeTypeObjectDefinitionConfiguration : ConfigurationSerialization<RangeTypeObjectDefinitionID, RangeTypeObjectDefinitionConfigurationInherentData>
    {

    }

}
