using System;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AIPatrolComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIPatrolComponent", order = 1)]
    public class AIPatrolComponent : AbstractAIComponent
    {
        protected override Type abstractManagerType => typeof(AbstractAIPatrolComponentManager);
        public float MaxDistance;
    }

    public abstract class AbstractAIPatrolComponentManager
    {
        #region External Dependencies
        protected NavMeshAgent patrollingAgent;
        protected AIFOVManager AIFOVManager;
        #endregion

        protected AIPatrolComponent AIPatrolComponent;

        protected AbstractAIPatrolComponentManager(NavMeshAgent patrollingAgent, AIPatrolComponent aIPatrolComponent, AIFOVManager aIFOVManager)
        {
            this.patrollingAgent = patrollingAgent;
            AIPatrolComponent = aIPatrolComponent;
            this.AIFOVManager = aIFOVManager;
        }

        public abstract void OnDestinationReached();
        public abstract void OnStateReset();
        public abstract bool IsPatrolling();

        public abstract Vector3? TickComponent();
        public abstract void GizmoTick();
    }

}