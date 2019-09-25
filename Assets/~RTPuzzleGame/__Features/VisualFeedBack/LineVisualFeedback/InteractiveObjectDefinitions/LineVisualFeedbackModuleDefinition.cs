using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("Visual Feedback", "Allow display of line feedbacks.")]
    public class LineVisualFeedbackModuleDefinition : AbstractInteractiveObjectDefinition
    {
        public override void CreateObject(Transform parent)
        {
            MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseLineVisualFeedbackModule, parent);
        }
    }
}