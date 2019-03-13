using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AITargetZoneComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AITargetZoneComponent", order = 1)]
    public class AITargetZoneComponent : ScriptableObject
    {
        public TargetZoneID TargetZoneID;

        private TargetZoneInherentData targetZoneConfigurationData;
        private TargetZone targetZone;
     
        #region State
        private bool isInTargetZone;
        #endregion
   
        public TargetZone TargetZone { get => targetZone; }
        public TargetZoneInherentData TargetZoneConfigurationData { get => targetZoneConfigurationData; }
        public bool IsInTargetZone { get => isInTargetZone; }

        public void Init()
        {
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

        private bool isInTargetZone;

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
        }

        public void TickComponent()
        {
            this.isInTargetZone = Vector3.Distance(agent.transform.position, AITargetZoneComponent.TargetZone.transform.position)
                                                <= AITargetZoneComponent.TargetZoneConfigurationData.EscapeMinDistance;
        }

    }

}
