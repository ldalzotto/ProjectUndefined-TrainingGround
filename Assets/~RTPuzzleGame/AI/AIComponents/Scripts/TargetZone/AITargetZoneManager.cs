using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AITargetZoneManager : AbstractAITargetZoneManager
    {
        private AiID aiID;
        private Collider aiCollider;

        #region External Dependencies
        private NavMeshAgent agent;
        private PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private TargetZoneContainer targetZoneContainer;
        #endregion

        #region Internal Dependencies
        private AIFOVManager AIFOVManager;
        private AITargetZoneComponent aITargetZoneComponent;
        #endregion

        #region Internal Managers
        private EscapeDestinationManager EscapeDestinationManager;
        #endregion

        public AITargetZoneManager(NavMeshAgent agent, Collider aiCollider, AITargetZoneComponent aITargetZoneComponent, AIFOVManager AIFOVManager, AiID aiID)
        {
            this.agent = agent;
            this.aiCollider = aiCollider;

            this.targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.puzzleGameConfigurationManager = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>();
            this.AIFOVManager = AIFOVManager;
            this.aITargetZoneComponent = aITargetZoneComponent;
            this.EscapeDestinationManager = new EscapeDestinationManager(this.agent);
            this.aiID = aiID;
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

        private TargetZone IsAIOverlappingWithATargetZone()
        {
            var overlappingTargetZones = this.targetZoneContainer.GetAllTargetZonesWhereDistanceCheckOverlaps(this.aiCollider.bounds);
            if (overlappingTargetZones != null && overlappingTargetZones.Count > 0)
            {
                return overlappingTargetZones[0];
            }
            return null;
        }


    }

}
