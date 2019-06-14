using UnityEngine;
using System.Collections;
using ConfigurationEditor;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "RepelableObjectsConfiguration", menuName = "Configuration/PuzzleGame/InteractiveObjects/RepelableObjetsConfiguration/RepelableObjectsConfiguration", order = 1)]
    public class RepelableObjectsConfiguration : ConfigurationSerialization<RepelableObjectID, RepelableObjectsInherentData>
    {  }

    public enum RepelableObjectID
    {
        CHEESE_SEWER_RTP_2
    }
}