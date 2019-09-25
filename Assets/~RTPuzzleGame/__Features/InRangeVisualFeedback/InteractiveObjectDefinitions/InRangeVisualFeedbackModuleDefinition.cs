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
    [ModuleMetadata("Interactive", "Track all colliders elligible to in range visual effect.")]
    public class InRangeVisualFeedbackModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [DrawDefinition(ConfigurationType = typeof(RangeTypeObjectDefinitionConfiguration))]
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
            var inRangeVisualFeedbackModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInRangeColliderTrackerModule, parent);
            var inRangeVisualFeedbackTrackerRange = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseRangeTypeObject, inRangeVisualFeedbackModule.transform);
            inRangeVisualFeedbackModule.ResolveInternalDependencies();

            if (this.RangeTypeObjectDefinitionIDPicker)
            {
                inRangeVisualFeedbackModule.InRangeVisualFeedbackTrackerRange.RangeTypeObjectDefinitionID = this.RangeTypeObjectDefinitionID;
            }
            else
            {
                inRangeVisualFeedbackModule.InRangeVisualFeedbackTrackerRange.RangeTypeObjectDefinitionID = RangeTypeObjectDefinitionID.NONE;
                this.RangeTypeObjectDefinitionInherentData.DefineRangeTypeObject(inRangeVisualFeedbackModule.InRangeVisualFeedbackTrackerRange, puzzlePrefabConfiguration);
            }
        }
    }
}
