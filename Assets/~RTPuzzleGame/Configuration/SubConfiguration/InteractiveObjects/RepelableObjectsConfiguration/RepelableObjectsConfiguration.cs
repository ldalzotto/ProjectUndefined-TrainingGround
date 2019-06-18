using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RepelableObjectsConfiguration", menuName = "Configuration/PuzzleGame/InteractiveObjects/RepelableObjetsConfiguration/RepelableObjectsConfiguration", order = 1)]
    public class RepelableObjectsConfiguration : ConfigurationSerialization<RepelableObjectID, RepelableObjectsInherentData>
    {  }
}