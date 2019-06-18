using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public abstract class AGhostPOIManager : MonoBehaviour
    {
        public abstract void Init();
        public abstract void OnPOICreated(APointOfInterestType pointOfInterestType);
        public abstract void OnPOIDisabled(APointOfInterestType pointOfInterestType);
    }
}
