using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace CoreGame
{
    public abstract class APointOfInterestType : MonoBehaviour
    {
        public abstract void Init();

        public abstract void Init_EndOfFrame();

        public abstract void Tick(float d);

        public abstract void OnPOIDisabled();
        public abstract void OnPOIEnabled();
        public abstract void SetAnimationPosition(AnimationID animationID);
    }

}
