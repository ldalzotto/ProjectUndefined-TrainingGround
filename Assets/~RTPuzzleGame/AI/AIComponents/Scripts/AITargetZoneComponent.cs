using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : ScriptableObject
    {
        public TargetZoneID TargetZoneID;
    }

    public class AITargetZoneComponentManagerDataRetrieval
    {
        private AITargetZoneComponentManager AITargetZoneComponentManagerRef;

        public AITargetZoneComponentManagerDataRetrieval(AITargetZoneComponentManager aITargetZoneComponentManagerRef)
        {
            AITargetZoneComponentManagerRef = aITargetZoneComponentManagerRef;
        }

        #region Data Retrieval
        public TargetZone GetTargetZone()
        {
            return this.AITargetZoneComponentManagerRef.TargetZone;
        }

        public TargetZoneInherentData GetTargetZoneConfigurationData()
        {
            return this.AITargetZoneComponentManagerRef.TargetZoneConfigurationData;
        }
        #endregion

        #region Logical Conditions
        public bool IsInTargetZone()
        {
            return this.AITargetZoneComponentManagerRef.IsInTargetZone();
        }
        #endregion
    }

    public class AITargetZoneComponentManager
    {
        #region External Dependencies
        private NavMeshAgent agent;
        #endregion

        #region Internal Dependencies
        private AITargetZoneComponent AITargetZoneComponent;
        private AITargetZoneComponentManagerDataRetrieval aITargetZoneComponentManagerDataRetrieval;
        private TargetZoneInherentData targetZoneConfigurationData;
        private TargetZone targetZone;
        #endregion

        #region State
        private bool isInTargetZone;
        #endregion

        public TargetZone TargetZone { get => targetZone; }
        public TargetZoneInherentData TargetZoneConfigurationData { get => targetZoneConfigurationData; }
        public AITargetZoneComponentManagerDataRetrieval AITargetZoneComponentManagerDataRetrieval { get => aITargetZoneComponentManagerDataRetrieval; }

        #region Logical Conditions
        public bool IsInTargetZone()
        {
            return isInTargetZone;
        }
        #endregion

        public AITargetZoneComponentManager(NavMeshAgent agent, AITargetZoneComponent aITargetZoneComponent)
        {
            this.agent = agent;
            AITargetZoneComponent = aITargetZoneComponent;
            this.aITargetZoneComponentManagerDataRetrieval = new AITargetZoneComponentManagerDataRetrieval(this);

            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.targetZone = targetZoneContainer.TargetZones[aITargetZoneComponent.TargetZoneID];
            this.targetZoneConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().TargetZonesConfiguration()[this.targetZone.TargetZoneID];
        }

        public void TickComponent()
        {
            this.isInTargetZone = Vector3.Distance(agent.transform.position, this.TargetZone.transform.position)
                                                <= this.TargetZoneConfigurationData.EscapeMinDistance;
        }

    }

}
