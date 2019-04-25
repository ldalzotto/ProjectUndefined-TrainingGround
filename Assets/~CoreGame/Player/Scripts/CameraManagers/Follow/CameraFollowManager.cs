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

        public CameraFollowManager(Transform playerPosition, Transform cameraPivotPoint, CameraFollowManagerComponent CameraFollowManagerComponent)
        {
            this.playerPosition = playerPosition;
            this.cameraPivotPoint = cameraPivotPoint;
            this.CameraFollowManagerComponent = CameraFollowManagerComponent;
        }

        public void Tick(float d)
        {
            cameraPivotPoint.position = Vector3.SmoothDamp(cameraPivotPoint.position, playerPosition.position, ref currentVelocity, this.CameraFollowManagerComponent.DampTime);
        }
    }
}