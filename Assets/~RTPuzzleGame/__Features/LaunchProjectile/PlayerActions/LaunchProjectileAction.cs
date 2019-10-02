using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;
using static AnimationConstants;

namespace RTPuzzle
{
    public class LaunchProjectileAction : RTPPlayerAction
    {
        public LaunchProjectileAction(LaunchProjectileActionInherentData launchProjectileActionInherentData) : base(launchProjectileActionInherentData)
        {
        }

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private PlayerManagerDataRetriever PlayerManagerDataRetriever;
        #endregion

        private LaunchProjectileScreenPositionManager LaunchProjectileScreenPositionManager;
        private LaunchProjectileRayPositionerManager LaunchProjectileRayPositionerManager;
        private ThrowProjectileManager ThrowProjectileManager;
        private LauncheProjectileActionExitManager LauncheProjectileActionExitManager;
        private LaunchProjectilePlayerAnimationManager LaunchProjectilePlayerAnimationManager;
        private PlayerOrientationManager PlayerOrientationManager;
        private LaunchProjectilePathAnimationManager LaunchProjectilePathAnimationManager;

        private LaunchProjectileInherentData projectileInherentData;
        private InteractiveObjectType projectileObject;
        public RangeObjectV2 ProjectileThrowRange { get; private set; }

        private bool isActionFinished = false;

        public InteractiveObjectType GetProjectileEffectCursorRange()
        {
            return this.LaunchProjectileRayPositionerManager.ProjectileCursorInteractiveObject;
        }

        public override bool FinishedCondition()
        {
            return isActionFinished;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            isActionFinished = false;

            #region External Dependencies
            var gameInputManager = CoreGameSingletonInstances.GameInputManager;
            this.PlayerManagerDataRetriever = PuzzleGameSingletonInstances.PlayerManagerDataRetriever;
            var camera = Camera.main;
            PuzzleEventsManager = PuzzleGameSingletonInstances.PuzzleEventsManager;
            PuzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            var CameraMovementManager = CoreGameSingletonInstances.CameraMovementManager;
            var PuzzleStaticConfigurationContainer = PuzzleGameSingletonInstances.PuzzleStaticConfigurationContainer;
            var canvas = GameObject.FindGameObjectWithTag(TagConstants.PUZZLE_CANVAS).GetComponent<Canvas>();
            var interactiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            #endregion

            var playerTransform = PlayerManagerDataRetriever.GetPlayerTransform();
            var playerTransformScreen = camera.WorldToScreenPoint(playerTransform.position);
            playerTransformScreen.y = camera.pixelHeight - playerTransformScreen.y;

            LaunchProjectileActionInherentData LaunchProjectileActionInherentData = (LaunchProjectileActionInherentData)this.playerActionInherentData;

            var launchProjectileInteractiveObjectDefinition = PuzzleGameConfigurationManager.PuzzleGameConfiguration.InteractiveObjectTypeDefinitionConfiguration.ConfigurationInherentData[LaunchProjectileActionInherentData.projectedObjectDefinitionID];
            this.projectileInherentData = PuzzleGameConfigurationManager.ProjectileConf()[launchProjectileInteractiveObjectDefinition.GetDefinitionModule<LaunchProjectileModuleDefinition>().LaunchProjectileID];

            this.ProjectileThrowRange = new SphereRangeObjectV2(new RangeGameObjectV2(null),
                    new SphereRangeObjectInitialization
                    {
                        RangeTypeID = RangeTypeID.LAUNCH_PROJECTILE,
                        IsTakingIntoAccountObstacles = true,
                        SphereRangeTypeDefinition =
                                new SphereRangeTypeDefinition
                                {
                                    Radius = this.projectileInherentData.ProjectileThrowRange
                                }
                    });
            this.ProjectileThrowRange.ReceiveEvent(new SetWorldPositionEvent { WorldPosition = PlayerManagerDataRetriever.GetPlayerWorldPosition() });

            this.projectileObject = ProjectileActionInstanciationHelper.CreateProjectileAtStart(this.projectileInherentData, launchProjectileInteractiveObjectDefinition,
                     interactiveObjectContainer, PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.PuzzlePrefabConfiguration, PuzzleGameConfigurationManager.PuzzleGameConfiguration);

            LaunchProjectileScreenPositionManager = new LaunchProjectileScreenPositionManager(playerTransformScreen, gameInputManager, canvas, CameraMovementManager);
            LaunchProjectileRayPositionerManager = new LaunchProjectileRayPositionerManager(camera, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition, this, PuzzleEventsManager, PuzzleStaticConfigurationContainer,
                         this.projectileInherentData, PuzzleGameConfigurationManager, this.projectileObject, PuzzleGameConfigurationManager.PuzzleGameConfiguration, interactiveObjectContainer);
            LaunchProjectilePathAnimationManager = new LaunchProjectilePathAnimationManager(PlayerManagerDataRetriever, LaunchProjectileRayPositionerManager, PuzzleGameConfigurationManager);
            ThrowProjectileManager = new ThrowProjectileManager(this, gameInputManager, this.projectileObject, playerTransform);
            LauncheProjectileActionExitManager = new LauncheProjectileActionExitManager(gameInputManager, this, this.projectileObject, interactiveObjectContainer);
            LaunchProjectilePlayerAnimationManager = new LaunchProjectilePlayerAnimationManager(PlayerManagerDataRetriever.GetPlayerAnimator(), PuzzleGameConfigurationManager.PuzzleGameConfiguration.PuzzleCutsceneConfiguration,
                interactiveObjectContainer, this.projectileInherentData, this.projectileObject);
            PlayerOrientationManager = new PlayerOrientationManager(PlayerManagerDataRetriever.GetPlayerRigidBody());
            LaunchProjectileRayPositionerManager.Tick(0f, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition);
        }


        public override void Tick(float d)
        {
            LaunchProjectilePlayerAnimationManager.Tick(d);

            //If launch animation is not playng (animation exit callback is exiting the action)
            // LaunchProjectilePlayerAnimationManager.Tick(d); may trigger the exit event -> we check 
            if (!this.LaunchProjectilePlayerAnimationManager.LaunchProjectileAnimationPlaying() && !this.FinishedCondition())
            {
                if (LaunchProjectileRayPositionerManager.IsCursorPositioned)
                {
                    ThrowProjectileManager.Tick(d, ref LaunchProjectileRayPositionerManager);
                    LaunchProjectilePathAnimationManager.Tick(d);
                }

                //If exit not called
                if (!LauncheProjectileActionExitManager.Tick())
                {
                    LaunchProjectileScreenPositionManager.Tick(d);
                    LaunchProjectileRayPositionerManager.Tick(d, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition);
                }
            }
        }

        public override void LateTick(float d)
        {
        }

        #region Internal Events
        public void OnExit()
        {
            this.ProjectileThrowRange.OnDestroy();
            LaunchProjectileRayPositionerManager.OnExit();
            LaunchProjectileScreenPositionManager.OnExit();
            LaunchProjectilePlayerAnimationManager.OnExit();
            LaunchProjectilePathAnimationManager.OnExit();
            isActionFinished = true;
        }

        public void OnLaunchProjectileSpawn()
        {
            if (LaunchProjectileRayPositionerManager.IsCursorPositioned)
            {
                this.SpawnLaunchProjectile(LaunchProjectileRayPositionerManager.GetCurrentCursorWorldPosition());
            }
        }

        public void SpawnLaunchProjectile(Vector3 tragetWorldPosition)
        {
            PlayerOrientationManager.OnLaunchProjectileSpawn(tragetWorldPosition);
            this.LaunchProjectilePlayerAnimationManager.PlayThrowProjectileAnimation(
                onAnimationEnd: () =>
                {
                    this.PlayerActionConsumed();
                    var throwPorjectilePath = BeziersControlPoints.Build(this.PlayerManagerDataRetriever.GetPlayerPuzzleLogicRootCollier().bounds.center, tragetWorldPosition,
                                         this.PlayerManagerDataRetriever.GetPlayerPuzzleLogicRootCollier().transform.up, BeziersControlPointsShape.CURVED);
                    ThrowProjectileManager.OnLaunchProjectileSpawn(this.projectileInherentData, throwPorjectilePath);
                }
               );
        }


        public void OnThrowProjectileCursorOnProjectileRange()
        {
            LaunchProjectilePathAnimationManager.OnThrowProjectileCursorOnProjectileRange();
            LaunchProjectileScreenPositionManager.OnThrowProjectileCursorOnProjectileRange();
        }
        public void OnThrowProjectileCursorOutOfProjectileRange()
        {
            LaunchProjectilePathAnimationManager.OnThrowProjectileCursorOutOfProjectileRange();
            LaunchProjectileScreenPositionManager.OnThrowProjectileCursorOutOfProjectileRange();
        }
        #endregion

        public override void GizmoTick()
        {
        }

        public override void GUITick()
        {
        }

    }

    #region Screen Position Manager
    class LaunchProjectileScreenPositionManager
    {
        private GameInputManager GameInputManager;
        private CameraMovementManager CameraMovementManager;

        private ThrowProjectileCursorType cursorObject;
        private Vector2 currentCursorScreenPosition;

        public LaunchProjectileScreenPositionManager(
            Vector2 currentCursorScreenPosition, GameInputManager GameInputManager, Canvas gameCanvas, CameraMovementManager CameraMovementManager)
        {
            this.currentCursorScreenPosition = currentCursorScreenPosition;
            this.GameInputManager = GameInputManager;
            this.cursorObject = ThrowProjectileCursorType.Instanciate(gameCanvas.transform);
            this.CameraMovementManager = CameraMovementManager;
            UpdateCursorPosition();
        }

        public Vector2 CurrentCursorScreenPosition { get => currentCursorScreenPosition; }

        public void Tick(float d)
        {
            if (!this.CameraMovementManager.IsCameraRotating())
            {
                var locomotionAxis = GameInputManager.CurrentInput.CursorDisplacement();
                currentCursorScreenPosition += (new Vector2(locomotionAxis.x, -locomotionAxis.z) * d);
                currentCursorScreenPosition.x = Mathf.Clamp(currentCursorScreenPosition.x, 0f, Screen.width);
                currentCursorScreenPosition.y = Mathf.Clamp(currentCursorScreenPosition.y, 0f, Screen.height);
                UpdateCursorPosition();
            }
        }

        private void UpdateCursorPosition()
        {
            if (this.cursorObject != null)
            {
                this.cursorObject.transform.position = new Vector3(currentCursorScreenPosition.x, Camera.main.pixelHeight - currentCursorScreenPosition.y, 0f);
            }
        }

        public void OnExit()
        {
            MonoBehaviour.Destroy(cursorObject.gameObject);
        }

        public void OnThrowProjectileCursorOnProjectileRange()
        {
            this.cursorObject.OnCursorOnProjectileRange();
        }

        public void OnThrowProjectileCursorOutOfProjectileRange()
        {
            this.cursorObject.OnCursorOutOfProjectileRange();
        }

    }

    [System.Serializable]
    public class LaunchProjectileScreenPositionManagerComponent
    {
        [Tooltip("In screen width %")]
        [Range(0.0f, 1.0f)]
        public float CursorSpeedScreenWidthPerCent;
    }
    #endregion

    #region Launch projectile ray manager
    class LaunchProjectileRayPositionerManager
    {
        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer;
        private PuzzleGameConfiguration PuzzleGameConfiguration;
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        private Camera camera;
        private LaunchProjectileInherentData projectileInherentData;
        private LaunchProjectileAction launchProjectileActionRef;
        private InteractiveObjectType projectileCursorInteractiveObject;

        private Vector3 currentCursorWorldPosition;
        private float effectiveEffectRange;

        private bool isCursorPositioned;
        private bool isCursorInRange;

        public bool IsCursorPositioned { get => isCursorPositioned; }
        public bool IsCursorInRange { get => isCursorInRange; }
        public InteractiveObjectType ProjectileCursorInteractiveObject { get => projectileCursorInteractiveObject; }

        public Vector3 GetCurrentCursorWorldPosition()
        {
            return this.currentCursorWorldPosition;
        }

        public LaunchProjectileRayPositionerManager(Camera camera, Vector2 cursorScreenPositionAtInit, LaunchProjectileAction launchProjectileAction,
                PuzzleEventsManager PuzzleEventsManager, PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer, LaunchProjectileInherentData projectileInherentData,
                PuzzleGameConfigurationManager puzzleGameConfigurationManager, InteractiveObjectType projectileInteractiveObject, PuzzleGameConfiguration PuzzleGameConfiguration,
                InteractiveObjectContainer InteractiveObjectContainer)
        {
            this.camera = camera;
            this.launchProjectileActionRef = launchProjectileAction;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.PuzzleStaticConfigurationContainer = PuzzleStaticConfigurationContainer;
            this.PuzzleGameConfiguration = PuzzleGameConfiguration;
            this.projectileInherentData = projectileInherentData;
            this.InteractiveObjectContainer = InteractiveObjectContainer;

            if (this.projectileInherentData.isExploding)
            {
                this.effectiveEffectRange = this.projectileInherentData.ExplodingEffectRange;
            }
            else if (this.projectileInherentData.isPersistingToAttractiveObject)
            {
                projectileInteractiveObject.GetDisabledModule<AttractiveObjectModule>().IfNotNull((AttractiveObjectTypeModule) => this.effectiveEffectRange = puzzleGameConfigurationManager.AttractiveObjectsConfiguration()[AttractiveObjectTypeModule.AttractiveObjectId].EffectRange);
            }
        }

        public void Tick(float d, Vector2 currentScreenPositionPoint)
        {
            Ray ray = camera.ScreenPointToRay(new Vector2(currentScreenPositionPoint.x, camera.pixelHeight - currentScreenPositionPoint.y));
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER)))
            {
                if (!isCursorPositioned)
                {
                    if (this.projectileCursorInteractiveObject != null)
                    {
                        MonoBehaviour.DestroyImmediate(this.projectileCursorInteractiveObject.gameObject);
                    }

                    var projectileCursorInteractiveObjectInherentData = VisualFeedbackEmitterModuleInstancer.BuildVisualFeedbackEmitterFromRange(
                             RangeObjectInitializationDataBuilderV2.SphereRangeWithObstacleListener(this.effectiveEffectRange, RangeTypeID.LAUNCH_PROJECTILE_CURSOR)
                    );
                    var projectileCursorInteractiveObjectInitializationObject = new InteractiveObjectInitializationObject()
                    {
                        InRangeVisualFeedbakcModuleInitializationData = new InRangeVisualFeedbakcModuleInitializationData()
                        {
                            RangeInitializer = new RangeTypeObjectInitializer()
                            {
                                RangeColorProvider = this.GetLaunchProjectileRangeActiveColor
                            }
                        }
                    };
                    this.projectileCursorInteractiveObject = InteractiveObjectType.Instantiate(projectileCursorInteractiveObjectInherentData, projectileCursorInteractiveObjectInitializationObject,
                                 PuzzleStaticConfigurationContainer.GetPuzzlePrefabConfiguration(), this.PuzzleGameConfiguration);

                }
                isCursorPositioned = true;
                currentCursorWorldPosition = hit.point;
                this.projectileCursorInteractiveObject.transform.position = currentCursorWorldPosition;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

                if (RangeIntersectionOperations.IsInsideAndNotOccluded(this.launchProjectileActionRef.ProjectileThrowRange, currentCursorWorldPosition, forceObstacleOcclusionIfNecessary: true))
                {
                    SetIsCursorInRange(true);
                }
                else
                {
                    SetIsCursorInRange(false);
                }

            }
            else
            {
                if (isCursorPositioned)
                {
                    this.InteractiveObjectContainer.OnInteractiveObjectDestroyed(this.projectileCursorInteractiveObject);
                }
                isCursorPositioned = false;
                SetIsCursorInRange(false);
                Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.red);

            }
        }

        private void SetIsCursorInRange(bool currentFrameIsCursorInRange)
        {
            if (currentFrameIsCursorInRange)
            {
                if (!this.isCursorInRange)
                {
                    this.launchProjectileActionRef.OnThrowProjectileCursorOnProjectileRange();
                }
            }
            else
            {
                if (this.isCursorInRange)
                {
                    this.launchProjectileActionRef.OnThrowProjectileCursorOutOfProjectileRange();
                }
            }

            this.isCursorInRange = currentFrameIsCursorInRange;
        }

        private Color GetLaunchProjectileRangeActiveColor()
        {
            if (isCursorInRange && IsCursorInRange)
            {
                return this.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.RangeColorConfiguration.ProjectileCursorOnRangeColor;
            }
            else
            {
                return this.PuzzleStaticConfigurationContainer.PuzzleStaticConfiguration.RangeColorConfiguration.ProjectileCursorOutOfRangeColor;
            }
        }

        public void OnExit()
        {
            if (this.projectileCursorInteractiveObject != null)
            {
                this.InteractiveObjectContainer.OnInteractiveObjectDestroyed(this.projectileCursorInteractiveObject);
            }
        }

    }
    #endregion

    #region Throw Projectile Manager
    class ThrowProjectileManager
    {
        private LaunchProjectileAction LaunchProjectileRTPActionRef;
        private GameInputManager GameInputManager;
        private InteractiveObjectType projectileObjectRef;
        private Transform playerTransform;

        public ThrowProjectileManager(LaunchProjectileAction launchProjectileRTPActionRef, GameInputManager gameInputManager,
            InteractiveObjectType projectileObjectRef, Transform playerTransform)
        {
            LaunchProjectileRTPActionRef = launchProjectileRTPActionRef;
            GameInputManager = gameInputManager;
            this.projectileObjectRef = projectileObjectRef;
            this.playerTransform = playerTransform;
        }


        public void Tick(float d, ref LaunchProjectileRayPositionerManager LaunchProjectileRayPositionerManager)
        {
            if (GameInputManager.CurrentInput.ActionButtonD() && LaunchProjectileRayPositionerManager.IsCursorInRange)
            {
                LaunchProjectileRTPActionRef.OnLaunchProjectileSpawn();
            }
        }

        public void OnLaunchProjectileSpawn(LaunchProjectileInherentData launchProjectileInherentData, BeziersControlPoints throwProjectilePath)
        {
            this.projectileObjectRef.transform.rotation = Quaternion.LookRotation(this.playerTransform.forward, this.playerTransform.up);
            ProjectileActionInstanciationHelper.OnProjectileSpawn(ref this.projectileObjectRef, throwProjectilePath, launchProjectileInherentData);
            LaunchProjectileRTPActionRef.OnExit();
        }

    }
    #endregion

    #region Launch Projectile Exit Action
    class LauncheProjectileActionExitManager
    {

        private GameInputManager GameInputManager;
        private LaunchProjectileAction LaunchProjectileAction;
        private InteractiveObjectContainer InteractiveObjectContainer;
        private InteractiveObjectType projectileObjectRef;

        public LauncheProjectileActionExitManager(GameInputManager gameInputManager, LaunchProjectileAction LaunchProjectileActionRef, InteractiveObjectType projectileObjectRef, InteractiveObjectContainer InteractiveObjectContainer)
        {
            GameInputManager = gameInputManager;
            this.LaunchProjectileAction = LaunchProjectileActionRef;
            this.InteractiveObjectContainer = InteractiveObjectContainer;
            this.projectileObjectRef = projectileObjectRef;
        }

        public bool Tick()
        {
            if (IsExitRequested())
            {
                this.InteractiveObjectContainer.OnInteractiveObjectDestroyed(this.projectileObjectRef);
                LaunchProjectileAction.OnExit();
                return true;
            }
            return false;
        }

        private bool IsExitRequested()
        {
            return GameInputManager.CurrentInput.CancelButtonD();
        }
    }
    #endregion

    #region Launch Projectile player animation manager
    class LaunchProjectilePlayerAnimationManager
    {
        private PuzzleCutsceneConfiguration PuzzleCutsceneConfiguration;
        private InteractiveObjectContainer InteractiveObjectContainer;

        private InteractiveObjectType projectileObjectRef;
        private Animator playerAnimator;
        private LaunchProjectileInherentData ProjectileInherentData;

        private SequencedActionPlayer ProjectilePreAnimationPlayer;
        private SequencedActionPlayer ProjectilePostAnimationPlayer;

        public LaunchProjectilePlayerAnimationManager(Animator playerAnimator, PuzzleCutsceneConfiguration PuzzleCutsceneConfiguration,
            InteractiveObjectContainer InteractiveObjectContainer,
            LaunchProjectileInherentData ProjectileInherentData, InteractiveObjectType projectileObject)
        {
            this.PuzzleCutsceneConfiguration = PuzzleCutsceneConfiguration;
            this.InteractiveObjectContainer = InteractiveObjectContainer;
            this.projectileObjectRef = projectileObject;
            this.playerAnimator = playerAnimator;
            this.ProjectileInherentData = ProjectileInherentData;

            this.ProjectilePreAnimationPlayer = new SequencedActionPlayer(PuzzleCutsceneConfiguration.ConfigurationInherentData[ProjectileInherentData.PreActionAnimationV2].PuzzleCutsceneGraph.GetRootActions(),
                     new PuzzleCutsceneActionInput(InteractiveObjectContainer,
                        PuzzleCutsceneActionInput.Build_GENERIC_AnimationWithFollowObject_Animation(BipedBoneRetriever.GetPlayerBone(BipedBone.RIGHT_HAND_CONTEXT, this.playerAnimator).transform,
                                   this.projectileObjectRef.gameObject, this.ProjectileInherentData.PreActionAnimation)));
            this.ProjectilePreAnimationPlayer.Play();
        }

        public bool LaunchProjectileAnimationPlaying()
        {
            return this.ProjectilePostAnimationPlayer != null && this.ProjectilePostAnimationPlayer.IsPlaying();
        }

        #region External Events
        public void PlayThrowProjectileAnimation(Action onAnimationEnd)
        {
            this.ProjectilePreAnimationPlayer.Kill();
            if (this.ProjectileInherentData.PostActionAnimation != AnimationID.NONE)
            {
                this.ProjectilePostAnimationPlayer = new SequencedActionPlayer(
                    PuzzleCutsceneConfiguration.ConfigurationInherentData[ProjectileInherentData.PreActionAnimationV2].PuzzleCutsceneGraph.GetRootActions(),
                    new PuzzleCutsceneActionInput(this.InteractiveObjectContainer, PuzzleCutsceneActionInput.Build_GENERIC_AnimationWithFollowObject_Animation(
                        BipedBoneRetriever.GetPlayerBone(BipedBone.RIGHT_HAND_CONTEXT, this.playerAnimator).transform, this.projectileObjectRef.gameObject, this.ProjectileInherentData.PostActionAnimation)),
                    onFinished: onAnimationEnd
                    );
                this.ProjectilePostAnimationPlayer.Play();
            }
            else
            {
                onAnimationEnd.Invoke();
            }

        }
        #endregion

        public void Tick(float d)
        {
            this.ProjectilePreAnimationPlayer.Tick(d);

            if (this.ProjectilePostAnimationPlayer != null)
            {
                this.ProjectilePostAnimationPlayer.Tick(d);
            }
        }

        public void OnExit()
        {
            this.ProjectilePreAnimationPlayer.Kill();
            if (this.ProjectilePostAnimationPlayer != null)
            {
                this.ProjectilePostAnimationPlayer.Kill();
            }
        }
    }

    class LaunchProjectilePathAnimationManager
    {
        #region External Dependencies
        private PlayerManagerDataRetriever PlayerManagerDataRetriever;
        private LaunchProjectileRayPositionerManager LaunchProjectileRayPositionerManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private DottedLine ProjectilePath;

        public LaunchProjectilePathAnimationManager(PlayerManagerDataRetriever playerManagerDataRetriever, LaunchProjectileRayPositionerManager launchProjectileRayPositionerManager,
                            PuzzleGameConfigurationManager puzzleGameConfigurationManager)
        {
            #region External Dependencies
            PlayerManagerDataRetriever = playerManagerDataRetriever;
            LaunchProjectileRayPositionerManager = launchProjectileRayPositionerManager;
            PuzzleGameConfigurationManager = puzzleGameConfigurationManager;
            #endregion

            this.ProjectilePath = DottedLine.CreateInstance(DottedLineID.PROJECTILE_POSITIONING, PuzzleGameConfigurationManager);
        }

        public void Tick(float d)
        {
            if (this.ProjectilePath != null)
            {
                this.ProjectilePath.Tick(d, this.PlayerManagerDataRetriever.GetPlayerPuzzleLogicRootCollier().bounds.center, this.LaunchProjectileRayPositionerManager.GetCurrentCursorWorldPosition());
            }
        }


        public void OnThrowProjectileCursorOnProjectileRange()
        {
            if (this.ProjectilePath == null)
            {
                this.ProjectilePath = DottedLine.CreateInstance(DottedLineID.PROJECTILE_POSITIONING, PuzzleGameConfigurationManager);
            }
        }

        public void OnThrowProjectileCursorOutOfProjectileRange()
        {
            this.ProjectilePath.DestroyInstance();
        }

        public void OnExit()
        {
            this.ProjectilePath.DestroyInstance();
        }
    }
    #endregion

    #region Player orientation manager
    class PlayerOrientationManager
    {
        private Rigidbody rigidbody;

        public PlayerOrientationManager(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }

        public void OnLaunchProjectileSpawn(Vector3 targetProjectileWorldPosition)
        {
            var targetProjetilePlayerRelativeDirection = Vector3.ProjectOnPlane((targetProjectileWorldPosition - this.rigidbody.position).normalized, this.rigidbody.transform.up);
            this.rigidbody.rotation = Quaternion.LookRotation(targetProjetilePlayerRelativeDirection, this.rigidbody.transform.up);
        }
    }
    #endregion

    #region Projectile instanciation helper
    public class ProjectileActionInstanciationHelper
    {
        public static InteractiveObjectType CreateProjectileAtStart(LaunchProjectileInherentData ProjectileInherentData, InteractiveObjectTypeDefinitionInherentData InteractiveObjectTypeDefinitionInherentData,
            InteractiveObjectContainer interactiveObjectContainer, PuzzlePrefabConfiguration puzzlePrefabConfiguration, PuzzleGameConfiguration puzzleGameConfiguration)
        {
            return InteractiveObjectType.Instantiate(InteractiveObjectTypeDefinitionInherentData, new InteractiveObjectInitializationObject() { LaunchProjectileInherentData = ProjectileInherentData },
                puzzlePrefabConfiguration, puzzleGameConfiguration, interactiveObjectContainer.transform, new List<Type>() {
                    typeof(ModelObjectModule)
                });
        }

        public static void OnProjectileSpawn(ref InteractiveObjectType projectileObjectRef, BeziersControlPoints throwProjectilePath, LaunchProjectileInherentData ProjectileInherentData)
        {
            projectileObjectRef.EnableModule(typeof(LaunchProjectileModule), new InteractiveObjectInitializationObject() { ProjectilePath = throwProjectilePath, LaunchProjectileInherentData = ProjectileInherentData });
        }
    }
    #endregion
}

