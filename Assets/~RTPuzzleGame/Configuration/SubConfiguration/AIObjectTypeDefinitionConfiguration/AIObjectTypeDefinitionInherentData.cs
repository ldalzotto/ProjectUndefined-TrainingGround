//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RTPuzzle
{
    using GameConfigurationID;
    using UnityEngine;


    [System.Serializable()]
    [UnityEngine.CreateAssetMenu(fileName = "AIObjectTypeDefinitionInherentData", menuName = "Configuration/PuzzleGame/AIObjectTypeDefinitionConfiguration/AIObjectTypeDefiniti" +
        "onInherentData", order = 1)]
    public class AIObjectTypeDefinitionInherentData : ScriptableObject
    {
        [CustomEnum(ConfigurationType = typeof(InteractiveObjectTypeDefinitionConfiguration), OpenToConfiguration = true)]
        public InteractiveObjectTypeDefinitionID InteractiveObjectTypeDefinitionID;
        
        [CustomEnum(ConfigurationType = typeof(AIComponentsConfiguration))]
        public AiID AiId;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public GenericPuzzleAIComponentsV2 GenericPuzzleAIComponents;

    }
}
