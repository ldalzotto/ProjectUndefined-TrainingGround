using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PointOfInterestConfiguration", menuName = "Configuration/AdventureGame/PointOfInterestConfiguration/PointOfInterestConfiguration", order = 1)]
    public class PointOfInterestConfiguration : ConfigurationSerialization<PointOfInterestId, PointOfInterestInherentData>
    {
    }
    
}
