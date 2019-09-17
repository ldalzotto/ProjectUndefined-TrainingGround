using ConfigurationEditor;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DisarmObjectConfiguration", menuName = "Configuration/PuzzleGame/DisarmObjectConfiguration/DisarmObjectConfiguration", order = 1)]
    public class DisarmObjectConfiguration : ConfigurationSerialization<DisarmObjectID, DisarmObjectInherentData>
    {    }
    
 
}
