using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IAILogicColliderModuleDataRetriever 
    {
        IInteractiveObjectTypeDataRetrieval IInteractiveObjectTypeDataRetrieval { get; }
        BoxCollider GetCollider();
    }

}
