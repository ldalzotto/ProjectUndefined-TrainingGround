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

        public CameraOrientationManager(Transform cameraPivotPoint, IGameInputManager gameInputManager, CameraOrientationManagerComponent CameraOrientationManagerComponent)
        {
            this.cameraPivotPoint = cameraPivotPoint;
            this.gameInputManager = gameInputManager;
            this.CameraOrientationManagerComponent = CameraOrientationManagerComponent;
        }

        public void Tick(float d)
        {
            cameraPivotPoint.eulerAngles += new Vector3(0,
                (gameInputManager.CurrentInput.LeftRotationCameraDH() - gameInputManager.CurrentInput.RightRotationCameraDH()) * d * this.CameraOrientationManagerComponent.CameraRotationSpeed,
                0);
        }
    }

}
