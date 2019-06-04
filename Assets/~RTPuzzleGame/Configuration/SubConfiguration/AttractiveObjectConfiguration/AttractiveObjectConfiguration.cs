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
        CHEESE = 0,
        EDITOR_TEST = 1,
        CHEESE_SEWER_RTP_2 = 2
    }
}
