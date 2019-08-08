using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIScriptedPatrolComponentManager : AbstractAIPatrolComponentManager
    {
        private bool isPatrolling;

        private AIPositionsManager aIPositionsManager;

        private Vector3 anchorPosition;

        public void Init(NavMeshAgent patrollingAgent, AIPatrolComponent aIPatrolComponent, AIFOVManager aIFOVManager, AiID aiID, AIPositionsManager aIPositionsManager)
        {
            this.BaseInit(patrollingAgent, aIPatrolComponent, aIFOVManager, aiID);
            this.aIPositionsManager = aIPositionsManager;
            this.anchorPosition = aIPositionsManager.GetAIPositions(aiID).GetPosition(AIPositionMarkerID._1_Town_GardenWatchman_1).transform.position;
        }

        public override void GizmoTick() { }

        public override void OnDestinationReached()
        {
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            this.isPatrolling = true;
            if (this.patrollingAgent.transform.position != this.anchorPosition)
            {
                return this.anchorPosition;
            }
            return null;
        }

        public override void OnStateReset()
        {
            this.isPatrolling = false;
        }

        protected override bool IsPatrolling()
        {
            return isPatrolling;
        }
    }

}
