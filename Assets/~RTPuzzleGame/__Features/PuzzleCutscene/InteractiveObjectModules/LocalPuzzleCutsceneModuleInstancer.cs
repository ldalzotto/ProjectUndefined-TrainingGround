using UnityEngine;

namespace RTPuzzle
{
    public static class LocalPuzzleCutsceneModuleInstancer
    {
        public static void PopuplateFromDefinition(LocalPuzzleCutsceneModuleDefinition LocalPuzzleCutsceneModuleDefinition, Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseLocalPuzzleCutsceneModule, parent);
        }
    }
}
