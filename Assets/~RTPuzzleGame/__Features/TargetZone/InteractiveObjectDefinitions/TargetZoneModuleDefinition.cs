using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [ModuleMetadata("Interactive")]
    public class TargetZoneModuleDefinition : AbstractInteractiveObjectDefinition
    {
        [CustomEnum(configurationType: typeof(TargetZoneConfiguration))]
        public TargetZoneID TargetZoneID;

        public override void CreateObject(Transform parent)
        {
            var TargetZoneModule = MonoBehaviour.Instantiate(PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration().BaseTargetZoneModule, parent);
            TargetZoneModule.TargetZoneID = this.TargetZoneID;
        }
    }
}
