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
        public FovModule BaseFovModule;
        public ContextMarkVisualFeedbackModule BaseContextMarkVisualFeedbackModule;
        public LineVisualFeedbackModule BaseLineVisualFeedbackModule;
        public InteractiveObjectAnimationModule BaseInteractiveObjectAnimationModule;
        public LocalPuzzleCutsceneModule BaseLocalPuzzleCutsceneModule;
//${PuzzlePrefabConfiguration:baseInteractiveObjectPrefabs}

        [Header("AI")]
        public AIObjectType BaseAIObjectType;

        [Header("AI Feedback")]
        public ContextMarkVisualFeedbackMarkType BaseAIFeedbackMarkType;
        public ContextMarkVisualFeedbackMarkType ExclamationMarkContextMarkPrefab;
        public ContextMarkVisualFeedbackMarkType DoubleExclamationMarkPrefab;

        [Header("Projectile Prefabs")]
        public ThrowProjectileCursorType ThrowProjectileCursorTypePrefab;

        [Header("Cooldown Feed Prefabs")]
        public CooldownFeedLineType CooldownFeedLineType;

        [Header("Visual Feedback")]
        public LevelCompleteVisualFeedbackFX LevelCompletedParticleEffect;

        public DottedLine BaseDottedLineBasePrefab;
    }
}

