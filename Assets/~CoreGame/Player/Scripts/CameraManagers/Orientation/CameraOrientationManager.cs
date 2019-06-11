using UnityEngine;
using System.Collections;

namespace CoreGame
{
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
