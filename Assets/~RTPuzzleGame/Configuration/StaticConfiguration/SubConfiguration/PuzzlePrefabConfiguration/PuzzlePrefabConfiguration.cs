using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PuzzlePrefabConfiguration", menuName = "Configuration/PuzzleGame/StaticConfiguration/PuzzlePrefabConfiguration", order = 1)]
    public class PuzzlePrefabConfiguration : ScriptableObject
    {
        [Header("Range")]
        public RangeTypeObject BaseRangeTypeObject;
        public RoundedFrustumRangeType BaseRoundedFrustumRangeType;
        public FrustumRangeType BaseFrustumRangeType;
        public BoxRangeType BaseBoxRangeType;
        public SphereRangeType BaseSphereRangeType;
        public RangeObstacleListener BaseRangeObstacleListener;

        [Header("Interactive Object")]
        public InteractiveObjectType BaseInteractiveObjectType;
        public TargetZoneModule BaseTargetZoneModule;
        public LevelCompletionTriggerModule BaseLevelCompletionTriggerModule;
        public InteractiveObjectCutsceneControllerModule BaseInteractiveObjectCutsceneControllerModule;
    }
}

