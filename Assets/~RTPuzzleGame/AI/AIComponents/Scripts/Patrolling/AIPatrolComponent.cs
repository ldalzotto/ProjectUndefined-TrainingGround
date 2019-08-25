using GameConfigurationID;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPatrolComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPatrolComponent", order = 1)]
    public class AIPatrolComponent : AbstractAIComponent
    {
        [CustomEnum()]
        public AIPatrolManagerType AIPatrolManagerType;
        public float MaxDistance;
    }
    
    public enum AIPatrolManagerType
    {
        RANDOM = 0,
        SCRIPTED = 1
    }

    public abstract class AbstractAIPatrolComponentManager : AbstractAIManager<AIPatrolComponent>, InterfaceAIManager
    {
        #region External Dependencies
        protected NavMeshAgent patrollingAgent;
        protected AIFOVManager AIFOVManager;
        #endregion

        protected AIObjectID aiID;

        protected AbstractAIPatrolComponentManager(AIPatrolComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        protected void BaseInit(NavMeshAgent patrollingAgent, AIFOVManager aIFOVManager, AIObjectID aiID)
        {
            this.aiID = aiID;
            this.patrollingAgent = patrollingAgent;
            this.AIFOVManager = aIFOVManager;
        }

        public abstract void OnDestinationReached();
        public abstract void OnStateReset();
        protected abstract bool IsPatrolling();
        public abstract void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext);
        public abstract void GizmoTick();

        public virtual void BeforeManagersUpdate(float d, float timeAttenuationFactor) { }

        public bool IsManagerEnabled()
        {
            return this.IsPatrolling();
        }
        
    }

}