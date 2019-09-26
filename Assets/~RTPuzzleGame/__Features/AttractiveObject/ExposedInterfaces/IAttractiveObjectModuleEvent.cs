using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IAttractiveObjectModuleEvent 
    {
        void OnAttractiveObjectPlayerActionExecuted(RaycastHit attractiveObjectWorldPositionHit);
    }
}
