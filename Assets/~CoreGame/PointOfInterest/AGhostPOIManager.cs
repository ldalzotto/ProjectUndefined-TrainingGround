using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;

namespace CoreGame
{
    public abstract class AGhostPOIManager : MonoBehaviour
    {
        public abstract void Init();
        public abstract void OnPOICreated(APointOfInterestType pointOfInterestType);
        public abstract void OnPOIDisabled(APointOfInterestType pointOfInterestType);
        public abstract List<PointOfInterestDefinitionID> GetAllPOIIdElligibleToBeDynamicallyInstanciated(List<LevelZoneChunkID>  levelZoneChunkIDs);
    }
}
