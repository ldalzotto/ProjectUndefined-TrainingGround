using UnityEngine;

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

        #region State
        private bool isRotating = false;
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
            var rotationVector = new Vector3(0, (gameInputManager.CurrentInput.LeftRotationCameraDH() - gameInputManager.CurrentInput.RightRotationCameraDH()) * d * this.CameraOrientationManagerComponent.CameraRotationSpeed, 0);
            this.isRotating = false;
            if (Mathf.Abs(rotationVector.y) >= float.Epsilon)
            {
                this.isRotating = true;
            }
            cameraPivotPoint.eulerAngles += rotationVector;
        }
    }
}
