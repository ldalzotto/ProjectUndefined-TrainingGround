using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace RTPuzzle
{
    public class InteractiveObjectCutsceneControllerModuleDefinition : AbstractInteractiveObjectDefinition
    {
        public override void CreateObject(Transform parent)
        {
            MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseInteractiveObjectCutsceneControllerModule, parent);
        }
    }
}
