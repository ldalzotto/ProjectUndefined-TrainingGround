using System;
using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectileAction : RTPPlayerAction
    {
        public override SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId => SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG;
        private LaunchProjectileId LaunchProjectileId;

        public LaunchProjectileAction(LaunchProjectileId launchProjectileId, float coolDownTime) : base(coolDownTime)
        {
            LaunchProjectileId = launchProjectileId;
        }

        #region External Dependencies
        private PuzzleEventsManager PuzzleEventsManager;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;
        #endregion

        private LaunchProjectileScreenPositionManager LaunchProjectileScreenPositionManager;
        private LaunchProjectileRayPositionerManager LaunchProjectileRayPositionerManager;
        private ThrowProjectileManager ThrowProjectileManager;
        private LauncheProjectileActionExitManager LauncheProjectileActionExitManager;
        private LaunchProjectilePathAnimationmanager LaunchProjectilePathAnimationmanager;

        private bool isActionFinished = false;

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
            var PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
            var camera = Camera.main;
            var launchProjectileEventManager = GameObject.FindObjectOfType<LaunchProjectileEventManager>();
            PuzzleEventsManager = GameObject.FindObjectOfType<PuzzleEventsManager>();
            PuzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            var canvas = GameObject.FindGameObjectWithTag(TagConstants.CANVAS_TAG).GetComponent<Canvas>();
            #endregion

            var playerTransform = PlayerManagerDataRetriever.GetPlayerTransform();
            var playerTransformScreen = camera.WorldToScreenPoint(playerTransform.position);
            playerTransformScreen.y = camera.pixelHeight - playerTransformScreen.y;
            var configuration = GameObject.FindObjectOfType<PlayerActionConfigurationManager>();

            LaunchProjectileScreenPositionManager = new LaunchProjectileScreenPositionManager(configuration.LaunchProjectileScreenPositionManagerComponent,
               playerTransformScreen, gameInputManager, canvas);
            LaunchProjectileRayPositionerManager = new LaunchProjectileRayPositionerManager(camera, configuration.LaunchProjectileRayPositionerManagerComponent, PlayerManagerDataRetriever,
                LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition, PuzzleEventsManager);
            ThrowProjectileManager = new ThrowProjectileManager(this, gameInputManager, launchProjectileEventManager, canvas, PuzzleGameConfigurationManager);
            LauncheProjectileActionExitManager = new LauncheProjectileActionExitManager(gameInputManager, this);
            LaunchProjectilePathAnimationmanager = new LaunchProjectilePathAnimationmanager(PlayerManagerDataRetriever.GetPlayerCollider());


            PuzzleEventsManager.OnThrowProjectileActionStart(new ThrowProjectileActionStartEvent(playerTransform,
                 configuration.LaunchProjectileRayPositionerManagerComponent.ProjectileThrowRange, LaunchProjectileRayPositionerManager.GetCurrentCursorPosition,
                LaunchProjectileId));
        }

        public override void Tick(float d)
        {
            if (!LauncheProjectileActionExitManager.Tick())
            {
                LaunchProjectileScreenPositionManager.Tick(d);
                LaunchProjectileRayPositionerManager.Tick(d, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition);

                if (LaunchProjectileRayPositionerManager.IsCursorPositioned)
                {
                    ThrowProjectileManager.Tick(d, ref LaunchProjectileRayPositionerManager);
                    LaunchProjectilePathAnimationmanager.Tick(d, LaunchProjectileRayPositionerManager.GetCurrentCursorPosition().Value);
                }
            }

        }

        #region External Events
        public void OnThrowProjectileCursorOnProjectileRange()
        {
            LaunchProjectileScreenPositionManager.OnThrowProjectileCursorOnProjectileRange();
        }
        public void OnThrowProjectileCursorOutOfProjectileRange()
        {
            LaunchProjectileScreenPositionManager.OnThrowProjectileCursorOutOfProjectileRange();
        }
        #endregion

        #region Internal Events
        public void OnExit()
        {
            PuzzleEventsManager.OnProjectileThrowedEvent();
            LaunchProjectileRayPositionerManager.OnExit();
            LaunchProjectilePathAnimationmanager.OnExit();
            LaunchProjectileScreenPositionManager.OnExit();
            isActionFinished = true;
        }

        public void OnLaunchProjectileSpawn()
        {
            if (LaunchProjectileRayPositionerManager.IsCursorPositioned)
            {
                ResetCoolDown();
                var throwPorjectilePath = LaunchProjectilePathAnimationmanager.ThrowProjectilePath;
                ThrowProjectileManager.OnLaunchProjectileSpawn(LaunchProjectileId, throwPorjectilePath);
            }
        }
        #endregion

        public override void GizmoTick()
        {
            if (LaunchProjectilePathAnimationmanager != null)
            {
                LaunchProjectilePathAnimationmanager.GizmoSelectedTick();
            }
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
            currentCursorScreenPosition += (new Vector2(locomotionAxis.x, -locomotionAxis.z) * LaunchProjectileScreenPositionManagerComponent.CursorSpeed * d);
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
        public float CursorSpeed;
    }
    #endregion

    #region Launch projectile ray manager
    class LaunchProjectileRayPositionerManager
    {
        private Camera camera;
        private LaunchProjectileRayPositionerManagerComponent LaunchProjectileRayPositionerManagerComponent;
        private PlayerManagerDataRetriever RTPlayerManagerDataRetriever;
        private PuzzleEventsManager PuzzleEventsManager;

        private Nullable<Vector3> currentCursorWorldPosition;

        private bool isCursorPositioned;
        private bool isCursorInRange;

        public bool IsCursorPositioned { get => isCursorPositioned; }
        public bool IsCursorInRange { get => isCursorInRange; }

        public LaunchProjectileRayPositionerManager(Camera camera, LaunchProjectileRayPositionerManagerComponent launchProjectileRayPositionerManagerComponent,
            PlayerManagerDataRetriever rTPlayerManagerDataRetriever,
            Vector2 cursorScreenPositionAtInit, PuzzleEventsManager PuzzleEventsManager)
        {
            this.camera = camera;
            this.PuzzleEventsManager = PuzzleEventsManager;
            LaunchProjectileRayPositionerManagerComponent = launchProjectileRayPositionerManagerComponent;
            RTPlayerManagerDataRetriever = rTPlayerManagerDataRetriever;
            Tick(0f, cursorScreenPositionAtInit);
        }

        public void Tick(float d, Vector2 currentScreenPositionPoint)
        {
            currentCursorWorldPosition = null;
            Ray ray = camera.ScreenPointToRay(new Vector2(currentScreenPositionPoint.x, camera.pixelHeight - currentScreenPositionPoint.y));
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER)))
            {
                if (!isCursorPositioned)
                {
                    PuzzleEventsManager.OnThrowProjectileCursorAvailable();
                }
                isCursorPositioned = true;
                currentCursorWorldPosition = hit.point;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);

                if (Vector3.Distance(RTPlayerManagerDataRetriever.GetPlayerTransform().position, currentCursorWorldPosition.Value) > LaunchProjectileRayPositionerManagerComponent.ProjectileThrowRange)
                {
                    SetIsCursorInRange(false);
                }
                else
                {
                    SetIsCursorInRange(true);
                }

            }
            else
            {
                if (isCursorPositioned)
                {
                    PuzzleEventsManager.OnThrowProjectileCursorNotAvailable();
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
                    this.PuzzleEventsManager.OnThrowProjectileCursorOnProjectileRange();
                }
            }
            else
            {
                if (this.isCursorInRange)
                {
                    this.PuzzleEventsManager.OnThrowProjectileCursorOutOfProjectileRange();
                }
            }

            this.isCursorInRange = currentFrameIsCursorInRange;
        }

        public void OnExit()
        {

        }

        public Nullable<Vector3> GetCurrentCursorPosition()
        {
            return currentCursorWorldPosition;
        }

    }

    [System.Serializable]
    public class LaunchProjectileRayPositionerManagerComponent
    {
        public float ProjectileThrowRange;
    }
    #endregion

    #region Throw Projectile Manager
    class ThrowProjectileManager
    {
        private LaunchProjectileAction LaunchProjectileRTPActionRef;
        private GameInputManager GameInputManager;
        private LaunchProjectileEventManager LaunchProjectileEventManager;
        private Canvas gameCanvas;
        private PuzzleGameConfigurationManager PuzzleGameConfigurationManager;

        public ThrowProjectileManager(LaunchProjectileAction launchProjectileRTPActionRef, GameInputManager gameInputManager, LaunchProjectileEventManager LaunchProjectileEventManager,
            Canvas gameCanvas, PuzzleGameConfigurationManager PuzzleGameConfigurationManager)
        {
            LaunchProjectileRTPActionRef = launchProjectileRTPActionRef;
            GameInputManager = gameInputManager;
            this.LaunchProjectileEventManager = LaunchProjectileEventManager;
            this.gameCanvas = gameCanvas;
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

        public void OnLaunchProjectileSpawn(LaunchProjectileId launchProjectileId, ThrowProjectilePath throwProjectilePath)
        {
            currentProjectile = LaunchProjectile.Instantiate(PuzzleGameConfigurationManager.ProjectileConf()[launchProjectileId], throwProjectilePath.BeziersControlPoints, this.gameCanvas);
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

    #region Launch Projectile Path Animation Manager 
    class LaunchProjectilePathAnimationmanager
    {
        private ThrowProjectilePath throwProjectilePath;

        public ThrowProjectilePath ThrowProjectilePath { get => throwProjectilePath; }

        public LaunchProjectilePathAnimationmanager(Collider throwerCollider)
        {
            throwProjectilePath = MonoBehaviour.Instantiate(PrefabContainer.Instance.ThrowProjectilePathPrefab, Vector3.zero, Quaternion.identity);
            throwProjectilePath.Init(throwerCollider);
        }

        public void Tick(float d, Vector3 currentCursorPosition)
        {
            throwProjectilePath.Tick(d, currentCursorPosition);
        }

        public void GizmoSelectedTick()
        {
            throwProjectilePath.GizmoSelectedTick();
        }

        public void OnExit()
        {
            MonoBehaviour.Destroy(throwProjectilePath.gameObject);
        }

        public void OnLaunchProjectileSpawn()
        {

        }
    }
    #endregion
}

