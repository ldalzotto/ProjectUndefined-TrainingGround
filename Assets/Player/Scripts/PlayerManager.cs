using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Movement")]
    public float SpeedMultiplicationFactor;
    public float AIRotationSpeed;

    [Header("Camera Rotation")]
    public float CameraRotationSpeed;
    [Header("Camera Follow")]
    public float DampTime;

    public PlayerPOITrackerManagerComponent PlayerPOITrackerManagerComponent;
    public PlayerPOIVisualHeadMovementComponent PlayerPOIVisualHeadMovementComponent;

    #region Sub Managers
    private PlayerAnimationManager PlayerAnimationManager;
    #endregion

    private CameraFollowManager CameraFollowManager;
    private CameraOrientationManager CameraOrientationManager;

    private PlayerMoveManager PlayerMoveManager;
    private PlayerPOITrackerManager PlayerPOITrackerManager;
    private PlayerPOIWheelTriggerManager PlayerPOIWheelTriggerManager;

    private PlayerPOIVisualHeadMovementManager PlayerPOIVisualHeadMovementManager;

    private PlayerContextActionManager PlayerContextActionManager;
    private PlayerInventoryTriggerManager PlayerInventoryTriggerManager;

    public void Init()
    {
        #region External dependencies
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        GameObject CameraPivotPoint = GameObject.FindGameObjectWithTag("CameraPivotPoint");
        ContextActionWheelEventManager ContextActionWheelEventManager = GameObject.FindObjectOfType<ContextActionWheelEventManager>();
        InventoryEventManager inventoryEventManager = GameObject.FindObjectOfType<InventoryEventManager>();
        #endregion

        GameObject PlayerObject = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG);
        Animator PlayerAnimator = GetComponentInChildren<Animator>();
        Rigidbody PlayerRigidBody = GetComponent<Rigidbody>();
        NavMeshAgent PlayerAgent = GetComponent<NavMeshAgent>();
        PlayerAgent.updatePosition = false;
        PlayerAgent.updateRotation = false;

        SphereCollider POITrackerCollider = transform.Find("POITracker").GetComponent<SphereCollider>();

        this.CameraFollowManager = new CameraFollowManager(PlayerObject.transform, CameraPivotPoint.transform);
        this.CameraOrientationManager = new CameraOrientationManager(CameraPivotPoint.transform, GameInputManager);
        this.PlayerMoveManager = new PlayerMoveManager(CameraPivotPoint.transform, PlayerRigidBody, GameInputManager, PlayerAgent, transform);
        this.PlayerPOITrackerManager = new PlayerPOITrackerManager(PlayerPOITrackerManagerComponent, POITrackerCollider, PlayerObject.transform);
        this.PlayerPOIWheelTriggerManager = new PlayerPOIWheelTriggerManager(PlayerObject.transform, GameInputManager, ContextActionWheelEventManager, PlayerPOITrackerManager);
        this.PlayerPOIVisualHeadMovementManager = new PlayerPOIVisualHeadMovementManager(PlayerPOIVisualHeadMovementComponent);
        this.PlayerContextActionManager = new PlayerContextActionManager();
        this.PlayerInventoryTriggerManager = new PlayerInventoryTriggerManager(GameInputManager, inventoryEventManager);
        this.PlayerAnimationManager = GetComponent<PlayerAnimationManager>();
    }

    public void Tick(float d)
    {
        CameraFollowManager.Tick(d, DampTime);
        CameraOrientationManager.Tick(d, CameraRotationSpeed);

        if (!IsAllowedToMove())
        {
            PlayerMoveManager.ResetSpeed();
        }
        else
        {
            PlayerMoveManager.Tick(d, SpeedMultiplicationFactor, AIRotationSpeed);
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

        PlayerAnimationManager.PlayerAnimationDataManager.Tick(PlayerMoveManager.PlayerSpeedMagnitude);

        if (PlayerContextActionManager.IsActionExecuting || PlayerMoveManager.PlayerSpeedMagnitude > float.Epsilon)
        {
            PlayerAnimationManager.OnIdleAnimationReset();
        }
        else
        {
            PlayerAnimationManager.PlayerIdleAnimationManager.Tick(d, PlayerMoveManager.PlayerSpeedMagnitude);
        }
    }

    public void FixedTick(float d)
    {
        PlayerMoveManager.FixedTick(d, SpeedMultiplicationFactor);
    }

    public void LateTick(float d)
    {
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
        return (!PlayerContextActionManager.IsActionExecuting && !PlayerPOIWheelTriggerManager.WheelEnabled && !PlayerInventoryTriggerManager.IsInventoryDisplayed)
            || PlayerMoveManager.IsDirectedByAi;
    }

    private bool IsAllowedToDoAnyInteractions()
    {
        return !PlayerContextActionManager.IsActionExecuting && !PlayerPOIWheelTriggerManager.WheelEnabled && !PlayerInventoryTriggerManager.IsInventoryDisplayed;
    }

    private bool IsHeadRotatingTowardsPOI()
    {
        return !PlayerContextActionManager.IsActionExecuting && !PlayerAnimationManager.IsIdleAnimationRunnig();
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
    public void OnContextActionAdded()
    {
        PlayerContextActionManager.OnContextActionAdded();
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
    public void SetDestination(Vector3 destination)
    {
        StartCoroutine(PlayerMoveManager.SetDestination(destination));
    }
    public IEnumerator SetDestinationCoRoutine(Vector3 destination)
    {
        return PlayerMoveManager.SetDestination(destination);
    }
    #endregion

    public Animator GetPlayerAnimator()
    {
        return PlayerAnimationManager.GetPlayerAnimator();
    }

    public PointOfInterestType GetCurrentTargetedPOI()
    {
        return PlayerPOITrackerManager.NearestInRangeInteractabledPointOfInterest;
    }

}

#region Camera

public class CameraFollowManager
{
    private Transform playerPosition;
    private Transform cameraPivotPoint;

    private Vector3 currentVelocity;

    public CameraFollowManager(Transform playerPosition, Transform cameraPivotPoint)
    {
        this.playerPosition = playerPosition;
        this.cameraPivotPoint = cameraPivotPoint;
    }

    public void Tick(float d, float smoothTime)
    {
        cameraPivotPoint.position = Vector3.SmoothDamp(cameraPivotPoint.position, playerPosition.position, ref currentVelocity, smoothTime);
    }
}

public class CameraOrientationManager
{
    private Transform cameraPivotPoint;
    private GameInputManager gameInputManager;

    public CameraOrientationManager(Transform cameraPivotPoint, GameInputManager gameInputManager)
    {
        this.cameraPivotPoint = cameraPivotPoint;
        this.gameInputManager = gameInputManager;
    }

    public void Tick(float d, float rotationSpeed)
    {
        var deltaRotation = gameInputManager.CurrentInput.RightRotationCameraDH() * d * rotationSpeed;
        deltaRotation += gameInputManager.CurrentInput.LeftRotationCameraDH() * d * -rotationSpeed;
        cameraPivotPoint.eulerAngles += new Vector3(0, deltaRotation, 0);
    }
}

#endregion

#region Player
class PlayerMoveManager
{
    private Transform CameraPivotPoint;
    private Rigidbody PlayerRigidBody;
    private GameInputManager GameInputManager;
    private NavMeshAgent playerAgent;
    private Transform playerTransform;

    private bool isDirectedByAi;
    private Vector3 playerMovementOrientation;
    private float playerSpeedMagnitude;

    public float PlayerSpeedMagnitude { get => playerSpeedMagnitude; }
    public bool IsDirectedByAi { get => isDirectedByAi; }

    public PlayerMoveManager(Transform cameraPivotPoint, Rigidbody playerRigidBody, GameInputManager gameInputManager, NavMeshAgent playerAgent, Transform playerTransform)
    {
        CameraPivotPoint = cameraPivotPoint;
        PlayerRigidBody = playerRigidBody;
        GameInputManager = gameInputManager;
        this.playerAgent = playerAgent;
        this.playerTransform = playerTransform;
    }

    public void Tick(float d, float SpeedMultiplicationFactor, float AIRotationSpeed)
    {
        if (!isDirectedByAi)
        {
            var currentCameraAngle = CameraPivotPoint.transform.eulerAngles.y;

            var inputDisplacementVector = GameInputManager.CurrentInput.LocomotionAxis();
            var projectedDisplacement = Quaternion.Euler(0, currentCameraAngle, 0) * inputDisplacementVector;

            playerMovementOrientation = projectedDisplacement.normalized;

            #region Calculate magnitude attenuation
            float magnitudeAttenuationDiagonal = 1f;

            playerSpeedMagnitude = inputDisplacementVector.sqrMagnitude / magnitudeAttenuationDiagonal;
            #endregion
        }
        else
        {
            playerSpeedMagnitude = 1;
            if (playerAgent.velocity.normalized != Vector3.zero)
            {
                playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, Quaternion.LookRotation(playerAgent.velocity.normalized), d * AIRotationSpeed);
            }

            playerMovementOrientation = (playerAgent.nextPosition - playerTransform.position).normalized;
            //    playerAgent.velocity = playerMovementOrientation * playerSpeedMagnitude * SpeedMultiplicationFactor;
            playerAgent.speed = SpeedMultiplicationFactor;
            PlayerRigidBody.transform.position = playerAgent.nextPosition;
        }

    }

    public void ResetSpeed()
    {
        playerSpeedMagnitude = 0;
    }

    public void FixedTick(float d, float SpeedMultiplicationFactor)
    {
        if (!isDirectedByAi)
        {
            //move rigid body rotation
            if (playerMovementOrientation.sqrMagnitude > .05)
            {
                PlayerRigidBody.MoveRotation(Quaternion.LookRotation(playerMovementOrientation));
            }

            //move rigid body
            PlayerRigidBody.velocity = playerMovementOrientation * playerSpeedMagnitude * SpeedMultiplicationFactor;
        }
    }

    public IEnumerator SetDestination(Vector3 destination)
    {
        isDirectedByAi = true;
        playerAgent.nextPosition = playerTransform.position;
        playerAgent.SetDestination(destination);
        PlayerRigidBody.isKinematic = true;
        yield return GameInputManager.StartCoroutine(new WaitForNavAgentDestinationReached(playerAgent));
        isDirectedByAi = false;
        PlayerRigidBody.isKinematic = false;
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
            if (Vector3.Distance(PlayerTransform.position, nearestInRangePointOfInterest.transform.position) <= nearestInRangePointOfInterest.MaxDistanceToInteractWithPlayer)
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
            if (beforeHeadMoveAngle <= playerPOIVisualHeadMovementComponent.HeadMoveAngleLimit)
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
                    var targetRotation = Quaternion.LookRotation(LastNearestPOI.transform.position - affectedBone.transform.position);
                    affectedBone.rotation = Quaternion.Slerp(InterpolatedBoneRotations[i], targetRotation, playerPOIVisualHeadMovementComponent.SmoothMovementSpeed * d);
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
    [Header("Angle limit in +-X")]
    public float HeadMoveAngleLimit;

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
            Gizmos.DrawRay(nearestPOI.transform.position, (PlayerTransform.position - nearestPOI.transform.position).normalized * nearestPOI.MaxDistanceToInteractWithPlayer);
        }
    }
}
#endregion

#region Context Actions Handler
class PlayerContextActionManager
{
    private bool isActionExecuting;

    public bool IsActionExecuting { get => isActionExecuting; }

    public void OnContextActionAdded()
    {
        isActionExecuting = true;
    }

    public void OnContextActionFinished()
    {
        isActionExecuting = false;
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

