﻿using CoreGame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace AdventureGame
{

    public class PlayerManager : PlayerManagerType
    {
        private const string ObstacelOvercomeObjectName = "ObstacleOvercomeTrigger";

        public PlayerPOITrackerManagerComponent PlayerPOITrackerManagerComponent;
        public PlayerPOIVisualHeadMovementComponent PlayerPOIVisualHeadMovementComponent;

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

        private PlayerPOITrackerManager PlayerPOITrackerManager;
        private PlayerPOIWheelTriggerManager PlayerPOIWheelTriggerManager;

        private PlayerPOIVisualHeadMovementManager PlayerPOIVisualHeadMovementManager;

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

            SphereCollider POITrackerCollider = transform.Find("POITracker").GetComponent<SphereCollider>();
            this.PlayerCommonComponents = GetComponentInChildren<PlayerCommonComponents>();
            this.PlayerDataComponentContainer = GetComponentInChildren<DataComponentContainer>();
            this.PlayerDataComponentContainer.Init();

            this.CameraFollowManager = new CameraFollowManager(playerObject.transform, CameraPivotPoint.transform, this.PlayerCommonComponents.CameraFollowManagerComponent, playerPosition);
            this.CameraOrientationManager = new CameraOrientationManager(CameraPivotPoint.transform, GameInputManager, this.PlayerCommonComponents.CameraOrientationManagerComponent);
            this.PlayerInputMoveManager = new PlayerInputMoveManager(this.PlayerDataComponentContainer.GetDataComponent<TransformMoveManagerComponentV2>(), CameraPivotPoint.transform, GameInputManager, playerRigidBody);
            this.PlayerObstacleOvercomeManager = new PlayerObstacleOvercomeManager(playerRigidBody, obstacleOvercomeCollider);
            this.PlayerPOITrackerManager = new PlayerPOITrackerManager(PlayerPOITrackerManagerComponent, POITrackerCollider, playerObject.transform);
            this.PlayerPOIWheelTriggerManager = new PlayerPOIWheelTriggerManager(playerObject.transform, GameInputManager, ContextActionWheelEventManager, PlayerPOITrackerManager);
            this.PlayerPOIVisualHeadMovementManager = new PlayerPOIVisualHeadMovementManager(PlayerPOIVisualHeadMovementComponent);
            this.PlayerContextActionManager = new PlayerContextActionManager();
            this.PlayerInventoryTriggerManager = new PlayerInventoryTriggerManager(GameInputManager, inventoryEventManager);
            this.PlayerAnimationManager = GetComponent<PlayerAnimationManager>();
            this.PlayerProceduralAnimationsManager = new PlayerProceduralAnimationsManager(this.PlayerCommonComponents, this.PlayerDataComponentContainer.GetDataComponent<TransformMoveManagerComponentV2>(), playerAnimator, playerRigidBody, coreConfigurationManager);
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


            if (IsAllowedToDoAnyInteractions())
            {
                PlayerPOITrackerManager.Tick(d);

                //if statement to avoid processing inpout at the same frame
                if (!PlayerInventoryTriggerManager.Tick())
                {
                    PlayerPOIWheelTriggerManager.Tick(d, PlayerPOITrackerManager.NearestInRangePointOfInterest);
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
            if (IsHeadRotatingTowardsPOI())
            {
                PlayerPOIVisualHeadMovementManager.LateTick(d, PlayerPOITrackerManager.NearestInRangePointOfInterest);
            }
            else
            {
                PlayerPOIVisualHeadMovementManager.LateTickNoFollowing();
            }
        }

        public void OnGizmoTick()
        {
            if (IsAllowedToDoAnyInteractions())
            {
                PlayerPOITrackerManager.OnGizmoTick();
                PlayerPOIWheelTriggerManager.GizmoTick(PlayerPOITrackerManager.NearestInRangePointOfInterest);
            }
            PlayerPOIVisualHeadMovementManager.GizmoTick();
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

        private bool IsHeadRotatingTowardsPOI()
        {
            return (!PlayerContextActionManager.IsActionExecuting || PlayerContextActionManager.IsTalkActionExecuting) && !PlayerAnimationManager.IsIdleAnimationRunnig();
        }

        #endregion

        #region External Events
        public void TriggerEnter(Collider collider, CollisionType source)
        {
            if (source.IsPoi)
            {
                PlayerPOITrackerManager.OnObjectEnter(collider.gameObject);
            }
        }
        public void TriggerExit(Collider collider, CollisionType source)
        {
            if (source.IsPoi)
            {
                PlayerPOITrackerManager.OnObjectExit(collider.gameObject);
            }
        }
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
            PlayerPOIVisualHeadMovementManager.OnContextActionAdded();
        }
        public void OnContextActionFinished()
        {
            PlayerContextActionManager.OnContextActionFinished();
        }
        public void OnWheelDisabled()
        {
            PlayerPOIWheelTriggerManager.OnWheelDisabled();
        }
        public void OnPOIDestroyed(PointOfInterestType pointOfInterestType)
        {
            PlayerPOITrackerManager.POIDeleted(pointOfInterestType);
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
            return PlayerPOITrackerManager.NearestInRangeInteractabledPointOfInterest;
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
    class PlayerPOITrackerManager
    {
        private PlayerPOITrackerManagerComponent PlayerPOITrackerManagerComponent;
        private SphereCollider TrackerCollider;
        private Transform PlayerTransform;
        private List<PointOfInterestType> InRangePointOfInterests = new List<PointOfInterestType>();
        private PointOfInterestType nearestInRangePointOfInterest;
        private PointOfInterestType nearestInRangeInteractabledPointOfInterest;

        public PointOfInterestType NearestInRangePointOfInterest { get => nearestInRangePointOfInterest; }
        public PointOfInterestType NearestInRangeInteractabledPointOfInterest { get => nearestInRangeInteractabledPointOfInterest; }

        public PlayerPOITrackerManager(PlayerPOITrackerManagerComponent playerPOITrackerManagerComponent, SphereCollider TrackerCollider, Transform PlayerTransform)
        {
            PlayerPOITrackerManagerComponent = playerPOITrackerManagerComponent;
            this.TrackerCollider = TrackerCollider;
            this.TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
            this.PlayerTransform = PlayerTransform;
        }

        public void Tick(float d)
        {
            TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
            nearestInRangePointOfInterest = GetNearestPOI();
            nearestInRangeInteractabledPointOfInterest = null;
            if (nearestInRangePointOfInterest != null)
            {
                if (Vector3.Distance(PlayerTransform.position, nearestInRangePointOfInterest.transform.position) <= nearestInRangePointOfInterest.GetMaxDistanceToInteractWithPlayer())
                {
                    nearestInRangeInteractabledPointOfInterest = nearestInRangePointOfInterest;
                }
            }
        }

        public void OnObjectEnter(GameObject obj)
        {
            var POIType = obj.GetComponent<PointOfInterestType>();
            if (POIType != null)
            {
                InRangePointOfInterests.Add(POIType);
            }
        }
        public void OnObjectExit(GameObject obj)
        {
            var POIType = obj.GetComponent<PointOfInterestType>();
            if (POIType != null)
            {
                InRangePointOfInterests.Remove(POIType);
            }
        }

        private PointOfInterestType GetNearestPOI()
        {
            PointOfInterestType nearestPoi = null;
            foreach (var POI in InRangePointOfInterests)
            {
                if (nearestPoi == null)
                {
                    nearestPoi = POI;
                }
                else
                {
                    if (Vector3.Distance(POI.transform.position, TrackerCollider.transform.position) <= Vector3.Distance(nearestPoi.transform.position, TrackerCollider.transform.position))
                    {
                        nearestPoi = POI;
                    }
                }
            }
            return nearestPoi;
        }

        public void POIDeleted(PointOfInterestType deletedPOI)
        {
            if (InRangePointOfInterests.Contains(deletedPOI))
            {
                InRangePointOfInterests.Remove(deletedPOI);
            }
        }

        public void OnGizmoTick()
        {
            if (PlayerPOITrackerManagerComponent != null && TrackerCollider != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(TrackerCollider.transform.position, PlayerPOITrackerManagerComponent.SphereDetectionRadius);
                var labelStyle = GUI.skin.GetStyle("Label");
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.textColor = Color.blue;
#if UNITY_EDITOR
                Handles.Label(TrackerCollider.transform.position + new Vector3(0, PlayerPOITrackerManagerComponent.SphereDetectionRadius, 0), "POI Trigger Sphere Detection", labelStyle);
#endif
            }
        }
    }

    [System.Serializable]
    public class PlayerPOITrackerManagerComponent
    {
        public float SphereDetectionRadius;
    }

    class PlayerPOIVisualHeadMovementManager
    {
        private PlayerPOIVisualHeadMovementComponent playerPOIVisualHeadMovementComponent;

        private PointOfInterestType LastNearestPOI;
        private List<Quaternion> InterpolatedBoneRotations = new List<Quaternion>();

        private bool IsLookingToPOI;
        private bool hasEndedSmoothingOut;

        public PlayerPOIVisualHeadMovementManager(PlayerPOIVisualHeadMovementComponent playerPOIVisualHeadMovementComponent)
        {
            this.playerPOIVisualHeadMovementComponent = playerPOIVisualHeadMovementComponent;
            for (var i = 0; i < playerPOIVisualHeadMovementComponent.BonesThatReactToPOI.Length; i++)
            {
                InterpolatedBoneRotations.Add(Quaternion.identity);
            }
        }

        public void LateTick(float d, PointOfInterestType NearestPOI)
        {

            LastNearestPOI = NearestPOI;
            if (LastNearestPOI != null)
            {
                var beforeHeadMoveAngle = Vector3.Angle(playerPOIVisualHeadMovementComponent.HeadBone.forward, LastNearestPOI.transform.position - playerPOIVisualHeadMovementComponent.HeadBone.position);
                if (beforeHeadMoveAngle <= playerPOIVisualHeadMovementComponent.POIDetectionAngleLimit)
                {

                    if (!IsLookingToPOI)
                    {
                        //first time looking
                        ResetInterpolatedBoneRotationToActual();
                    }

                    IsLookingToPOI = true;
                    hasEndedSmoothingOut = false;

                    for (var i = 0; i < playerPOIVisualHeadMovementComponent.BonesThatReactToPOI.Length; i++)
                    {
                        var affectedBone = playerPOIVisualHeadMovementComponent.BonesThatReactToPOI[i];

                        // (1) - Target direction is the direction between bone and POI point.
                        var targetDirection = (LastNearestPOI.transform.position - affectedBone.transform.position).normalized;

                        // (2) - We clamp the bone rotation to a cone.
                        var coneClampedRotation = QuaternionHelper.ConeReduction(playerPOIVisualHeadMovementComponent.HeadBone.forward, targetDirection, this.playerPOIVisualHeadMovementComponent.RotationAngleLimit);

                        // (3) - We rotate the target direction to fit the cone constraint.
                        var adjustedDirection = (coneClampedRotation * targetDirection).normalized;

                        /*
                        Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (playerPOIVisualHeadMovementComponent.HeadBone.forward * 10), Color.blue);
                        Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (targetDirection.normalized * 10), Color.green);
                        Debug.DrawLine(playerPOIVisualHeadMovementComponent.HeadBone.transform.position, playerPOIVisualHeadMovementComponent.HeadBone.transform.position + (adjustedDirection.normalized * 10), Color.red);
                        */

                        affectedBone.rotation = Quaternion.Slerp(InterpolatedBoneRotations[i], Quaternion.LookRotation(adjustedDirection, affectedBone.transform.up),
                            playerPOIVisualHeadMovementComponent.SmoothMovementSpeed * d);
                        InterpolatedBoneRotations[i] = affectedBone.rotation;
                    }
                }
                else
                {
                    IsLookingToPOI = false;
                    SmoothNoLookingTransition(d);
                }
            }
            else
            {
                IsLookingToPOI = false;
                SmoothNoLookingTransition(d);
            }
        }

        private void SmoothNoLookingTransition(float d)
        {
            if (!hasEndedSmoothingOut)
            {
                for (var i = 0; i < playerPOIVisualHeadMovementComponent.BonesThatReactToPOI.Length; i++)
                {
                    var affectedBone = playerPOIVisualHeadMovementComponent.BonesThatReactToPOI[i];
                    var dotProductToTarget = Mathf.Abs(Quaternion.Dot(InterpolatedBoneRotations[i], affectedBone.rotation));

                    //too much angle to smooth -> direct transition
                    if (dotProductToTarget <= playerPOIVisualHeadMovementComponent.SmoothOutMaxDotProductLimit)
                    {
                        hasEndedSmoothingOut = true;
                    }
                    else if (dotProductToTarget <= 0.9999f)
                    {
                        affectedBone.rotation = Quaternion.Slerp(InterpolatedBoneRotations[i], affectedBone.rotation, playerPOIVisualHeadMovementComponent.SmoothMovementSpeed * d);
                        InterpolatedBoneRotations[i] = affectedBone.rotation;
                    }
                    else
                    {
                        hasEndedSmoothingOut = true;
                    }
                }
            }
        }

        public void LateTickNoFollowing()
        {
            ResetInterpolatedBoneRotationToActual();
        }


        private void ResetInterpolatedBoneRotationToActual()
        {
            for (var i = 0; i < playerPOIVisualHeadMovementComponent.BonesThatReactToPOI.Length; i++)
            {
                var affectedBone = playerPOIVisualHeadMovementComponent.BonesThatReactToPOI[i];
                InterpolatedBoneRotations[i] = affectedBone.rotation;
            }
        }

        public void OnContextActionAdded()
        {
            hasEndedSmoothingOut = false;
        }

        public void GizmoTick()
        {
            if (LastNearestPOI != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(LastNearestPOI.transform.position, 1f);

                for (var i = 0; i < playerPOIVisualHeadMovementComponent.BonesThatReactToPOI.Length; i++)
                {
                    Gizmos.DrawLine(playerPOIVisualHeadMovementComponent.BonesThatReactToPOI[i].position, LastNearestPOI.transform.position);
#if UNITY_EDITOR
                    Handles.Label(LastNearestPOI.transform.position, "Targeted POI");
#endif
                }
            }
        }

    }

    [System.Serializable]
    public class PlayerPOIVisualHeadMovementComponent
    {
        public Transform[] BonesThatReactToPOI;
        public Transform HeadBone;
        [Tooltip("This angle is the maximum value for the look system to be enabled. The angle is Ang(player forward, player to POI)")]
        public float POIDetectionAngleLimit;
        [Tooltip("This angle is the maximum angle where player actually rotate.")]
        public float RotationAngleLimit;

        public float SmoothMovementSpeed;
        [Range(0.0f, 1.0f)]
        [Tooltip("When head exits POI interest, indicates the minimum dot product from current head rotation and target to smooth out." +
            "If calculated dot < SmoothOutMaxDotProductLimit -> no smooth out, head is instantly rotating towards animation rotation.")]
        public float SmoothOutMaxDotProductLimit = 0.4f;
    }

    class PlayerPOIWheelTriggerManager
    {
        private Transform PlayerTransform;
        private GameInputManager GameInputManager;
        private ContextActionWheelEventManager ContextActionWheelEventManager;
        private PlayerPOITrackerManager PlayerPOITrackerManager;

        private bool wheelEnabled;

        public bool WheelEnabled { get => wheelEnabled; }

        public PlayerPOIWheelTriggerManager(Transform playerTransform, GameInputManager gameInputManager, ContextActionWheelEventManager contextActionWheelEventManager, PlayerPOITrackerManager PlayerPOITrackerManager)
        {
            PlayerTransform = playerTransform;
            GameInputManager = gameInputManager;
            ContextActionWheelEventManager = contextActionWheelEventManager;
            this.PlayerPOITrackerManager = PlayerPOITrackerManager;
        }

        public void Tick(float d, PointOfInterestType nearestPOI)
        {
            if (nearestPOI != null)
            {
                if (GameInputManager.CurrentInput.ActionButtonD())
                {
                    if (PlayerPOITrackerManager.NearestInRangeInteractabledPointOfInterest != null)
                    {
                        Debug.Log(PlayerPOITrackerManager.NearestInRangeInteractabledPointOfInterest.name);
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