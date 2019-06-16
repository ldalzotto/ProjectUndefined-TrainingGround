using UnityEngine;
using System.Collections;

namespace CoreGame
{
    [System.Serializable]
    public class CameraFollowManagerComponent
    {
        public float DampTime;
    }
    public class CameraFollowManager
    {
        private CameraFollowManagerComponent CameraFollowManagerComponent;

        private Transform playerPosition;
        private Transform cameraPivotPoint;

        private Vector3 currentVelocity;

        public CameraFollowManager(Transform playerPosition, Transform cameraPivotPoint, CameraFollowManagerComponent CameraFollowManagerComponent, PlayerPosition persistedPlayerPosition = null)
        {
            this.playerPosition = playerPosition;
            this.cameraPivotPoint = cameraPivotPoint;
            this.CameraFollowManagerComponent = CameraFollowManagerComponent;
            //set initial position
            this.cameraPivotPoint.position = this.playerPosition.position;
            if (persistedPlayerPosition != null)
            {
                this.cameraPivotPoint.rotation = persistedPlayerPosition.GetCameraQuaternion();
            }
        }

        public void Tick(float d)
        {
            cameraPivotPoint.position = Vector3.SmoothDamp(cameraPivotPoint.position, playerPosition.position, ref currentVelocity, this.CameraFollowManagerComponent.DampTime);
        }

        #region Data Retrieval
        public Transform GetCameraPivotPointTransform()
        {
            return this.cameraPivotPoint;
        }
        #endregion
    }
}