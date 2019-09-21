using UnityEngine;
using System.Collections;
using OdinSerializer;

namespace AdventureGame
{
    public class PointOfInterestTrackerModuleDefinition : SerializedScriptableObject
    {
        [WireCircle(R = 0, G = 1, B = 1)]
        public float SphereDetectionRadius = 25f;
    }
}
