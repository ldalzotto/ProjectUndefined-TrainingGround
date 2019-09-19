using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IFovModuleDataRetrieval 
    {
        Vector3 GetRingPositionOffset();
        float GetInteractionRingHeight();
    }
}
