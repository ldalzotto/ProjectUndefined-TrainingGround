using CoreGame;
using System;
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

        public TargetZoneInherentData TargetZoneConfigurationData { get => targetZoneConfigurationData; }


        public AITargetZoneManager(NavMeshAgent agent, AITargetZoneComponent aITargetZoneComponent, AIFOVManager AIFOVManager)
        {
            this.agent = agent;
            this.aITargetZoneComponentManagerDataRetrieval = new AITargetZoneComponentManagerDataRetrieval(this);

            var targetZoneContainer = GameObject.FindObjectOfType<TargetZoneContainer>();
            this.targetZone = targetZoneContainer.TargetZones[aITargetZoneComponent.TargetZoneID];
            this.targetZoneConfigurationData = GameObject.FindObjectOfType<PuzzleGameConfigurationManager>().TargetZonesConfiguration()[this.targetZone.TargetZoneID];
            this.AIFOVManager = AIFOVManager;
            this.aITargetZoneComponent = aITargetZoneComponent;
        }

        private NavMeshHit[] noTargetZonehits;
        private Ray[] noTargetZonePhysicsRay;

        public override void TickComponent()
        {
            this.isInTargetZone = Vector3.Distance(agent.transform.position, this.GetTargetZone().transform.position)
                                                <= this.TargetZoneConfigurationData.EscapeMinDistance;
        }

        public override void OnDestinationReached()
        {
            isEscapingFromTargetZone = false;
            this.ClearEscapeDestination();
        }

        public override Vector3? TriggerTargetZoneEscape()
        {
            isEscapingFromTargetZone = true;
            this.escapeDestination = EscapeFromExitZone((agent.transform.position - this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().transform.position).normalized);
            return this.escapeDestination;
        }

        private Vector3? EscapeFromExitZone(Vector3 localEscapeDirection)
        {

            noTargetZonehits = new NavMeshHit[7];
            noTargetZonePhysicsRay = new Ray[7];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, agent);
            // Debug.DrawRay(escapingAgent.transform.position, localEscapeDirection, Color.green, 1f);

            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - this.aITargetZoneComponentManagerDataRetrieval.GetTargetZoneConfigurationData().EscapeFOVSemiAngle,
                worldEscapeDirectionAngle + this.aITargetZoneComponentManagerDataRetrieval.GetTargetZoneConfigurationData().EscapeFOVSemiAngle);
            noTargetZonehits = AIFOVManager.NavMeshRaycastSample(7, agent.transform, aITargetZoneComponent.TargetZoneEscapeDistance);

            for (var i = 0; i < noTargetZonehits.Length; i++)
            {
                noTargetZonePhysicsRay[i] = new Ray(agent.transform.position, noTargetZonehits[i].position - agent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToForbidden = 0;
            for (var i = 0; i < noTargetZonehits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsHelper.PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().ZoneCollider))
                    {
                        currentDistanceToForbidden = Vector3.Distance(noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().transform.position);
                        selectedPosition = noTargetZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsHelper.PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().ZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().transform.position);
                        if (currentDistanceToForbidden < computedDistance)
                        {
                            selectedPosition = noTargetZonehits[i].position;
                            currentDistanceToForbidden = computedDistance;
                        }

                    }
                }
            }

            return selectedPosition;
        }

    }

}
