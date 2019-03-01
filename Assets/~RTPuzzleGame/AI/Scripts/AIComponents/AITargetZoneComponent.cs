using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AITargetZoneComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public TargetZoneID TargetZoneID;
        public float TargetZoneMinDistance;
        private TargetZone targetZone;

        #region State
        public bool IsInTargetZone;
        #endregion

        public TargetZone TargetZone { get => targetZone; }
        public void InitializeContainer(AIComponents aIComponents)
        {
            aIComponents.AITargetZoneComponent = this;
            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.targetZone = targetZoneContainer.TargetZones[TargetZoneID];
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
            AITargetZoneComponent.IsInTargetZone = Vector3.Distance(agent.transform.position, AITargetZoneComponent.TargetZone.transform.position) <= AITargetZoneComponent.TargetZoneMinDistance;
        }

    }

}
