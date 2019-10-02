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
    [ModuleMetadata("Interactive", "A range representing sight vision.")]
    public class ObjectSightModuleDefinition : AbstractInteractiveObjectDefinition
    {
        public Vector3 LocalPosition;
        public Quaternion LocalRotation;

        [Inline(CreateAtSameLevelIfAbsent = true)]
        public RangeObjectInitialization RangeObjectInitialization;

        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration();
            var objectSightModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseObjectSightModule, parent);
            objectSightModule.transform.localPosition = this.LocalPosition;
            objectSightModule.transform.localRotation = this.LocalRotation;
            objectSightModule.SightVisionRange = RangeObjectInitializer.FromRangeObjectInitialization(this.RangeObjectInitialization, parent.gameObject);
        }
    }
}
