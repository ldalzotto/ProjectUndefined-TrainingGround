using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AITargetZoneEscapeManager : AbstractAITargetZoneManager
    {
        private AIObjectID aiID;
        private Collider aiCollider;

        #region External Dependencies
        private NavMeshAgent agent;
        private PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        #region Internal Dependencies
        private AIFOVManager AIFOVManager;
        #endregion

        #region Internal Managers
        private EscapeDestinationManager EscapeDestinationManager;

        #endregion
        
        public AITargetZoneEscapeManager(AITargetZoneComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public void Init(NavMeshAgent agent, Collider aiCollider, AIFOVManager AIFOVManager, AIObjectID aiID)
        {
            this.agent = agent;
            this.aiCollider = aiCollider;

            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            this.puzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            this.AIFOVManager = AIFOVManager;
            this.EscapeDestinationManager = new EscapeDestinationManager(this.agent);
            this.aiID = aiID;
        }

        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            NPCAIDestinationContext.TargetPosition = this.EscapeDestinationManager.Tick();
        }

        public override void OnDestinationReached()
        {
            if (this.EscapeDestinationManager.OnAgentDestinationReached())
            {
                if (this.isEscapingFromTargetZone)
                {
                    var overlappingTargetZone = this.IsAIOverlappingWithATargetZone();
                    if (overlappingTargetZone != null)
                    {
                        this.TriggerTargetZoneEscape(overlappingTargetZone);
                    }
                    else
                    {
                        this.OnStateReset();
                        isEscapingFromTargetZone = false;
                    }
                }
                else
                {
                    this.OnStateReset();
                    isEscapingFromTargetZone = false;
                }
            }
            else
            {
                this.CalculateEscapeDirection();
            }
        }

        public override void OnStateReset()
        {
            this.EscapeDestinationManager.OnStateReset();
            isEscapingFromTargetZone = false;
        }

        public override void TriggerTargetZoneEscape(TargetZoneModule targetZone)
        {
            Debug.Log(Time.frameCount + "Target zone trigger : " + targetZone.TargetZoneID);

            var targetZoneConfigurationData = this.puzzleGameConfigurationManager.TargetZonesConfiguration()[targetZone.TargetZoneID];

            this.isEscapingFromTargetZone = true;
            this.EscapeDestinationManager.ResetDistanceComputation(this.AssociatedAIComponent.TargetZoneEscapeDistance);

            AIFOVManager.IntersectFOV_FromEscapeDirection(targetZone.transform.position, agent.transform.position, targetZoneConfigurationData.EscapeFOVSemiAngle);

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

        private TargetZoneModule IsAIOverlappingWithATargetZone()
        {
            var overlappingTargetZones = TargetZoneHelper.GetAllTargetZonesWhereDistanceCheckOverlaps(this.aiCollider.bounds, this.InteractiveObjectContainer);
            if (overlappingTargetZones != null && overlappingTargetZones.Count > 0)
            {
                return overlappingTargetZones[0];
            }
            return null;
        }

    }

}
