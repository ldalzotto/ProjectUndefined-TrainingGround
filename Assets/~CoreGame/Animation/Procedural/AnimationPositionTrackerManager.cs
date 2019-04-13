using UnityEngine;

namespace CoreGame
{
    public class AnimationPositionTrackerManager
    {
        private GameObject trackedObject;

        public AnimationPositionTrackerManager(GameObject trackedObject)
        {
            this.trackedObject = trackedObject;
            this.animationPositionTrackerInformations = new AnimationPositionTrackerInformations();
        }

        private AnimationPositionTrackerInformations animationPositionTrackerInformations;

        public AnimationPositionTrackerInformations AnimationPositionTrackerInformations { get => animationPositionTrackerInformations; }

        public void LateTick(float d)
        {
            this.animationPositionTrackerInformations.CurrentDeltaTime = d;

            //last frame info
            this.animationPositionTrackerInformations.LastFrameWorldPos = this.animationPositionTrackerInformations.CurrentFrameWorldPos;
            this.animationPositionTrackerInformations.LastFrameLocalEulerAngles = this.animationPositionTrackerInformations.CurrentFrameLocalEulerAngles;
            this.animationPositionTrackerInformations.LastFrameCrossedDistanceSigned = this.animationPositionTrackerInformations.CrossedDistanceSigned;
            this.animationPositionTrackerInformations.LastFrameCurrentSpeedSigned = this.animationPositionTrackerInformations.CurrentSpeedSigned;

            this.animationPositionTrackerInformations.CurrentFrameWorldPos = this.trackedObject.transform.position;
            this.animationPositionTrackerInformations.CurrentFrameLocalEulerAngles = this.trackedObject.transform.localEulerAngles;

            //current frame calculation
            if (this.animationPositionTrackerInformations.LastFrameWorldPos.HasValue)
            {
                this.animationPositionTrackerInformations.CrossedDistanceSigned = this.animationPositionTrackerInformations.CurrentFrameWorldPos - this.animationPositionTrackerInformations.LastFrameWorldPos.Value;
                this.animationPositionTrackerInformations.CurrentSpeedSigned = this.animationPositionTrackerInformations.CrossedDistanceSigned / this.animationPositionTrackerInformations.CurrentDeltaTime;
            }
        }

    }

    public struct AnimationPositionTrackerInformations
    {
        public float CurrentDeltaTime;

        public Vector3 CurrentFrameWorldPos;
        public Vector3 CurrentFrameLocalEulerAngles;

        public Vector3? CrossedDistanceSigned;
        public Vector3? CurrentSpeedSigned;

        public Vector3? LastFrameWorldPos;
        public Vector3? LastFrameLocalEulerAngles;

        public Vector3? LastFrameCrossedDistanceSigned;
        public Vector3? LastFrameCurrentSpeedSigned;

    }
}

