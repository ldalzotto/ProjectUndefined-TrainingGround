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
    public class ObjectRepelModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [CustomEnum(ConfigurationType = typeof(ObjectRepelConfiguration))]
        public ObjectRepelID ObjectRepelID;

        public override void CreateObject(Transform parent)
        {
            var objectRepelModule = MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseObjectRepelModule, parent);
            objectRepelModule.ObjectRepelID = this.ObjectRepelID;
        }
    }
}
