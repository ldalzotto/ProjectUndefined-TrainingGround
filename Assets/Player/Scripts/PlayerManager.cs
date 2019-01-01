using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("Player Movement")]
    public float SpeedMultiplicationFactor;

    [Header("Camera Rotation")]
    public float CameraRotationSpeed;
    [Header("Camera Follow")]
    public float DampTime;

    public PlayerPOITrackerManagerComponent PlayerPOITrackerManagerComponent;
    public PlayerPOIVisualHeadMovementComponent PlayerPOIVisualHeadMovementComponent;

    private CameraFollowManager CameraFollowManager;
    private CameraOrientationManager CameraOrientationManager;

    private PlayerMoveManager PlayerMoveManager;
    private PlayerPOITrackerManager PlayerPOITrackerManager;
    private PlayerPOIVisualHeadMovementManager PlayerPOIVisualHeadMovementManager;

    private PlayerContextActionManager PlayerContextActionManager;

    private PlayerAnimationDataManager PlayerAnimationDataManager;

    private void Start()
    {
        GameInputManager GameInputManager = GameObject.FindObjectOfType<GameInputManager>();
        GameObject CameraPivotPoint = GameObject.FindGameObjectWithTag("CameraPivotPoint");
        GameObject PlayerObject = GameObject.FindGameObjectWithTag("Player");
        Animator PlayerAnimator = GetComponentInChildren<Animator>();
        Rigidbody PlayerRigidBody = GetComponent<Rigidbody>();

        SphereCollider POITrackerCollider = transform.Find("POITracker").GetComponent<SphereCollider>();

        this.CameraFollowManager = new CameraFollowManager(PlayerObject.transform, CameraPivotPoint.transform);
        this.CameraOrientationManager = new CameraOrientationManager(CameraPivotPoint.transform, GameInputManager);
        this.PlayerMoveManager = new PlayerMoveManager(CameraPivotPoint.transform, PlayerRigidBody, GameInputManager);
        this.PlayerPOITrackerManager = new PlayerPOITrackerManager(PlayerPOITrackerManagerComponent, POITrackerCollider);
        this.PlayerPOIVisualHeadMovementManager = new PlayerPOIVisualHeadMovementManager(PlayerPOIVisualHeadMovementComponent);
        this.PlayerContextActionManager = new PlayerContextActionManager();
        this.PlayerAnimationDataManager = new PlayerAnimationDataManager(PlayerAnimator);
    }

    public void Tick(float d)
    {
        CameraFollowManager.Tick(d, DampTime);
        CameraOrientationManager.Tick(d, CameraRotationSpeed);

        if (PlayerContextActionManager.IsActionExecuting)
        {
            PlayerMoveManager.ResetSpeed();
        }
        else
        {
            PlayerMoveManager.Tick(d);
        }

        PlayerPOITrackerManager.Tick(d);
        PlayerAnimationDataManager.Tick(PlayerMoveManager.PlayerSpeedMagnitude);
    }

    public void FixedTick(float d)
    {
        PlayerMoveManager.FixedTick(d, SpeedMultiplicationFactor);
    }

    public void LateTick(float d)
    {
        if (!PlayerContextActionManager.IsActionExecuting)
        {
            PlayerPOIVisualHeadMovementManager.LateTick(d, PlayerPOITrackerManager.GetNearestPOI());
        }

    }

    public void OnGizmoTick()
    {
        PlayerPOITrackerManager.OnGizmoTick();
        PlayerPOIVisualHeadMovementManager.GizmoTick();
    }

    #region External Events
    public void TriggerEnter(Collider collider, CollisionTag source)
    {
        switch (source)
        {
            case CollisionTag.POITracker:
                PlayerPOITrackerManager.OnObjectEnter(collider.gameObject);
                break;
        }
    }
    public void TriggerExit(Collider collider, CollisionTag source)
    {
        switch (source)
        {
            case CollisionTag.POITracker:
                PlayerPOITrackerManager.OnObjectExit(collider.gameObject);
                break;
        }
    }
    public void OnContextActionAdded(AContextAction contextAction)
    {
        PlayerContextActionManager.OnContextActionAdded(contextAction);
    }
    #endregion
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

    private Vector3 playerMovementOrientation;
    private float playerSpeedMagnitude;

    public float PlayerSpeedMagnitude { get => playerSpeedMagnitude; }

    public PlayerMoveManager(Transform cameraPivotPoint, Rigidbody playerRigidBody, GameInputManager gameInputManager)
    {
        CameraPivotPoint = cameraPivotPoint;
        PlayerRigidBody = playerRigidBody;
        GameInputManager = gameInputManager;
    }

    public void Tick(float d)
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

    public void ResetSpeed()
    {
        playerSpeedMagnitude = 0;
    }

    public void FixedTick(float d, float SpeedMultiplicationFactor)
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
#endregion

#region POI
class PlayerPOITrackerManager
{
    private PlayerPOITrackerManagerComponent PlayerPOITrackerManagerComponent;
    private SphereCollider TrackerCollider;
    private List<PointOfInterestType> InRangePointOfInterests = new List<PointOfInterestType>();

    public PlayerPOITrackerManager(PlayerPOITrackerManagerComponent playerPOITrackerManagerComponent, SphereCollider TrackerCollider)
    {
        PlayerPOITrackerManagerComponent = playerPOITrackerManagerComponent;
        this.TrackerCollider = TrackerCollider;
        this.TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
    }

    public void Tick(float d)
    {
        TrackerCollider.radius = PlayerPOITrackerManagerComponent.SphereDetectionRadius;
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

    public PointOfInterestType GetNearestPOI()
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

    public void OnGizmoTick()
    {
        if (PlayerPOITrackerManagerComponent != null && TrackerCollider != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(TrackerCollider.transform.position, PlayerPOITrackerManagerComponent.SphereDetectionRadius);
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
            }
        }
        else
        {
            IsLookingToPOI = false;
        }
    }

    private void ResetInterpolatedBoneRotationToActual()
    {
        for (var i = 0; i < playerPOIVisualHeadMovementComponent.BonesThatReactToPOI.Length; i++)
        {
            var affectedBone = playerPOIVisualHeadMovementComponent.BonesThatReactToPOI[i];
            InterpolatedBoneRotations[i] = affectedBone.rotation;
        }
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
}

#endregion

#region Animation
class PlayerAnimationDataManager
{

    public const string SpeedMagnitude = "Speed";

    private Animator Animator;

    public PlayerAnimationDataManager(Animator animator)
    {
        Animator = animator;
    }

    public void Tick(float unscaledSpeedMagnitude)
    {
        Animator.SetFloat(SpeedMagnitude, unscaledSpeedMagnitude);
    }

}
#endregion

#region Context Actions Handler
class PlayerContextActionManager
{
    private bool isActionExecuting;

    public bool IsActionExecuting { get => isActionExecuting; }

    public void OnContextActionAdded(AContextAction contextAction)
    {
        contextAction.OnFinished += () =>
        {
            isActionExecuting = false;
        };
        isActionExecuting = true;
    }
}

#endregion