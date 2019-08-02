using UnityEngine;
using System.Collections;
using ConfigurationEditor;
using GameConfigurationID;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "ObjectRepelConfiguration", menuName = "Configuration/PuzzleGame/InteractiveObjects/ObjectRpelConfiguration/ObjectRpelConfiguration", order = 1)]
    public class ObjectRepelConfiguration : ConfigurationSerialization<ObjectRepelID, ObjectRepelInherentData>
    {  }
}