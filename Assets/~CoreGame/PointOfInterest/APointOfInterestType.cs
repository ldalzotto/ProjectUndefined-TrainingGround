using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace CoreGame
{
    public abstract class APointOfInterestType : MonoBehaviour
    {
        [HideInInspector]
        public PointOfInterestId PointOfInterestId;

        public abstract void Init();

        public abstract void Init_EndOfFrame();

        public abstract void Tick(float d);

        public abstract void OnPOIDisabled(APointOfInterestType disabledPointOfInterest);
        public abstract void OnPOIDestroyed(APointOfInterestType poiToBeDestroyed);
        public abstract void OnPOIEnabled();
        public abstract void SetAnimationPosition(AnimationID animationID);
        public abstract void SetPosition(TransformBinarryFormatted position);
    }

}
