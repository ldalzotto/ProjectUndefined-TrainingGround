using CoreGame;
using OdinSerializer;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    public class PointOfInterestSharedDataTypeInherentData : SerializedScriptableObject
    {
        public bool InteractionWithPlayerAllowed = true;
        
        [WireArc(R = 1, G = 1, B = 0, Radius = 10f)]
        [Tooltip("This angle is the maximum value for the tracker system to be enabled. The angle is Ang(player forward, player to POI)")]
        public float POIDetectionAngleLimit = 90f;

        public bool IsPersistantToPuzzle = false;
        public bool IsAlwaysDisplayed = false;

        [Foldable()]
        public TransformMoveManagerComponentV3 TransformMoveManagerComponent;
    }
}
