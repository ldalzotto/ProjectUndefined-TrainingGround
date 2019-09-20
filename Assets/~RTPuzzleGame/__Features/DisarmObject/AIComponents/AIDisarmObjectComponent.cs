//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RTPuzzle
{
    using UnityEngine;
    using System;


    [Serializable()]
    [ModuleMetadata("AI", "Send disarm events to interactive object.")]
    [CreateAssetMenu(fileName = "AIDisarmObjectComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIDisarmObjectComponent", order = 1)]
    public class AIDisarmObjectComponent : AbstractAIComponent
    {
        public override InterfaceAIManager BuildManager()
        {
            return new AIDisarmObjectManager(this);
        }
    }

    public abstract class AbstractAIDisarmObjectManager : AbstractAIManager<AIDisarmObjectComponent>, InterfaceAIManager
    {
        protected AbstractAIDisarmObjectManager(AIDisarmObjectComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public abstract void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData);

        public abstract void BeforeManagersUpdate(float d, float timeAttenuationFactor);
        
        public abstract bool IsManagerEnabled();

        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        
        public abstract void OnDestinationReached();
        
        public abstract void OnStateReset();

        #region External Evetns      
        public abstract void OnDisarmingObjectStart(IDisarmObjectModuleDataRetrieval disarmingObject);
        public abstract void OnDisarmingObjectExit(IDisarmObjectModuleDataRetrieval disarmingObject);
        #endregion
    }
}
