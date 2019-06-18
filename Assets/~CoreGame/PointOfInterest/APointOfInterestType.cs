using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public abstract class APointOfInterestType : MonoBehaviour
    {
        public abstract void Init();

        public abstract void Init_EndOfFrame();

        public abstract void OnPOIDisabled();
        public abstract void OnPOIEnabled();
    }

}
