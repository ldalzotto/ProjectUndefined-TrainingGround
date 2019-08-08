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
        public float MaxDistance;
    }

    public abstract class AbstractAIPatrolComponentManager : AbstractAIManager, InterfaceAIManager
    {
        #region External Dependencies
        protected NavMeshAgent patrollingAgent;
        protected AIFOVManager AIFOVManager;
        #endregion

        protected AiID aiID;
        protected AIPatrolComponent AIPatrolComponent;

        protected void BaseInit(NavMeshAgent patrollingAgent, AIPatrolComponent aIPatrolComponent, AIFOVManager aIFOVManager, AiID aiID)
        {
            this.aiID = aiID;
            this.patrollingAgent = patrollingAgent;
            AIPatrolComponent = aIPatrolComponent;
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