using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public class CutsceneGlobalController : MonoBehaviour
    {
        #region External Dependencies
        private CameraMovementManager CameraMovementManager;
        private PointOfInterestManager PointOfInterestManager;
        #endregion

        public void Init()
        {
            this.CameraMovementManager = CameraMovementManager.Get();
            this.PointOfInterestManager = AdventureGameSingletonInstances.PointOfInterestManager;
        }

        #region External Events
        public void SetCameraFollow(PointOfInterestId pointOfInterestID)
        {
            var poiToFollow = this.PointOfInterestManager.GetActivePointOfInterest(pointOfInterestID);
            if (poiToFollow != null)
            {
                this.CameraMovementManager.SetCameraFollowTarget(poiToFollow.GetRootObject().transform);
            }
        }
        public void SetCameraTargetAngle(float targetAngle)
        {
            this.CameraMovementManager.SetTargetAngle(targetAngle);
        }
        #endregion
    }
}
