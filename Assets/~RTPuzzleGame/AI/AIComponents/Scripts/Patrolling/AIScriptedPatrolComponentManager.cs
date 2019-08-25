using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIScriptedPatrolComponentManager : AbstractAIPatrolComponentManager
    {
        private bool isPatrolling;

        private AIPositionsManager aIPositionsManager;

        private Transform anchorPosition;

        public AIScriptedPatrolComponentManager(AIPatrolComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public void Init(NavMeshAgent patrollingAgent, AIFOVManager aIFOVManager, AIObjectID aiID, AIPositionsManager aIPositionsManager)
        {
            this.BaseInit(patrollingAgent, aIFOVManager, aiID);
            this.aIPositionsManager = aIPositionsManager;

            var aiPositions = aIPositionsManager.GetAIPositions(aiID);
            if (aiPositions != null)
            {
                var anchorPositionMarker = aiPositions.GetPosition(AIPositionMarkerID._1_Town_GardenWatchman_1);
                if (anchorPositionMarker != null)
                {
                    this.anchorPosition = anchorPositionMarker.transform;
                }
            }

        }

        public override void GizmoTick() { }

        public override void OnDestinationReached()
        {
        }

        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            this.isPatrolling = true;
            if (this.anchorPosition != null)
            {
                NPCAIDestinationContext.TargetPosition = this.anchorPosition.position;
                NPCAIDestinationContext.TargetRotation = this.anchorPosition.rotation;
            }
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
