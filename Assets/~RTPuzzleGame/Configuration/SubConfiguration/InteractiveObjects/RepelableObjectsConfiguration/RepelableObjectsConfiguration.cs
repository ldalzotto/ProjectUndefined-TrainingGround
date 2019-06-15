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
        CHEESE_SEWER_RTP_2 = 0,
        RTP_PUZZLE_CREATION_TEST = 1,
        CHEESE_SEWER_RTP_2_1 = 2
    }
}