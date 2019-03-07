using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AITargetZoneComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public TargetZoneID TargetZoneID;

        private TargetZoneInherentData targetZoneConfigurationData;
        private TargetZone targetZone;

        #region State
        public bool IsInTargetZone;
        #endregion

        public TargetZone TargetZone { get => targetZone; }
        public TargetZoneInherentData TargetZoneConfigurationData { get => targetZoneConfigurationData; }

        public void InitializeContainer(AIComponents aIComponents)
        {
            aIComponents.AITargetZoneComponent = this;
            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.targetZone = targetZoneContainer.TargetZones[TargetZoneID];
            this.targetZoneConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().TargetZonesConfiguration()[this.targetZone.TargetZoneID];
        }
    }

    public class AITargetZoneComponentManager
    {
        #region External Dependencies
        private NavMeshAgent agent;
        #endregion

        #region Internal Dependencies
        private AITargetZoneComponent AITargetZoneComponent;
        #endregion

        #region Logical Conditions
        public bool IsInTargetZone()
        {
            return AITargetZoneComponent.IsInTargetZone;
        }
        #endregion

        public AITargetZoneComponentManager(NavMeshAgent agent, AITargetZoneComponent aITargetZoneComponent)
        {
            this.agent = agent;
            AITargetZoneComponent = aITargetZoneComponent;
        }

        public void TickComponent()
        {
            AITargetZoneComponent.IsInTargetZone = Vector3.Distance(agent.transform.position, AITargetZoneComponent.TargetZone.transform.position) <= AITargetZoneComponent.TargetZoneConfigurationData.EscapeMinDistance;
        }

    }

}
