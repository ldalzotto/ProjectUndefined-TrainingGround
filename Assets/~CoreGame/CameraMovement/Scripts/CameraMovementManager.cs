﻿using UnityEngine;

namespace CoreGame
{
    public class CameraMovementManager : MonoBehaviour
    {
        public CameraFollowManagerComponent CameraFollowManagerComponent;
        public CameraOrientationManagerComponent CameraOrientationManagerComponent;

        private CameraFollowManager CameraFollowManager;
        private CameraOrientationManager CameraOrientationManager;

        public void Init()
        {
            var playerPosition = GameObject.FindGameObjectWithTag(TagConstants.PLAYER_TAG).transform;
            var cameraPivotPoint = GameObject.FindGameObjectWithTag(TagConstants.CAMERA_PIVOT_POINT_TAG).transform;
            var gameInputManager = GameObject.FindObjectOfType<GameInputManager>();

            this.CameraFollowManager = new CameraFollowManager(playerPosition, cameraPivotPoint, CameraFollowManagerComponent);
            this.CameraOrientationManager = new CameraOrientationManager(cameraPivotPoint, gameInputManager, CameraOrientationManagerComponent);
        }

        public void Tick(float d)
        {
            this.CameraFollowManager.Tick(d);
            this.CameraOrientationManager.Tick(d);
        }

        #region External Events
        public void SetCameraFollowTarget(Transform followTarget)
        {
            this.CameraFollowManager.SetCameraFollowTarget(followTarget);
        }
        public void SetTargetAngle(float targetAngle)
        {
            this.CameraOrientationManager.SetTargetAngle(targetAngle);
        }
        #endregion

        #region Logical conditions
        public bool IsCameraRotating()
        {
            return this.CameraOrientationManager.IsRotating;
        }
        #endregion
    }

    [System.Serializable]
    public class CameraFollowManagerComponent
    {
        public float DampTime;
    }

    public class CameraFollowManager
    {
        private CameraFollowManagerComponent CameraFollowManagerComponent;

        private Transform targetTrasform;
        private Transform cameraPivotPoint;

        private Vector3 currentVelocity;

        public CameraFollowManager(Transform targetTransform, Transform cameraPivotPoint, CameraFollowManagerComponent CameraFollowManagerComponent)
        {
            this.targetTrasform = targetTransform;
            this.cameraPivotPoint = cameraPivotPoint;
            this.CameraFollowManagerComponent = CameraFollowManagerComponent;
            //set initial position
            this.cameraPivotPoint.position = this.targetTrasform.position;
        }

        public void Tick(float d)
        {
            cameraPivotPoint.position = Vector3.SmoothDamp(cameraPivotPoint.position, targetTrasform.position, ref currentVelocity, this.CameraFollowManagerComponent.DampTime);
        }

        public void SetCameraFollowTarget(Transform followTarget)
        {
            this.targetTrasform = followTarget;
        }

        #region Data Retrieval
        public Transform GetCameraPivotPointTransform()
        {
            return this.cameraPivotPoint;
        }
        #endregion
    }

    [System.Serializable]
    public class CameraOrientationManagerComponent
    {
        public float CameraRotationSpeed;
    }
    public class CameraOrientationManager
    {
        private CameraOrientationManagerComponent CameraOrientationManagerComponent;
        private Transform cameraPivotPoint;
        private IGameInputManager gameInputManager;

        private float targetAngle;

        #region State
        private bool isRotating = false;
        private bool isRotatingTowardsAtarget = false;
        #endregion

        public CameraOrientationManager(Transform cameraPivotPoint, IGameInputManager gameInputManager, CameraOrientationManagerComponent CameraOrientationManagerComponent)
        {
            this.cameraPivotPoint = cameraPivotPoint;
            this.gameInputManager = gameInputManager;
            this.CameraOrientationManagerComponent = CameraOrientationManagerComponent;
        }

        public bool IsRotating { get => isRotating; }

        public void Tick(float d)
        {
            Vector3 rotationVector;
            if (this.isRotatingTowardsAtarget)
            {
                float initialY = this.cameraPivotPoint.transform.rotation.eulerAngles.y;
                float deltaAngle = Mathf.Lerp(initialY, this.targetAngle, 0.1f) - initialY;
                rotationVector = new Vector3(0, Mathf.Abs(deltaAngle) * Mathf.Sign(this.targetAngle - initialY), 0);
            }
            else
            {
                rotationVector = new Vector3(0, (gameInputManager.CurrentInput.LeftRotationCameraDH() - gameInputManager.CurrentInput.RightRotationCameraDH()) * d * this.CameraOrientationManagerComponent.CameraRotationSpeed, 0);
            }
            
            if (Mathf.Abs(rotationVector.y) <= 0.001)
            {
                if (this.isRotatingTowardsAtarget)
                {
                    cameraPivotPoint.eulerAngles = new Vector3(0, this.targetAngle, 0);
                }
                this.isRotatingTowardsAtarget = false;
                this.isRotating = false;
            }
            else
            {
                this.isRotating = true;
                cameraPivotPoint.eulerAngles += rotationVector;
            }
        }

        public void SetTargetAngle(float targetAngle)
        {
            this.targetAngle = targetAngle;
            this.isRotatingTowardsAtarget = true;
        }
    }
}
