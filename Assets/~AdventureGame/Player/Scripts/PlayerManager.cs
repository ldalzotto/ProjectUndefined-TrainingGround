using CoreGame;
using UnityEngine;
using UnityEngine.AI;

namespace AdventureGame
{
    public class PlayerManager : PlayerManagerType, IVisualMovementPermission
    {
        #region Animation Managers

        private PlayerAnimationManager PlayerAnimationManager;

        #endregion

        private PlayerBodyPhysicsEnvironment PlayerBodyPhysicsEnvironment;

        private PlayerContextActionManager PlayerContextActionManager;

        private PlayerInputMoveManager PlayerInputMoveManager;
        private PlayerInventoryTriggerManager PlayerInventoryTriggerManager;

        #region Player POI selection manager

        private PlayerPointOfInterestSelectionManager PlayerPointOfInterestSelectionManager = PlayerPointOfInterestSelectionManager.Get();

        #endregion

        private PlayerPOIWheelTriggerManager PlayerPOIWheelTriggerManager;

        private PointOfInterestTrackerModule pointOfInterestTrackerModule;

        //Inter dependency
        private PointOfInterestType PointOfInterestType;

        public void Init()
        {
            #region External dependencies

            var GameInputManager = CoreGameSingletonInstances.GameInputManager;
            var CameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            var ContextActionWheelEventManager = AdventureGameSingletonInstances.ContextActionWheelEventManager;
            var inventoryEventManager = AdventureGameSingletonInstances.InventoryEventManager;
            var coreConfigurationManager = CoreGameSingletonInstances.CoreConfigurationManager;

            #endregion

            #region Load Persisted Position

            var playerPosition = CoreGameSingletonInstances.PlayerAdventurePositionManager.PlayerPositionBeforeLevelLoad;
            if (playerPosition != null)
            {
                transform.position = playerPosition.GetPosition();
                transform.rotation = playerPosition.GetQuaternion();
            }

            #endregion

            PointOfInterestType = GetComponentInChildren<PointOfInterestType>();
            var playerObject = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG);
            var playerAnimator = GetComponentInChildren<Animator>();
            var playerRigidBody = GetComponent<Rigidbody>();
            var playerCollider = playerRigidBody.GetComponent<Collider>();
            var playerAgent = GetComponent<NavMeshAgent>();
            playerAgent.updatePosition = false;
            playerAgent.updateRotation = false;

            PlayerCommonComponents = GetComponentInChildren<PlayerCommonComponents>();

            PlayerDataComponentContainer = GetComponentInChildren<DataComponentContainer>();
            PlayerDataComponentContainer.Init();

            #region Data Components

            var TransformMoveManagerComponentV3 = AdventureGameSingletonInstances.AdventureStaticConfigurationContainer.AdventureStaticConfiguration.AdventurePlayerMovementConfiguration.PlayerTransformMoveComponent;
            var PlayerPhysicsMovementComponent = PlayerDataComponentContainer.GetDataComponent<PlayerPhysicsMovementComponent>();

            #endregion

            #region POI Modules

            pointOfInterestTrackerModule = PointOfInterestType.GetPointOfInterestTrackerModule();

            #endregion

            PlayerPointOfInterestSelectionManager.AdventureInit(CoreGameSingletonInstances.GameInputManager, pointOfInterestTrackerModule);

            PlayerInputMoveManager = new PlayerInputMoveManager(TransformMoveManagerComponentV3.SpeedMultiplicationFactor, CameraPivotPoint.transform, GameInputManager, playerRigidBody);
            PlayerPOIWheelTriggerManager = new PlayerPOIWheelTriggerManager(playerObject.transform, GameInputManager, ContextActionWheelEventManager, pointOfInterestTrackerModule);
            PlayerContextActionManager = new PlayerContextActionManager();
            PlayerInventoryTriggerManager = new PlayerInventoryTriggerManager(GameInputManager, inventoryEventManager);
            PlayerAnimationManager = GetComponent<PlayerAnimationManager>();
            PlayerBodyPhysicsEnvironment = new PlayerBodyPhysicsEnvironment(playerRigidBody, playerCollider, PlayerPhysicsMovementComponent.MinimumDistanceToStick);
        }

        public void Tick(float d)
        {
            var playerSpeedMagnitude = 0f;

            if (!PointOfInterestType.IsDirectedByCutscene())
            {
                if (IsAllowedToMove())
                    PlayerInputMoveManager.Tick(d);
                else
                    PlayerInputMoveManager.ResetSpeed();
                playerSpeedMagnitude = PlayerInputMoveManager.PlayerSpeedProcessingInput.PlayerSpeedMagnitude;
            }
            else
            {
                playerSpeedMagnitude = PointOfInterestType.GetPointOfInterestCutsceneController().GetCurrentNormalizedSpeedMagnitude();
            }

            pointOfInterestTrackerModule.Tick(d);

            if (IsAllowedToDoAnyInteractions())
                //if statement to avoid processing inpout at the same frame
                if (!PlayerInventoryTriggerManager.Tick())
                    PlayerPOIWheelTriggerManager.Tick(d, PlayerPointOfInterestSelectionManager.GetCurrentSelectedPOI());

            PlayerAnimationManager.AnimationDataManager.Tick(playerSpeedMagnitude);
        }

        public void FixedTick(float d)
        {
            if (!PointOfInterestType.IsDirectedByCutscene())
            {
                PlayerInputMoveManager.FixedTick(d);
                //Physics is desabled when cutscene is playing to avoid conflicts with nav mesh agent
                PlayerBodyPhysicsEnvironment.FixedTick(d);
            }
        }

        public void LateTick(float d)
        {
        }

        public void OnGizmoTick()
        {
            // PlayerPOIVisualHeadMovementManager.GizmoTick();
        }

        public Animator GetPlayerAnimator()
        {
            if (PlayerAnimationManager != null) return PlayerAnimationManager.GetPlayerAnimator();
            return null;
        }

        public PointOfInterestType GetCurrentTargetedPOI()
        {
            return pointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest();
        }

        #region Player Common component

        private PlayerCommonComponents PlayerCommonComponents;
        private DataComponentContainer PlayerDataComponentContainer;

        #endregion

        #region Logical Conditions

        private bool IsAllowedToMove()
        {
            return !PlayerContextActionManager.IsActionExecuting && !PlayerPOIWheelTriggerManager.WheelEnabled && !PlayerInventoryTriggerManager.IsInventoryDisplayed;
        }

        private bool IsAllowedToDoAnyInteractions()
        {
            return IsAllowedToMove() && !PointOfInterestType.IsDirectedByCutscene();
        }

        public bool IsVisualMovementAllowed()
        {
            return (!PlayerContextActionManager.IsActionExecuting || PlayerContextActionManager.IsTalkActionExecuting)
                   && PointOfInterestType.IsVisualMovementAllowed();
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
    }

    #region POI

    internal class PlayerPOIWheelTriggerManager
    {
        private ContextActionWheelEventManager ContextActionWheelEventManager;
        private GameInputManager GameInputManager;
        private Transform PlayerTransform;
        private PointOfInterestTrackerModule PointOfInterestTrackerModule;

        private bool wheelEnabled;

        public PlayerPOIWheelTriggerManager(Transform playerTransform, GameInputManager gameInputManager, ContextActionWheelEventManager contextActionWheelEventManager, PointOfInterestTrackerModule pointOfInterestTrackerModule)
        {
            PlayerTransform = playerTransform;
            GameInputManager = gameInputManager;
            ContextActionWheelEventManager = contextActionWheelEventManager;
            PointOfInterestTrackerModule = pointOfInterestTrackerModule;
        }

        public bool WheelEnabled => wheelEnabled;

        public void Tick(float d, PointOfInterestType selectedPOI)
        {
            if (selectedPOI != null)
                if (GameInputManager.CurrentInput.ActionButtonD())
                    if (PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest() != null)
                    {
                        Debug.Log(PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest().name);
                        wheelEnabled = true;
                        ContextActionWheelEventManager.OnWheelEnabled(selectedPOI.GetContextActions(), WheelTriggerSource.PLAYER);
                    }
        }

        public void OnWheelDisabled()
        {
            wheelEnabled = false;
        }
    }

    #endregion

    #region Context Actions Handler

    internal class PlayerContextActionManager
    {
        private bool isActionExecuting;
        private bool isTalkActionExecuting;

        public bool IsActionExecuting => isActionExecuting;
        public bool IsTalkActionExecuting => isTalkActionExecuting;

        public void OnContextActionAdded(AContextAction contextActionAdded)
        {
            isActionExecuting = true;
            isTalkActionExecuting = false;
            if (contextActionAdded.IsTalkAction()) isTalkActionExecuting = true;
        }

        public void OnContextActionFinished()
        {
            isActionExecuting = false;
            isTalkActionExecuting = false;
        }
    }

    #endregion

    #region Inventory Trigger

    internal class PlayerInventoryTriggerManager
    {
        private GameInputManager GameInputManager;
        private InventoryEventManager InventoryEventManager;

        private bool isInventoryDisplayed;

        public PlayerInventoryTriggerManager(GameInputManager gameInputManager, InventoryEventManager inventoryEventManager)
        {
            GameInputManager = gameInputManager;
            InventoryEventManager = inventoryEventManager;
        }

        public bool IsInventoryDisplayed => isInventoryDisplayed;

        public bool Tick()
        {
            if (!isInventoryDisplayed)
                if (GameInputManager.CurrentInput.InventoryButtonD())
                {
                    InventoryEventManager.OnInventoryEnabled();
                    return true;
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