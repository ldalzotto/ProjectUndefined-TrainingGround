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

    [ModuleMetadata("General", "Tag the object to be selectable by player.")]
    public class ActionInteractableObjectModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [CustomEnum(ConfigurationType = typeof(ActionInteractableObjectConfiguration))]
        public ActionInteractableObjectID ActionInteractableObjectID;

        public override void CreateObject(Transform parent)
        {
            var ActionInteractableObjectModule = MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseActionInteractableObjectModule, parent);
            ActionInteractableObjectModule.ActionInteractableObjectID = this.ActionInteractableObjectID;
        }
    }
}
