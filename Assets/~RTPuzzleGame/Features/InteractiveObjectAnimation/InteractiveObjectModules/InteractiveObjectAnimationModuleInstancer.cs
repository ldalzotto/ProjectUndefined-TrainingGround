using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public static class InteractiveObjectAnimationModuleInstancer
    {
        public static void PopuplateFromDefinition(InteractiveObjectAnimationModuleDefinition InteractiveObjectAnimationModuleDefinition, Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInteractiveObjectAnimationModule, parent);
        }
    }

}
