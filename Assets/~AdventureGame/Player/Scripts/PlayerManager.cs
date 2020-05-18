﻿using CoreGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using static AnimationConstants;

namespace AdventureGame
{

    public class PlayerManager : PlayerManagerType, IVisualMovementPermission
    {
        private const string ObstacelOvercomeObjectName = "ObstacleOvercomeTrigger";

        #region Player Common component
        private PlayerCommonComponents PlayerCommonComponents;
        private DataComponentContainer PlayerDataComponentContainer;
        #endregion

        #region Animation Managers
        private PlayerAnimationManager PlayerAnimationManager;
        private PlayerProceduralAnimationsManager PlayerProceduralAnimationsManager;
        #endregion

        private CameraOrientationManager CameraOrientationManager;

        private PlayerInputMoveManager PlayerInputMoveManager;
        //Inter dependency
        private PointOfInterestType PointOfInterestType;
        // private PlayerAIMoveManager PlayerAIMoveManager;
        private PlayerObstacleOvercomeManager PlayerObstacleOvercomeManager;

        private PointOfInterestTrackerModule PointOfInterestTrackerModule;
        private PlayerPOIWheelTriggerManager PlayerPOIWheelTriggerManager;

        // private PlayerPOIVisualHeadMovementManager PlayerPOIVisualHeadMovementManager;

        private PlayerContextActionManager PlayerContextActionManager;
        private PlayerInventoryTriggerManager PlayerInventoryTriggerManager;

        public void Init()
        {
            #region External dependencies
            GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
            GameObject CameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG);
            ContextActionWheelEventManager ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
            InventoryEventManager inventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
            var coreConfigurationManager = GameObject.FindObjectOfType<CoreConfigurationManager>();
            #endregion

            #region Load Persisted Position
            var playerPosition = GameObject.FindObjectOfType<PlayerAdventurePositionManager>().PlayerPositionBeforeLevelLoad;
            if (playerPosition != null)
            {
                this.transform.position = playerPosition.GetPosition();
                this.transform.rotation = playerPosition.GetQuaternion();
            }
            #endregion

            this.PointOfInterestType = GetComponentInChildren<PointOfInterestType>();
            GameObject playerObject = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG);
            BoxCollider obstacleOvercomeCollider = gameObject.FindChildObjectRecursively(ObstacelOvercomeObjectName).GetComponent<BoxCollider>();
            Animator playerAnimator = GetComponentInChildren<Animator>();
            Rigidbody playerRigidBody = GetComponent<Rigidbody>();
            NavMeshAgent playerAgent = GetComponent<NavMeshAgent>();
            playerAgent.updatePosition = false;
            playerAgent.updateRotation = false;

            this.PlayerCommonComponents = GetComponentInChildren<PlayerCommonComponents>();
            this.PlayerDataComponentContainer = GetComponentInChildren<DataComponentContainer>();
            this.PlayerDataComponentContainer.Init();

            #region Data Components
            var TransformMoveManagerComponentV2 = this.PlayerDataComponentContainer.GetDataComponent<TransformMoveManagerComponentV2>();
            var PlayerPOITrackerManagerComponentV2 = this.PlayerDataComponentContainer.GetDataComponent<PlayerPOITrackerManagerComponentV2>();
            #endregion

            #region POI Modules
            this.PointOfInterestTrackerModule = this.PointOfInterestType.GetPointOfInterestTrackerModule();
            #endregion

            this.CameraFollowManager = new CameraFollowManager(playerObject.transform, CameraPivotPoint.transform, this.PlayerCommonComponents.CameraFollowManagerComponent, playerPosition);
            this.CameraOrientationManager = new CameraOrientationManager(CameraPivotPoint.transform, GameInputManager, this.PlayerCommonComponents.CameraOrientationManagerComponent);
            this.PlayerInputMoveManager = new PlayerInputMoveManager(TransformMoveManagerComponentV2, CameraPivotPoint.transform, GameInputManager, playerRigidBody);
            this.PlayerObstacleOvercomeManager = new PlayerObstacleOvercomeManager(playerRigidBody, obstacleOvercomeCollider);
            this.PlayerPOIWheelTriggerManager = new PlayerPOIWheelTriggerManager(playerObject.transform, GameInputManager, ContextActionWheelEventManager, this.PointOfInterestTrackerModule);
            //  this.PlayerPOIVisualHeadMovementManager = new PlayerPOIVisualHeadMovementManager(PlayerPOIVisualHeadMovementComponent, playerAnimator);
            this.PlayerContextActionManager = new PlayerContextActionManager();
            this.PlayerInventoryTriggerManager = new PlayerInventoryTriggerManager(GameInputManager, inventoryEventManager);
            this.PlayerAnimationManager = GetComponent<PlayerAnimationManager>();
            this.PlayerProceduralAnimationsManager = new PlayerProceduralAnimationsManager(this.PlayerCommonComponents, TransformMoveManagerComponentV2, playerAnimator, playerRigidBody, coreConfigurationManager);
        }

        public void Tick(float d)
        {
            CameraFollowManager.Tick(d);
            CameraOrientationManager.Tick(d);

            var playerSpeedMagnitude = 0f;

            if (!this.PointOfInterestType.GetPointOfInterestCutsceneController().IsDirectedByCutscene())
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

            PointOfInterestTrackerModule.Tick(d);

            if (IsAllowedToDoAnyInteractions())
            {

                //if statement to avoid processing inpout at the same frame
                if (!PlayerInventoryTriggerManager.Tick())
                {
                    PlayerPOIWheelTriggerManager.Tick(d, PointOfInterestTrackerModule.NearestInRangePointOfInterest());
                }
            }

            PlayerAnimationManager.PlayerAnimationDataManager.Tick(playerSpeedMagnitude);

            if (PlayerContextActionManager.IsActionExecuting || playerSpeedMagnitude > float.Epsilon)
            {
                PlayerAnimationManager.OnIdleAnimationReset();
            }
            else
            {
                PlayerAnimationManager.PlayerIdleAnimationManager.Tick(d, playerSpeedMagnitude);
            }
        }

        public void FixedTick(float d)
        {
            this.PlayerProceduralAnimationsManager.FickedTick(d);

            if (!this.PointOfInterestType.GetPointOfInterestCutsceneController().IsDirectedByCutscene())
            {
                this.PlayerInputMoveManager.FixedTick(d);
            }
        }

        public void LateTick(float d)
        {
            this.PlayerProceduralAnimationsManager.LateTick(d);
        }

        public void OnGizmoTick()
        {
            if (IsAllowedToDoAnyInteractions())
            {
                // PointOfInterestTrackerModule.OnGizmoTick();
                // PlayerPOIWheelTriggerManager.GizmoTick(PointOfInterestTrackerModule.NearestInRangePointOfInterest());
            }
            // PlayerPOIVisualHeadMovementManager.GizmoTick();
        }

        #region Logical Conditions
        private bool IsAllowedToMove()
        {
            return !PlayerContextActionManager.IsActionExecuting && !PlayerPOIWheelTriggerManager.WheelEnabled && !PlayerInventoryTriggerManager.IsInventoryDisplayed;
        }

        private bool IsAllowedToDoAnyInteractions()
        {
            return this.IsAllowedToMove() && !this.PointOfInterestType.GetPointOfInterestCutsceneController().IsDirectedByCutscene();
        }

        public bool IsVisualMovementAllowed()
        {
            return (!PlayerContextActionManager.IsActionExecuting || PlayerContextActionManager.IsTalkActionExecuting) && !PlayerAnimationManager.IsIdleAnimationRunnig();
        }

        #endregion

        #region External Events
        public void ObstacleOvercomeTriggerEnter(Collider collider)
        {
            if (IsAllowedToMove())
            {
                PlayerObstacleOvercomeManager.ObstacleOvercomeTriggerEnter(collider);
            }
        }
        public void OnContextActionAdded(AContextAction contextActionAdded)
        {
            PlayerContextActionManager.OnContextActionAdded(contextActionAdded);
            // PlayerPOIVisualHeadMovementManager.OnContextActionAdded();
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

        [Obsolete("Cutscene movement must be handled with the new system")]
        public IEnumerator SetAIDestinationCoRoutine(Transform destination, float normalizedSpeed)
        {
            return this.PointOfInterestType.GetPointOfInterestCutsceneController().SetAIDestination(destination, normalizedSpeed);
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
            return PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest();
        }

    }

    #region Player Movement

    class PlayerObstacleOvercomeManager
    {
        private Rigidbody playerRigidBody;
        private BoxCollider obstacleOvercomeCollider;

        public PlayerObstacleOvercomeManager(Rigidbody playerRigidBody, BoxCollider ObstacleOvercomeCollider)
        {
            this.playerRigidBody = playerRigidBody;
            this.obstacleOvercomeCollider = ObstacleOvercomeCollider;
        }

        public void ObstacleOvercomeTriggerEnter(Collider collider)
        {
            if (obstacleOvercomeCollider.bounds.max.y >= collider.bounds.max.y)
            {
                var nexPosition = new Vector3(playerRigidBody.position.x, collider.bounds.max.y, playerRigidBody.position.z);
                this.playerRigidBody.MovePosition(nexPosition);
            }
        }
    }
    #endregion

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

        public void Tick(float d, PointOfInterestType nearestPOI)
        {
            if (nearestPOI != null)
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    if (PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest() != null)
                    {
                        Debug.Log(PointOfInterestTrackerModule.NearestInRangeInteractabledPointOfInterest().name);
                        wheelEnabled = true;
                        ContextActionWheelEventManager.OnWheelEnabled(nearestPOI.GetContextActions(), WheelTriggerSource.PLAYER);
                    }
                }
            }
        }

        public void OnWheelDisabled()
        {
            wheelEnabled = false;
        }

        public void GizmoTick(PointOfInterestType nearestPOI)
        {
            if (PlayerTransform != null && nearestPOI != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(nearestPOI.transform.position, (PlayerTransform.position - nearestPOI.transform.position).normalized * nearestPOI.GetMaxDistanceToInteractWithPlayer());
            }
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