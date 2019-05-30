using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;

namespace AdventureGame
{
    [System.Serializable]
    public abstract class AContextActionInherentData : SerializedScriptableObject
    {
        public abstract AContextAction BuildContextAction();
    }


}
