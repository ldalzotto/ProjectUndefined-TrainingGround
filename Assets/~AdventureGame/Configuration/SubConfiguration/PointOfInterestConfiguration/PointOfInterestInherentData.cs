using UnityEngine;
using System.Collections;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PointOfInterestInherentData", menuName = "Configuration/AdventureGame/PointOfInterestConfiguration/PointOfInterestInherentData", order = 1)]
    public class PointOfInterestInherentData : ScriptableObject
    {
        public bool InteractionWithPlayerAllowed = true;
        public float MaxDistanceToInteractWithPlayer;
        public bool IsPersistantToPuzzle = false;
        public bool IsAlwaysDisplayed = false;

        [Tooltip("This is used for cutscene instanciation")]
        public GameObject PointOfInterestPrefab;
    }

}
