using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AITargetZoneManager : AbstractAITargetZoneManager
    {
        #region External Dependencies
        private NavMeshAgent agent;
        private PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        #endregion

        #region Internal Dependencies
        private AIFOVManager AIFOVManager;
        private AITargetZoneComponent aITargetZoneComponent;
        #endregion

        #region Internal Managers
        private EscapeDestinationManager EscapeDestinationManager;
        #endregion

        public AITargetZoneManager(NavMeshAgent agent, AITargetZoneComponent aITargetZoneComponent, AIFOVManager AIFOVManager)
        {
            this.agent = agent;

            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.puzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.AIFOVManager = AIFOVManager;
            this.aITargetZoneComponent = aITargetZoneComponent;
            this.EscapeDestinationManager = new EscapeDestinationManager(this.agent);
        }

        public override Nullable<Vector3> TickComponent()
        {
            this.EscapeDestinationManager.Tick();
            return this.EscapeDestinationManager.EscapeDestination;
        }

        public override void OnDestinationReached()
        {
            this.EscapeDestinationManager.OnAgentDestinationReached();
            if (this.EscapeDestinationManager.IsDistanceReached())
            {
                this.OnStateReset();
                isEscapingFromTargetZone = false;
            }
            else
            {
                this.CalculateEscapeDirection();
            }
        }

        public override void OnStateReset()
        {
            isEscapingFromTargetZone = false;
        }

        public override void TriggerTargetZoneEscape(TargetZone targetZone)
        {
            Debug.Log(Time.frameCount + "Target zone trigger : " + targetZone.TargetZoneID);

            var targetZoneConfigurationData = this.puzzleGameConfigurationManager.TargetZonesConfiguration()[targetZone.TargetZoneID];

            this.isEscapingFromTargetZone = true;
            this.EscapeDestinationManager.ResetDistanceComputation(aITargetZoneComponent.TargetZoneEscapeDistance);

            var localEscapeDirection = (agent.transform.position - targetZone.transform.position).normalized;
            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, agent);

            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - targetZoneConfigurationData.EscapeFOVSemiAngle,
                worldEscapeDirectionAngle + targetZoneConfigurationData.EscapeFOVSemiAngle);
            this.CalculateEscapeDirection();
        }

        private void CalculateEscapeDirection()
        {
            this.EscapeDestinationManager.EscapeDestinationCalculationStrategy(
                escapeDestinationCalculationMethod: (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
                {
                    this.EscapeDestinationManager.EscapeToFarest(7, navMeshRaycastStrategy, this.AIFOVManager);
                },
                ifAllFailsAction: this.OnStateReset
             );
        }


    }

}
