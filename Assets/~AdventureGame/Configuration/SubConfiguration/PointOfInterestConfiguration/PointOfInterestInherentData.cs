using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PointOfInterestInherentData", menuName = "Configuration/AdventureGame/PointOfInterestConfiguration/PointOfInterestInherentData", order = 1)]
    public class PointOfInterestInherentData : ScriptableObject
    {
        public bool InteractionWithPlayerAllowed = true;

        [FormerlySerializedAs("MaxDistanceToInteractWithPlayer")]
        public float MaxDistanceToInteract;
        [Tooltip("This angle is the maximum value for the tracker system to be enabled. The angle is Ang(player forward, player to POI)")]
        public float POIDetectionAngleLimit = 90f;

        public bool IsPersistantToPuzzle = false;
        public bool IsAlwaysDisplayed = false;
    }

}
