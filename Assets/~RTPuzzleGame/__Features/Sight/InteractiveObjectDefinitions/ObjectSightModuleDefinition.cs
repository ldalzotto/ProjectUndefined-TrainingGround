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

        [ScriptableObjectSubstitution(substitutionName: nameof(RangeTypeObjectDefinitionInherentData),
                  sourcePickerName: nameof(RangeTypeObjectDefinitionIDPicker))]
        [CustomEnum(ConfigurationType = typeof(RangeTypeObjectDefinitionConfiguration))]
        public RangeTypeObjectDefinitionID RangeTypeObjectDefinitionID;

        [Inline()]
        public RangeTypeObjectDefinitionInherentData RangeTypeObjectDefinitionInherentData;
        public bool RangeTypeObjectDefinitionIDPicker;

        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration();
            var objectSightModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseObjectSightModule, parent);
            objectSightModule.ResolveInternalDependencies();
            objectSightModule.transform.localPosition = this.LocalPosition;
            objectSightModule.transform.localRotation = this.LocalRotation;
            if (this.RangeTypeObjectDefinitionIDPicker)
            {
                objectSightModule.SightVisionRange.RangeTypeObjectDefinitionID = this.RangeTypeObjectDefinitionID;
            }
            else
            {
                objectSightModule.SightVisionRange.RangeTypeObjectDefinitionID = RangeTypeObjectDefinitionID.NONE;
                this.RangeTypeObjectDefinitionInherentData.DefineRangeTypeObject(objectSightModule.SightVisionRange, puzzlePrefabConfiguration);
            }
        }
    }
}
