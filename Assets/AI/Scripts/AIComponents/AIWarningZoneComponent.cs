using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIWarningZoneComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public Transform WarningPoint;
        public float WarningZoneMinDistance;

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

        #region State
        private bool isInWarningZone;
        #endregion

        #region Logical Conditions
        public bool IsInWarningZone()
        {
            return isInWarningZone;
        }
        #endregion

        public AIWarningZoneComponentManager(NavMeshAgent agent, AIWarningZoneComponent aIWarningZoneComponent)
        {
            this.agent = agent;
            AIWarningZoneComponent = aIWarningZoneComponent;
        }

        public void TickComponent()
        {
            isInWarningZone = Vector3.Distance(agent.transform.position, AIWarningZoneComponent.WarningPoint.position) <= AIWarningZoneComponent.WarningZoneMinDistance;
        }

        public Transform GetWarningPoint()
        {
            return AIWarningZoneComponent.WarningPoint;
        }

    }

}
