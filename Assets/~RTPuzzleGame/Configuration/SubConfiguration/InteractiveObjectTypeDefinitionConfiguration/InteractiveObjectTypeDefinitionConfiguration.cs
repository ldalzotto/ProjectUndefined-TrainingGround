using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "InteractiveObjectTypeDefinitionConfiguration", menuName = "Configuration/PuzzleGame/InteractiveObjectTypeDefinitionConfiguration/InteractiveObjectTypeDefinitionConfiguration", order = 1)]
    public class InteractiveObjectTypeDefinitionConfiguration : ConfigurationSerialization<InteractiveObjectTypeDefinitionID, InteractiveObjectTypeDefinitionInherentData>
    {
    }

}
