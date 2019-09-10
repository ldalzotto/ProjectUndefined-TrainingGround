using UnityEngine;

namespace RTPuzzle
{

    public class PrefabContainer : MonoBehaviour
    {
        private static PrefabContainer instance;

        [Header("Projectile Prefabs")]
        [Space(20)]
        [Header("RT Puzzle Prefabs")]
        [Space(20)]
        public ThrowProjectileCursorType ThrowProjectileCursorTypePrefab;

        [Header("RT Puzzle Target Zone Prefab")]
        [Space(20)]
        public InteractiveObjectType TargetZonePrefab;


        [Header("Puzzle attracitve objects prefabs")]

        [Header("NPC prefabs")]
        public NpcInteractionRingType NpcInteractionRingPrefab;

        [Header("Cooldown Feed Prefabs")]
        public CooldownFeedManager CooldownFeedManager;
        public CooldownFeedLineType CooldownFeedLineType;

        [Header("Ranges prefab")]
        public RangeTypeObject BaseRangeTypeObject;
        public RangeTypeObject BaseSphereRangePrefab;

        [Header("Visual Feedback")]
        public LevelCompleteVisualFeedbackFX LevelCompletedParticleEffect;

        public DottedLine BaseDottedLineBasePrefab;

        public static PrefabContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = PuzzleGameSingletonInstances.PrefabContainer;
                }
                return instance;
            }
        }
    }

}
