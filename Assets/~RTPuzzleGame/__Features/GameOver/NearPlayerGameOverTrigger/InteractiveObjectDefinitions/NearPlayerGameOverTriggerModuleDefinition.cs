//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("Interactive", "Trigger GameOver when player is near.")]
    public class NearPlayerGameOverTriggerModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [DrawConfiguration(ConfigurationType = typeof(NearPlayerGameOverTriggerConfiguration))]
        [CustomEnum(ConfigurationType = typeof(NearPlayerGameOverTriggerConfiguration))]
        public NearPlayerGameOverTriggerID NearPlayerGameOverTriggerID;

        public override void CreateObject(Transform parent)
        {
            var nearPlayerGameOverTriggerModule = MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseNearPlayerGameOverTriggerModule, parent);
            nearPlayerGameOverTriggerModule.NearPlayerGameOverTriggerID = this.NearPlayerGameOverTriggerID;
        }
    }
}