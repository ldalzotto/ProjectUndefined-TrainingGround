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
        private AITargetZoneComponent AITargetZoneComponent;
        private TargetZoneInherentData targetZoneConfigurationData;
        private TargetZone targetZone;
        #endregion

        public TargetZone TargetZone { get => targetZone; }
        public TargetZoneInherentData TargetZoneConfigurationData { get => targetZoneConfigurationData; }


        public AITargetZoneManager(NavMeshAgent agent, AITargetZoneComponent aITargetZoneComponent)
        {
            this.agent = agent;
            AITargetZoneComponent = aITargetZoneComponent;
            this.aITargetZoneComponentManagerDataRetrieval = new AITargetZoneComponentManagerDataRetrieval(this);

            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.targetZone = targetZoneContainer.TargetZones[aITargetZoneComponent.TargetZoneID];
            this.targetZoneConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().TargetZonesConfiguration()[this.targetZone.TargetZoneID];
        }

        public override void TickComponent()
        {
            this.isInTargetZone = Vector3.Distance(agent.transform.position, this.TargetZone.transform.position)
                                                <= this.TargetZoneConfigurationData.EscapeMinDistance;
        }

    }

}
