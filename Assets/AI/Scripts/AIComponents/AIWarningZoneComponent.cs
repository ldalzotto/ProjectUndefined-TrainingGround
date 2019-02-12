using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIWarningZoneComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public Transform WarningPoint;
        public float WarningZoneMinDistance;
        public Collider WarningZoneCollider;

        #region State
        public bool IsInWarningZone;
        #endregion

        public void InitializeContainer(AIComponents aIComponents)
        {
            aIComponents.AIWarningZoneComponent = this;
        }
    }

    public class AIWarningZoneComponentManager
    {
        #region External Dependencies
        private NavMeshAgent agent;
        #endregion

        #region Internal Dependencies
        private AIWarningZoneComponent AIWarningZoneComponent;
        #endregion

        #region Logical Conditions
        public bool IsInWarningZone()
        {
            return AIWarningZoneComponent.IsInWarningZone;
        }
        #endregion

        public AIWarningZoneComponentManager(NavMeshAgent agent, AIWarningZoneComponent aIWarningZoneComponent)
        {
            this.agent = agent;
            AIWarningZoneComponent = aIWarningZoneComponent;
        }

        public void TickComponent()
        {
            AIWarningZoneComponent.IsInWarningZone = Vector3.Distance(agent.transform.position, AIWarningZoneComponent.WarningPoint.position) <= AIWarningZoneComponent.WarningZoneMinDistance;
        }

    }

}
