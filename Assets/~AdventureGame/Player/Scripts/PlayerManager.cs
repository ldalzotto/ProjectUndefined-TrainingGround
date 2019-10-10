using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace AdventureGame
{

    public class PlayerManager : PlayerManagerType, IVisualMovementPermission
    {

        #region Player Common component
        private PlayerCommonComponents PlayerCommonComponents;
        private DataComponentContainer PlayerDataComponentContainer;
        #endregion

        #region Player POI selection manager
        private PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager;
        #endregion

        #region Animation Managers
        private PlayerAnimationManager PlayerAnimationManager;
        #endregion

        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;

        private PlayerInputMoveManager PlayerInputMoveManager;
        //Inter dependency
        private PointOfInterestType PointOfInterestType;

        private PointOfInterestTrackerModule pointOfInterestTrackerModule;
        private PlayerPOIWheelTriggerManager PlayerPOIWheelTriggerManager;

        private PlayerContextActionManager PlayerContextActionManager;
        private PlayerInventoryTriggerManager PlayerInventoryTriggerManager;

        public void Init()
        {
            #region External dependencies
            GameInputManager GameInputManager = CoreGameSingletonInstances.GameInputManager;
            GameObject CameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            ContextActionWheelEventManager ContextActionWheelEventManager = AdventureGameSingletonInstances.ContextActionWheelEventManager;
            InventoryEventManager inventoryEventManager = AdventureGameSingletonInstances.InventoryEventManager;
            var coreConfigurationManager = CoreGameSingletonInstances.CoreConfigurationManager;
            this.PlayerPointOfInterestSelectionManager = this.GetComponent<PlayerPointOfInterestSelectionManager>();
            #endregion

            #region Load Persisted Position
            var playerPosition = CoreGameSingletonInstances.PlayerAdventurePositionManager.PlayerPositionBeforeLevelLoad;
            if (playerPosition != null)
            {
                this.transform.position = playerPosition.GetPosition();
                this.transform.rotation = playerPosition.GetQuaternion();
            }
            #endregion

            this.PointOfInterestType = GetComponentInChildren<PointOfInterestType>();
            GameObject playerObject = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG);
            Animator playerAnimator = GetComponentInChildren<Animator>();
            Rigidbody playerRigidBody = GetComponent<Rigidbody>();
            Collider playerCollider = playerRigidBody.GetComponent<Collider>();
            NavMeshAgent playerAgent = GetComponent<NavMeshAgent>();
            playerAgent.updatePosition = false;
            playerAgent.updateRotation = false;

            this.PlayerCommonComponents = GetComponentInChildren<PlayerCommonComponents>();

            this.PlayerDataComponentContainer = GetComponentInChildren<DataComponentContainer>();
            this.PlayerDataComponentContainer.Init();

            #region Data Components
            var TransformMoveManagerComponentV3 = AdventureGameSingletonInstances.AdventureStaticConfigurationContainer.AdventureStaticConfiguration.AdventurePlayerMovementConfiguration.PlayerTransformMoveComponent;
            var PlayerPhysicsMovementComponent = this.PlayerDataComponentContainer.GetDataComponent<PlayerPhysicsMovementComponent>();
            #endregion

            #region POI Modules
            this.pointOfInterestTrackerModule = this.PointOfInterestType.GetPointOfInterestTrackerModule();
            #endregion
            
            this.PlayerPointOfInterestSelectionManager.AdventureInit(CoreGameSingletonInstances.GameInputManager, this.pointOfInterestTrackerModule);

            this.PlayerInputMoveManager = new PlayerInputMoveManager(TransformMoveManagerComponentV3.SpeedMultiplicationFactor, CameraPivotPoint.transform, GameInputManager, playerRigidBody);
            this.PlayerPOIWheelTriggerManager = new PlayerPOIWheelTriggerManager(playerObject.transform, GameInputManager, ContextActionWheelEventManager, this.pointOfInterestTrackerModule);
            this.PlayerContextActionManager = new PlayerContextActionManager();
            this.PlayerInventoryTriggerManager = new PlayerInventoryTriggerManager(GameInputManager, inventoryEventManager);
            this.PlayerAnimationManager = GetComponent<PlayerAnimationManager>();
            this.PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(playerRigidBody, playerCollider, PlayerPhysicsMovementComponent.MinimumDistanceToStick);
        }

        public void Tick(float d)
        {
            var playerSpeedMagnitude = 0f;

            if (!this.PointOfInterestType.IsDirectedByCutscene())
            {
                if (IsAllowedToMove())
                {
                    PlayerInputMoveManager.Tick(d);
                }
                else
                {
                    PlayerInputMoveManager.ResetSpeed();
                }
                playerSpeedMagnitude = PlayerInputMoveManager.PlayerSpeedProcessingInput.PlayerSpeedMagnitude;
            }
            else
            {
                playerSpeedMagnitude = this.PointOfInterestType.GetPointOfInterestCutsceneController().GetCurrentNormalizedSpeedMagnitude();
            }

            pointOfInterestTrackerModule.Tick(d);

            if (IsAllowedToDoAnyInteractions())
            {

                //if statement to avoid processing inpout at the same frame
                if (!PlayerInventoryTriggerManager.Tick())
                {
                    PlayerPOIWheelTriggerManager.Tick(d, this.PlayerPointOfInterestSelectionManager.GetCurrentSelectedPOI());
                }
            }

            PlayerAnimationManager.PlayerAnimationDataManager.Tick(playerSpeedMagnitude);
        }

        public void FixedTick(float d)
        {
            if (!this.PointOfInterestType.IsDirectedByCutscene())
            {
                this.PlayerInputMoveManager.FixedTick(d);
                //Physics is desabled when cutscene is playing to avoid conflicts with nav mesh agent
                this.PlayerBodyPhysicsEnvironment.FixedTick(d);
            }

        }

        public void LateTick(float d)
        {
        }

        public void OnGizmoTick()
        {
            // PlayerPOIVisualHeadMovementManager.GizmoTick();
        }

        #region Logical Conditions
        private bool IsAllowedToMove()
        {
            return !PlayerContextActionManager.IsActionExecuting && !PlayerPOIWheelTriggerManager.WheelEnabled && !PlayerInventoryTriggerManager.IsInventoryDisplayed;
        }

        private bool IsAllowedToDoAnyInteractions()
        {
            return this.IsAllowedToMove() && !this.PointOfInterestType.IsDirectedByCutscene();
        }

        public bool IsVisualMovementAllowed()
        {
            return (!PlayerContextActionManager.IsActionExecuting || PlayerContextActionManager.IsTalkActionExecuting)
                        && this.PointOfInterestType.IsVisualMovementAllowed();
        }

        #endregion

        #region External Events
        public void OnContextActionAdded(AContextAction contextActionAdded)
        {
            PlayerContextActionManager.OnContextActionAdded(contextActionAdded);
        }
        public void OnContextActionFinished()
        {
            PlayerContextActionManager.OnContextActionFinished();
        }
        public void OnWheelDisabled()
        {
            PlayerPOIWheelTriggerManager.OnWheelDisabled();
        }
        public void OnInventoryEnabled()
        {
            PlayerInventoryTriggerManager.OnInventoryEnabled();
        }
        public void OnInventoryDisabled()
        {
            PlayerInventoryTriggerManager.OnInventoryDisabled();
        }

        #endregion

        public Animator GetPlayerAnimator()
        {
            if (PlayerAnimationManager != null)
            {
                return PlayerAnimationManager.GetPlayerAnimator();
            }
            return null;
        }

        public PointOfInterestType GetCurrentTargetedPOI()
        {
            return pointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest();
        }

    }

    #region POI
    class PlayerPOIWheelTriggerManager
    {
        private Transform PlayerTransform;
        private GameInputManager GameInputManager;
        private ContextActionWheelEventManager ContextActionWheelEventManager;
        private PointOfInterestTrackerModule PointOfInterestTrackerModule;

        private bool wheelEnabled;

        public bool WheelEnabled { get => wheelEnabled; }

        public PlayerPOIWheelTriggerManager(Transform playerTransform, GameInputManager gameInputManager, ContextActionWheelEventManager contextActionWheelEventManager, PointOfInterestTrackerModule pointOfInterestTrackerModule)
        {
            PlayerTransform = playerTransform;
            GameInputManager = gameInputManager;
            ContextActionWheelEventManager = contextActionWheelEventManager;
            this.PointOfInterestTrackerModule = pointOfInterestTrackerModule;
        }

        public void Tick(float d, PointOfInterestType selectedPOI)
        {
            if (selectedPOI != null)
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    if (PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest() != null)
                    {
                        Debug.Log(PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest().name);
                        wheelEnabled = true;
                        ContextActionWheelEventManager.OnWheelEnabled(selectedPOI.GetContextActions(), WheelTriggerSource.PLAYER);
                    }
                }
            }
        }

        public void OnWheelDisabled()
        {
            wheelEnabled = false;
        }
    }
    #endregion

    #region Context Actions Handler
    class PlayerContextActionManager
    {
        private bool isActionExecuting;
        private bool isTalkActionExecuting;

        public bool IsActionExecuting { get => isActionExecuting; }
        public bool IsTalkActionExecuting { get => isTalkActionExecuting; }

        public void OnContextActionAdded(AContextAction contextActionAdded)
        {
            isActionExecuting = true;
            isTalkActionExecuting = false;
            if (contextActionAdded.IsTalkAction())
            {
                isTalkActionExecuting = true;
            }
        }

        public void OnContextActionFinished()
        {
            isActionExecuting = false;
            isTalkActionExecuting = false;
        }
    }

    #endregion

    #region Inventory Trigger
    class PlayerInventoryTriggerManager
    {
        private GameInputManager GameInputManager;
        private InventoryEventManager InventoryEventManager;

        private bool isInventoryDisplayed;

        public bool IsInventoryDisplayed { get => isInventoryDisplayed; }

        public PlayerInventoryTriggerManager(GameInputManager gameInputManager, InventoryEventManager inventoryEventManager)
        {
            GameInputManager = gameInputManager;
            InventoryEventManager = inventoryEventManager;
        }

        public bool Tick()
        {
            if (!isInventoryDisplayed)
            {
                if (GameInputManager.CurrentInput.InventoryButtonD())
                {
                    InventoryEventManager.OnInventoryEnabled();
                    return true;
                }
            }
            return false;
        }

        public void OnInventoryEnabled()
        {
            isInventoryDisplayed = true;
        }

        public void OnInventoryDisabled()
        {
            isInventoryDisplayed = false;
        }
    }
    #endregion


}