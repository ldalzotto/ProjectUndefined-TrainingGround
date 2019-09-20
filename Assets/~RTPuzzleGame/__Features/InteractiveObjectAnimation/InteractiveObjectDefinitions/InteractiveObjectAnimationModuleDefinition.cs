using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("General", "Manage animator variables.")]
    public class InteractiveObjectAnimationModuleDefinition : AbstractInteractiveObjectDefinition
    {
        public override void CreateObject(Transform parent)
        {
            var puzzlePrefabConfiguration = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration;
            MonoBehaviour.Instantiate(puzzlePrefabConfiguration.BaseInteractiveObjectAnimationModule, parent);
        }
    }
    
}
