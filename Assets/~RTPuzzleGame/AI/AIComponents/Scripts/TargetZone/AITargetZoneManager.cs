using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AITargetZoneManager : AbstractAITargetZoneManager
    {
        #region External Dependencies
        private NavMeshAgent agent;
        #endregion

        #region Internal Dependencies
        private TargetZoneInherentData targetZoneConfigurationData;

        private AIFOVManager AIFOVManager;
        private AITargetZoneComponent aITargetZoneComponent;
        #endregion

        #region Internal Managers
        private EscapeDestinationManager EscapeDestinationManager;
        #endregion

        public TargetZoneInherentData TargetZoneConfigurationData { get => targetZoneConfigurationData; }


        public AITargetZoneManager(NavMeshAgent agent, AITargetZoneComponent aITargetZoneComponent, AIFOVManager AIFOVManager)
        {
            this.agent = agent;

            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.targetZone = targetZoneContainer.TargetZones[aITargetZoneComponent.TargetZoneID];
            this.targetZoneConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().TargetZonesConfiguration()[this.targetZone.TargetZoneID];
            this.AIFOVManager = AIFOVManager;
            this.aITargetZoneComponent = aITargetZoneComponent;
            this.EscapeDestinationManager = new EscapeDestinationManager(this.agent);
        }

        public override Vector3? GetCurrentEscapeDestination()
        {
            return this.EscapeDestinationManager.EscapeDestination;
        }

        public override void TickComponent()
        {
            this.EscapeDestinationManager.Tick();
            this.isInTargetZone = Vector3.Distance(agent.transform.position, this.GetTargetZone().transform.position)
                                                <= this.TargetZoneConfigurationData.EscapeMinDistance;
        }

        public override void OnDestinationReached()
        {
            this.EscapeDestinationManager.OnAgentDestinationReached();
            if (this.EscapeDestinationManager.IsDistanceReached())
            {
                if (this.isEscapingFromTargetZone)
                {
                    this.AIFOVManager.ResetFOV();
                }
                isEscapingFromTargetZone = false;
            }
            else
            {
                this.CalculateEscapeDirection();
            }
        }

        public override Vector3? TriggerTargetZoneEscape()
        {
            Debug.Log(Time.frameCount + " : Trigger target zone");
            isEscapingFromTargetZone = true;
            this.EscapeDestinationManager.ResetDistanceComputation(aITargetZoneComponent.TargetZoneEscapeDistance);

            var localEscapeDirection = (agent.transform.position - this.targetZone.transform.position).normalized;
            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, agent);

            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - this.targetZoneConfigurationData.EscapeFOVSemiAngle,
                worldEscapeDirectionAngle + this.targetZoneConfigurationData.EscapeFOVSemiAngle);
            this.CalculateEscapeDirection();
            return this.EscapeDestinationManager.EscapeDestination;
        }

        private void CalculateEscapeDirection()
        {
            this.EscapeDestinationManager.EscapeDestinationCalculationStrategy(
                escapeDestinationCalculationMethod: (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
                {
                    this.EscapeDestinationManager.EscapeToFarest(7, navMeshRaycastStrategy, this.AIFOVManager);
                },
                ifAllFailsAction: null
             );
        }


    }

}
