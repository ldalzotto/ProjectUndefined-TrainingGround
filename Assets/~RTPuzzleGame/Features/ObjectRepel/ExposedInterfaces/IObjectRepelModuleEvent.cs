using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IObjectRepelModuleEvent
    {
        void OnObjectRepelRepelled(Vector3 targetWorldPosition);
    }
}
