using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("General")]
    public class ContextMarkVisualFeedbackModuleDefinition : AbstractInteractiveObjectDefinition
    {
        public override void CreateObject(Transform parent)
        {
            MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseContextMarkVisualFeedbackModule, parent);
        }
    }
}
