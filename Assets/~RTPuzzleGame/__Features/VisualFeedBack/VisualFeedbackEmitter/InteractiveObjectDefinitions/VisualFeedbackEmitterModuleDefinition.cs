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
    [ModuleMetadata("Visual Feedback", "Send Data To Visual Feedback Modules.")]
    public class VisualFeedbackEmitterModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [Inline()]
        public RangeObjectInitialization RangeObjectInitialization;

        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration();
            var inRangeVisualFeedbackModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInRangeColliderTrackerModule, parent);
            inRangeVisualFeedbackModule.InRangeVisualFeedbackTrackerRange = RangeObjectInitializer.FromRangeObjectInitialization(this.RangeObjectInitialization, parent.gameObject);
        }
    }
}
