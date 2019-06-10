using CoreGame;
using System;
using UnityEngine;

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

        private SphereRangeType projectileSphereRange;

        private bool isActionFinished = false;

        public SphereRangeType ProjectileSphereRange { get => projectileSphereRange; }

        public override bool FinishedCondition()
        {
            return isActionFinished;
        }

        public override void FirstExecution()
        {
            base.FirstExecution();
            isActionFinished = false;

            #region External Dependencies
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            this.PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var camera = Camera.main;
            var launchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
            PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var PuzzleStaticConfigurationContainer = GameObject.FindObjectOfType<PuzzleStaticConfigurationContainer>();
            var canvas = GameObject.FindGameObjectWithTag(TagConstants.CANVAS_TAG).GetComponent<Canvas>();
            var launchProjectileContainerManager = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            #endregion

            var playerTransform = PlayerManagerDataRetriever.GetPlayerTransform();
            var playerTransformScreen = camera.WorldToScreenPoint(playerTransform.position);
            playerTransformScreen.y = camera.pixelHeight - playerTransformScreen.y;
            var configuration = GameObject.FindObjectOfType<PlayerActionConfigurationManager>();

            var projectileInherentData = PuzzleGameConfigurationManager.ProjectileConf()[((LaunchProjectileActionInherentData)this.playerActionInherentData).launchProjectileId];
            this.projectileSphereRange = SphereRangeType.Instanciate(RangeTypeID.LAUNCH_PROJECTILE, projectileInherentData.ProjectileThrowRange, PlayerManagerDataRetriever.GetPlayerWorldPosition);


            LaunchProjectileScreenPositionManager = new LaunchProjectileScreenPositionManager(configuration.LaunchProjectileScreenPositionManagerComponent,
               playerTransformScreen, gameInputManager, canvas);
            LaunchProjectileRayPositionerManager = new LaunchProjectileRayPositionerManager(camera, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition, this, PuzzleEventsManager, PuzzleStaticConfigurationContainer,
                         projectileInherentData);
            LaunchProjectilePathAnimationManager = new LaunchProjectilePathAnimationManager(PlayerManagerDataRetriever, LaunchProjectileRayPositionerManager, PuzzleGameConfigurationManager);
            ThrowProjectileManager = new ThrowProjectileManager(this, gameInputManager, launchProjectileEventManager, launchProjectileContainerManager, PuzzleGameConfigurationManager);
            LauncheProjectileActionExitManager = new LauncheProjectileActionExitManager(gameInputManager, this);
            LaunchProjectilePlayerAnimationManager = new LaunchProjectilePlayerAnimationManager(PlayerManagerDataRetriever.GetPlayerAnimator(), projectileInherentData);
            PlayerOrientationManager = new PlayerOrientationManager(PlayerManagerDataRetriever.GetPlayerRigidBody());

            LaunchProjectileRayPositionerManager.Tick(0f, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition);
        }
        

        public override void Tick(float d)
        {
            LaunchProjectilePlayerAnimationManager.Tick(d);

            //If launch animation is not playng (animation exit callback is exiting the action)
            if (!this.LaunchProjectilePlayerAnimationManager.LaunchProjectileAnimationPlaying())
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

        #region External Events
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

        #region Internal Events
        public void OnExit()
        {
            MonoBehaviour.Destroy(this.projectileSphereRange.gameObject);
            // PuzzleEventsManager.PZ_EVT_ThrowProjectile_PlayerAction_End();
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
                PlayerOrientationManager.OnLaunchProjectileSpawn(LaunchProjectileRayPositionerManager.GetCurrentCursorWorldPosition());
                this.LaunchProjectilePlayerAnimationManager.PlayThrowProjectileAnimation(
                    onAnimationEnd: () =>
                    {
                        //LaunchProjectileRayPositionerManager.GetCurrentCursorWorldPosition()
                        ResetCoolDown();
                        var throwPorjectilePath = BeziersControlPoints.Build(this.PlayerManagerDataRetriever.GetPlayerCollider().bounds.center, LaunchProjectileRayPositionerManager.GetCurrentCursorWorldPosition(),
                                             this.PlayerManagerDataRetriever.GetPlayerCollider().transform.up, BeziersControlPointsShape.CURVED);
                        ThrowProjectileManager.OnLaunchProjectileSpawn(((LaunchProjectileActionInherentData)this.playerActionInherentData).launchProjectileId, throwPorjectilePath);
                    }
                   );
            }
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
        private LaunchProjectileScreenPositionManagerComponent LaunchProjectileScreenPositionManagerComponent;
        private GameInputManager GameInputManager;

        private ThrowProjectileCursorType cursorObject;
        private Vector2 currentCursorScreenPosition;

        public LaunchProjectileScreenPositionManager(LaunchProjectileScreenPositionManagerComponent launchProjectileScreenPositionManagerComponent,
            Vector2 currentCursorScreenPosition, GameInputManager GameInputManager, Canvas gameCanvas)
        {
            LaunchProjectileScreenPositionManagerComponent = launchProjectileScreenPositionManagerComponent;
            this.currentCursorScreenPosition = currentCursorScreenPosition;
            this.GameInputManager = GameInputManager;
            this.cursorObject = ThrowProjectileCursorType.Instanciate(gameCanvas.transform);
            UpdateCursorPosition();
        }

        public Vector2 CurrentCursorScreenPosition { get => currentCursorScreenPosition; }

        public void Tick(float d)
        {
            var locomotionAxis = GameInputManager.CurrentInput.LocomotionAxis();
            currentCursorScreenPosition += (new Vector2(locomotionAxis.x, -locomotionAxis.z) * Screen.width * LaunchProjectileScreenPositionManagerComponent.CursorSpeedScreenWidthPerCent * d);
            UpdateCursorPosition();
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
        private Camera camera;
        private PuzzleEventsManager PuzzleEventsManager;
        private ProjectileInherentData projectileInherentData;
        private PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer;
        private LaunchProjectileAction launchProjectileActionRef;
        private SphereRangeType projectileCursorRange;

        private Vector3 currentCursorWorldPosition;

        private bool isCursorPositioned;
        private bool isCursorInRange;

        public bool IsCursorPositioned { get => isCursorPositioned; }
        public bool IsCursorInRange { get => isCursorInRange; }

        public Vector3 GetCurrentCursorWorldPosition()
        {
            return this.currentCursorWorldPosition;
        }

        public LaunchProjectileRayPositionerManager(Camera camera, Vector2 cursorScreenPositionAtInit, LaunchProjectileAction launchProjectileAction,
                PuzzleEventsManager PuzzleEventsManager, PuzzleStaticConfigurationContainer PuzzleStaticConfigurationContainer, ProjectileInherentData projectileInherentData)
        {
            this.camera = camera;
            this.launchProjectileActionRef = launchProjectileAction;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.PuzzleStaticConfigurationContainer = PuzzleStaticConfigurationContainer;
            this.projectileInherentData = projectileInherentData;
        }

        public void Tick(float d, Vector2 currentScreenPositionPoint)
        {
            Ray ray = camera.ScreenPointToRay(new Vector2(currentScreenPositionPoint.x, camera.pixelHeight - currentScreenPositionPoint.y));
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER)))
            {
                if (!isCursorPositioned)
                {
                    if (this.projectileCursorRange != null)
                    {
                        MonoBehaviour.DestroyImmediate(this.projectileCursorRange.gameObject);
                    }
                    this.projectileCursorRange = SphereRangeType.Instanciate(RangeTypeID.LAUNCH_PROJECTILE_CURSOR, this.projectileInherentData.EffectRange, this.GetCurrentCursorWorldPosition, this.GetLaunchProjectileRangeActiveColor);
                }
                isCursorPositioned = true;
                currentCursorWorldPosition = hit.point;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

                if (this.launchProjectileActionRef.ProjectileSphereRange.IsInside(currentCursorWorldPosition))
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
                    MonoBehaviour.Destroy(this.projectileCursorRange.gameObject);
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
                    this.PuzzleEventsManager.PZ_EVT_ThrowProjectileCursor_OnProjectileRange();
                }
            }
            else
            {
                if (this.isCursorInRange)
                {
                    this.PuzzleEventsManager.PZ_EVT_ThrowProjectileCursor_OutOfProjectileRange();
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
            if (this.projectileCursorRange != null)
            {
                MonoBehaviour.Destroy(this.projectileCursorRange.gameObject);
            }
        }

    }
    #endregion

    #region Throw Projectile Manager
    class ThrowProjectileManager
    {
        private LaunchProjectileAction LaunchProjectileRTPActionRef;
        private GameInputManager GameInputManager;
        private LaunchProjectileEventManager LaunchProjectileEventManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        private LaunchProjectileContainerManager launchProjectileContainerManager;

        public ThrowProjectileManager(LaunchProjectileAction launchProjectileRTPActionRef, GameInputManager gameInputManager, LaunchProjectileEventManager LaunchProjectileEventManager,
            LaunchProjectileContainerManager launchProjectileContainerManager, PuzzleGameConfigurationManager PuzzleGameConfigurationManager)
        {
            LaunchProjectileRTPActionRef = launchProjectileRTPActionRef;
            GameInputManager = gameInputManager;
            this.LaunchProjectileEventManager = LaunchProjectileEventManager;
            this.launchProjectileContainerManager = launchProjectileContainerManager;
            this.PuzzleGameConfigurationManager = PuzzleGameConfigurationManager;
        }

        private LaunchProjectile currentProjectile;

        public void Tick(float d, ref LaunchProjectileRayPositionerManager LaunchProjectileRayPositionerManager)
        {
            if (GameInputManager.CurrentInput.ActionButtonD() && LaunchProjectileRayPositionerManager.IsCursorInRange)
            {
                LaunchProjectileRTPActionRef.OnLaunchProjectileSpawn();
            }
        }

        public void OnLaunchProjectileSpawn(LaunchProjectileId launchProjectileId, BeziersControlPoints throwProjectilePath)
        {
            currentProjectile = LaunchProjectile.Instantiate(PuzzleGameConfigurationManager.ProjectileConf()[launchProjectileId], throwProjectilePath, launchProjectileContainerManager.transform);
            LaunchProjectileEventManager.OnLaunchProjectileSpawn(currentProjectile);
            LaunchProjectileRTPActionRef.OnExit();
        }

    }
    #endregion

    #region Launch Projectile Exit Action
    class LauncheProjectileActionExitManager
    {

        private GameInputManager GameInputManager;
        private LaunchProjectileAction LaunchProjectileAction;
        public LauncheProjectileActionExitManager(GameInputManager gameInputManager, LaunchProjectileAction LaunchProjectileActionRef)
        {
            GameInputManager = gameInputManager;
            this.LaunchProjectileAction = LaunchProjectileActionRef;
        }

        public bool Tick()
        {
            if (IsExitRequested())
            {
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
        private Animator playerAnimator;
        private ProjectileInherentData ProjectileInherentData;

        private PlayerAnimationWithObjectManager ProjectileAnimationManager;
        private PlayerAnimationWithObjectManager ProjectileLaunchAnimationManager;

        public LaunchProjectilePlayerAnimationManager(Animator playerAnimator, ProjectileInherentData ProjectileInherentData)
        {
            this.playerAnimator = playerAnimator;
            this.ProjectileInherentData = ProjectileInherentData;

            this.ProjectileAnimationManager = new PlayerAnimationWithObjectManager(MonoBehaviour.Instantiate(ProjectileInherentData.ProjectileModelPrefab), this.playerAnimator, this.ProjectileInherentData.PreActionAnimation, 0f, true,
                onAnimationEndAction: null);
            this.ProjectileAnimationManager.Play();
        }

        public bool LaunchProjectileAnimationPlaying()
        {
            return this.ProjectileLaunchAnimationManager != null && this.ProjectileLaunchAnimationManager.IsPlaying();
        }

        #region External Events
        public void PlayThrowProjectileAnimation(Action onAnimationEnd)
        {
            this.ProjectileAnimationManager.KillSilently();
            #region TO DELETE
            var sph = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MonoBehaviour.Destroy(sph.GetComponent<Collider>());
            #endregion
            this.ProjectileLaunchAnimationManager = new PlayerAnimationWithObjectManager(sph, this.playerAnimator, this.ProjectileInherentData.PostActionAnimation, 0.1f, true,
                onAnimationEndAction: onAnimationEnd);
            this.ProjectileLaunchAnimationManager.Play();
        }
        #endregion

        public void Tick(float d)
        {
            this.ProjectileAnimationManager.Tick(d);

            if (this.ProjectileLaunchAnimationManager != null)
            {
                this.ProjectileLaunchAnimationManager.Tick(d);
            }
        }

        public void OnExit()
        {
            this.ProjectileAnimationManager.Kill();

            /*
            if (this.ProjectileLaunchAnimationManager != null)
            {
                this.ProjectileLaunchAnimationManager.Kill();
            }
            */
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
                this.ProjectilePath.Tick(d, this.PlayerManagerDataRetriever.GetPlayerCollider().bounds.center, this.LaunchProjectileRayPositionerManager.GetCurrentCursorWorldPosition());
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
            //this.rigidbody.MoveRotation(Quaternion.FromToRotation(this.rigidbody.transform.forward, targetProjetilePlayerRelativeDirection));
        }
    }
    #endregion


}

