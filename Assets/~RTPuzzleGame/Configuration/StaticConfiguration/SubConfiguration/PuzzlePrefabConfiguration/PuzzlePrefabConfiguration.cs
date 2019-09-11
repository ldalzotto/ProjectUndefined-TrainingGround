using UnityEngine;

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
        public ActionInteractableObjectModule BaseActionInteractableObjectModule;
        public NearPlayerGameOverTriggerModule BaseNearPlayerGameOverTriggerModule;
        public LaunchProjectileModule BaseLaunchProjectileModule;
        public AttractiveObjectModule BaseAttractiveObjectModule;
        public ModelObjectModule BaseModelObjectModule;
        public DisarmObjectModule BaseDisarmObjectModule;
        public GrabObjectModule BaseGrabObjectModule;
        public ObjectRepelModule BaseObjectRepelModule;
        public ObjectSightModule BaseObjectSightModule;
        public AILogicColliderModule BaseAILogicColliderModule;
        public InRangeColliderTrackerModule BaseInRangeColliderTrackerModule;
//${PuzzlePrefabConfiguration:baseInteractiveObjectPrefabs}

        [Header("AI")]
        public AIObjectType BaseAIObjectType;

        [Header("AI Feedback")]
        public AIFeedbackMarkType BaseAIFeedbackMarkType;
        public AIFeedbackMarkType ProjectileHitPrefab;
        public AIFeedbackMarkType EscapeWithoutTargetPrefab;

        [Header("Projectile Prefabs")]
        public ThrowProjectileCursorType ThrowProjectileCursorTypePrefab;
        
        [Header("NPC prefabs")]
        public NpcInteractionRingType NpcInteractionRingPrefab;

        [Header("Cooldown Feed Prefabs")]
        public CooldownFeedLineType CooldownFeedLineType;

        [Header("Visual Feedback")]
        public LevelCompleteVisualFeedbackFX LevelCompletedParticleEffect;

        public DottedLine BaseDottedLineBasePrefab;
    }
}

