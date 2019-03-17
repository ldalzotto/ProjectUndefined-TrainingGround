using ConfigurationEditor;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AttractiveObjectConfiguration", menuName = "Configuration/PuzzleGame/AttractiveObjectConfiguration/AttractiveObjectConfiguration", order = 1)]
    public class AttractiveObjectConfiguration : ConfigurationSerialization<AttractiveObjectId, AttractiveObjectInherentConfigurationData>
    {    }
    
    public enum AttractiveObjectId
    {
        CHEESE = 0
    }
}
