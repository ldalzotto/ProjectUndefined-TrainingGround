using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    /// <summary>
    /// An <see cref="InterfaceAIManager"/> is responsible of computing the next AI destination <see cref="M:InterfaceAIManager.OnManagerTick(float, float)"/> when asked.
    /// The manager has an internal state that is used to determine if it is enabled or not <see cref="M:InterfaceAIManager.IsManagerEnabled()"/>.
    /// The AI Manager must not communicate with others (<see cref="BehaviorStateTracker"/> is used especcialy for that.).
    /// </summary>
    public interface InterfaceAIManager
    {
        void BeforeManagersUpdate(float d, float timeAttenuationFactor);
        bool IsManagerEnabled();
        void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        void OnDestinationReached();
        void OnStateReset();
    }
}
