using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface InterfaceAIManager
    {
        void BeforeManagersUpdate(float d, float timeAttenuationFactor);
        bool IsManagerEnabled();
        Vector3? OnManagerTick(float d, float timeAttenuationFactor);
        void OnDestinationReached();
        void OnStateReset();
    }
}
