using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("General", "Allow display of feedback icons.")]
    public class ContextMarkVisualFeedbackModuleDefinition : AbstractInteractiveObjectDefinition
    {
        public override void CreateObject(Transform parent)
        {
            MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseContextMarkVisualFeedbackModule, parent);
        }
    }
}
