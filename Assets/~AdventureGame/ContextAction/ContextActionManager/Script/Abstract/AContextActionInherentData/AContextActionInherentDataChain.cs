using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AContextActionInherentDataChain", menuName = "Configuration/AdventureGame/ContextAction/AContextActionInherentDataChain")]
    public class AContextActionInherentDataChain : SerializedScriptableObject
    {
        public List<AContextActionInherentData> ContextActionChain;
    }

}
