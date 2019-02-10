using System;
using UnityEngine;

namespace RTPuzzle
{
    public class LaunchProjectileRTPAction : RTPPlayerAction
    {
        public override SelectionWheelNodeConfigurationId ActionWheelNodeConfigurationId => SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG;

        #region External Dependencies
        private GroundEffectsManager GroundEffectsEventManager;
        #endregion

        private LaunchProjectileScreenPositionManager LaunchProjectileScreenPositionManager;
        private LaunchProjectileRayPositionerManager LaunchProjectileRayPositionerManager;
        private ThrowProjectileManager ThrowProjectileManager;
        private LauncheProjectileActionExitManager LauncheProjectileActionExitManager;

        private bool isActionFinished = false;

        public override bool FinishedCondition()
        {
            return isActionFinished;
        }

        public override void FirstExecution()
        {
            isActionFinished = false;

            #region External Dependencies
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            var rTPlayerManagerDataRetriever = GameObject.FindObjectOfType<RTPlayerManagerDataRetriever>();
            var camera = Camera.main;
            var launchProjectileContainer = GameObject.FindObjectOfType<LaunchProjectileContainerManager>();
            GroundEffectsEventManager = GameObject.FindObjectOfType<GroundEffectsManager>();
            #endregion

            var playerTransform = rTPlayerManagerDataRetriever.GetPlayerTransform();
            var configuration = GameObject.FindObjectOfType<RTPlayerActionConfigurationManager>();

            GroundEffectsEventManager.OnThrowProjectileActionStart(playerTransform, configuration.LaunchProjectileRayPositionerManagerComponent.ProjectileThrowRange);

            LaunchProjectileScreenPositionManager = new LaunchProjectileScreenPositionManager(configuration.LaunchProjectileScreenPositionManagerComponent,
                camera.WorldToScreenPoint(playerTransform.position), gameInputManager);
            LaunchProjectileRayPositionerManager = new LaunchProjectileRayPositionerManager(camera, configuration.LaunchProjectileRayPositionerManagerComponent, rTPlayerManagerDataRetriever);
            ThrowProjectileManager = new ThrowProjectileManager(this, gameInputManager, launchProjectileContainer);
            LauncheProjectileActionExitManager = new LauncheProjectileActionExitManager(gameInputManager, this);
        }

        public override void Tick(float d)
        {
            if (!LauncheProjectileActionExitManager.Tick())
            {
                LaunchProjectileScreenPositionManager.Tick(d);
                LaunchProjectileRayPositionerManager.Tick(d, LaunchProjectileScreenPositionManager.CurrentCursorScreenPosition);

                var currentCursorPosition = LaunchProjectileRayPositionerManager.GetCurrentCursorPosition();
                if (currentCursorPosition.HasValue)
                {
                    ThrowProjectileManager.Tick(d, currentCursorPosition.Value);
                }
            }

        }

        #region Internal Events
        public void OnExit()
        {
            GroundEffectsEventManager.OnThrowProjectileThrowed();
            LaunchProjectileRayPositionerManager.OnExit();
            isActionFinished = true;
        }
        #endregion

        public override void GizmoTick()
        {
        }

        public override void GUITick()
        {
            LaunchProjectileScreenPositionManager.GUITick();
        }
    }

    #region Screen Position Manager
    class LaunchProjectileScreenPositionManager
    {
        private LaunchProjectileScreenPositionManagerComponent LaunchProjectileScreenPositionManagerComponent;
        private GameInputManager GameInputManager;

        private Vector2 currentCursorScreenPosition;

        public LaunchProjectileScreenPositionManager(LaunchProjectileScreenPositionManagerComponent launchProjectileScreenPositionManagerComponent,
            Vector2 currentCursorScreenPosition, GameInputManager GameInputManager)
        {
            LaunchProjectileScreenPositionManagerComponent = launchProjectileScreenPositionManagerComponent;
            this.currentCursorScreenPosition = currentCursorScreenPosition;
            this.GameInputManager = GameInputManager;

        }

        public Vector2 CurrentCursorScreenPosition { get => currentCursorScreenPosition; }

        public void Tick(float d)
        {
            var locomotionAxis = GameInputManager.CurrentInput.LocomotionAxis();
            currentCursorScreenPosition += (new Vector2(locomotionAxis.x, -locomotionAxis.z) * LaunchProjectileScreenPositionManagerComponent.CursorSpeed * d);
        }

        public void GUITick()
        {
            GUI.DrawTexture(new Rect(currentCursorScreenPosition - new Vector2(10, 10), new Vector2(20, 20)), LaunchProjectileScreenPositionManagerComponent.DebugTexture);
        }


    }

    [System.Serializable]
    public class LaunchProjectileScreenPositionManagerComponent
    {
        public float CursorSpeed;
        public Texture DebugTexture;
    }
    #endregion

    #region Launch projectile ray manager
    class LaunchProjectileRayPositionerManager
    {
        private Camera camera;
        private LaunchProjectileRayPositionerManagerComponent LaunchProjectileRayPositionerManagerComponent;
        private RTPlayerManagerDataRetriever RTPlayerManagerDataRetriever;

        private GameObject currentCursor;
        private MeshRenderer currentCursorMeshRenderer;
        private Material currentCursorInitialMaterial;

        public LaunchProjectileRayPositionerManager(Camera camera, LaunchProjectileRayPositionerManagerComponent launchProjectileRayPositionerManagerComponent, RTPlayerManagerDataRetriever rTPlayerManagerDataRetriever)
        {
            this.camera = camera;
            LaunchProjectileRayPositionerManagerComponent = launchProjectileRayPositionerManagerComponent;
            RTPlayerManagerDataRetriever = rTPlayerManagerDataRetriever;
        }

        public void Tick(float d, Vector2 currentScreenPositionPoint)
        {
            Ray ray = camera.ScreenPointToRay(new Vector2(currentScreenPositionPoint.x, camera.pixelHeight - currentScreenPositionPoint.y));
            RaycastHit hit;
            if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.green);
                if (currentCursor == null)
                {
                    currentCursor = MonoBehaviour.Instantiate(LaunchProjectileRayPositionerManagerComponent.ProjectileCursor);
                    currentCursorMeshRenderer = currentCursor.GetComponent<MeshRenderer>();
                    currentCursorInitialMaterial = currentCursorMeshRenderer.material;
                }

                currentCursor.transform.position = hit.point;
                currentCursor.transform.up = hit.normal;

                if (Vector3.Distance(RTPlayerManagerDataRetriever.GetPlayerTransform().position, currentCursor.transform.position) > LaunchProjectileRayPositionerManagerComponent.ProjectileThrowRange)
                {
                    currentCursorMeshRenderer.material = MaterialContainer.Instance.LaunchProjectileUnavailableMaterial;
                }
                else
                {
                    currentCursorMeshRenderer.material = currentCursorInitialMaterial;
                }

            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * Mathf.Infinity, Color.red);
                if (currentCursor != null)
                {
                    MonoBehaviour.Destroy(currentCursor);
                }
            }
        }

        public void OnExit()
        {
            if (currentCursor != null)
            {
                MonoBehaviour.Destroy(currentCursor);
            }
        }

        public Nullable<Vector3> GetCurrentCursorPosition()
        {
            if (currentCursor == null)
            {
                return null;
            }
            return currentCursor.transform.position;
        }

    }

    [System.Serializable]
    public class LaunchProjectileRayPositionerManagerComponent
    {
        public GameObject ProjectileCursor;
        public float ProjectileThrowRange;
    }
    #endregion

    #region Throw Projectile Manager
    class ThrowProjectileManager
    {
        private LaunchProjectileRTPAction LaunchProjectileRTPActionRef;
        private GameInputManager GameInputManager;
        private LaunchProjectileContainerManager LaunchProjectileContainerManager;

        public ThrowProjectileManager(LaunchProjectileRTPAction launchProjectileRTPActionRef, GameInputManager gameInputManager, LaunchProjectileContainerManager LaunchProjectileContainerManager)
        {
            LaunchProjectileRTPActionRef = launchProjectileRTPActionRef;
            GameInputManager = gameInputManager;
            this.LaunchProjectileContainerManager = LaunchProjectileContainerManager;
        }

        private LaunchProjectile currentProjectile;

        public void Tick(float d, Vector3 cursorPosition)
        {
            if (GameInputManager.CurrentInput.ActionButtonD())
            {
                currentProjectile = MonoBehaviour.Instantiate(PrefabContainer.Instance.ProjectilePrefab, cursorPosition, Quaternion.identity);
                LaunchProjectileContainerManager.OnLaunchProjectileSpawn(currentProjectile);
            }
        }

    }
    #endregion

    #region Launch Projectile Exit Action
    class LauncheProjectileActionExitManager
    {

        private GameInputManager GameInputManager;
        private LaunchProjectileRTPAction LaunchProjectileRTPAction;
        public LauncheProjectileActionExitManager(GameInputManager gameInputManager, LaunchProjectileRTPAction LaunchProjectileRTPActionRef)
        {
            GameInputManager = gameInputManager;
            this.LaunchProjectileRTPAction = LaunchProjectileRTPActionRef;
        }

        public bool Tick()
        {
            if (IsExitRequested())
            {
                LaunchProjectileRTPAction.OnExit();
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
}

