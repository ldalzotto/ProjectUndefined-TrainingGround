using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("General", "Trigger LevelCompleted event when entering.")]
    public class LevelCompletionTriggerModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [Inline()]
        public RangeObjectInitialization RangeObjectInitialization;

        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration();
            var LevelCompletionTriggerModule = MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseLevelCompletionTriggerModule, parent);
            LevelCompletionTriggerModule.ResolveModuleDependencies();
            LevelCompletionTriggerModule.LevelCompletionRange = RangeObjectInitializer.FromRangeObjectInitialization(this.RangeObjectInitialization, parent.gameObject);
        }
    }
}
