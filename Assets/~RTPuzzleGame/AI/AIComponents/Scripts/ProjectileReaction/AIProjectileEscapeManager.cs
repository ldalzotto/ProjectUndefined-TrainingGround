using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIProjectileEscapeManager : AbstractAIProjectileEscapeManager
    {
        private AiID aiID;

        #region External Dependencies
        private NavMeshAgent escapingAgent;
        private AIFOVManager AIFOVManager;
        private PuzzleEventsManager PuzzleEventsManager;
        private Func<Collider[]> targetZoneTriggerColliderProvider;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region Internal Managers
        private EscapeDestinationManager escapeDestinationManager;
        #endregion

        #region Internal State
        private bool escapingWithTargetZone;
        #endregion

        #region Internal Events
        private void OnDestinationSetFromProjectileContact()
        {
            this.escapeDestinationManager.ResetDistanceComputation(this.AIProjectileEscapeComponent.EscapeDistance);
        }
        #endregion

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AIFOVManager AIFOVManager, PuzzleEventsManager PuzzleEventsManager, AiID aiID, Func<Collider[]> targetZoneTriggerColliderProvider)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
            this.AIFOVManager = AIFOVManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.targetZoneTriggerColliderProvider = targetZoneTriggerColliderProvider;
            this.aiID = aiID;

            this.escapeDestinationManager = new EscapeDestinationManager(escapingAgent);
        }

        public override Nullable<Vector3> TickComponent()
        {
            this.escapeDestinationManager.Tick();
            return escapeDestinationManager.EscapeDestination;
        }

        private void SetIsEscapingFromProjectile(bool value)
        {
            if (this.isEscapingFromProjectile && !value)
            {
                this.PuzzleEventsManager.OnAiAffectedByProjectileEnd(this.aiID);
            }
            if (!value)
            {
                this.escapingWithTargetZone = false;
                this.isEscapingToFarest = false;
            }
            this.isEscapingFromProjectile = value;
        }

        public override void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            var localEscapeDirection = (escapingAgent.transform.position - collider.transform.position).normalized;
            var launchProjectile = LaunchProjectile.GetFromCollisionType(collisionType);
            if (launchProjectile != null)
            {
                this.OnDestinationSetFromProjectileContact();
                this.IntersectFOV(localEscapeDirection, launchProjectile.LaunchProjectileInherentData.EscapeSemiAngle);
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.OnTriggerEnterEscapeDestinationCalculation, null);
            }
            this.SetIsEscapingFromProjectile(true);
        }

        private void IntersectFOV(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
        }

        public override void OnDestinationReached()
        {
            this.escapeDestinationManager.OnAgentDestinationReached();
            if (this.escapeDestinationManager.IsDistanceReached())
            {
                //if travelled escape distance is reached, we reset
                ResetAIProjectileEscapeManagerState();
            }
            else
            {
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.EscapeDestinationOnlyCalculation, this.ResetAIProjectileEscapeManagerState);
            }
        }

        public override void OnStateReset()
        {
            this.SetIsEscapingFromProjectile(false);
        }

        private void OnTriggerEnterEscapeDestinationCalculation(NavMeshRaycastStrategy navMeshRaycastStrategy)
        {
            this.TargetZoneConsiderationBranch(
                    withoutTargetZoneAction: () =>
                    {
                        //if already escaping from projectile
                        Debug.Log("EscapeToFarest");
                        this.PuzzleEventsManager.OnAiHittedByProjectile(this.aiID, 2);
                        this.escapeDestinationManager.EscapeToFarest(5, navMeshRaycastStrategy, this.AIFOVManager);
                        this.isEscapingToFarest = true;
                    },
                    withTargetZoneAction: () =>
                    {
                        Debug.Log("EscapeToFarestWithTargetZone");
                        this.PuzzleEventsManager.OnAiHittedByProjectile(this.aiID, 1);
                        this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.AIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
                        this.escapingWithTargetZone = true;
                    }
                   );
        }

        private void EscapeDestinationOnlyCalculation(NavMeshRaycastStrategy navMeshRaycastStrategy)
        {
            this.TargetZoneConsiderationBranch(
                  withTargetZoneAction: () =>
                  {
                      this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(5, navMeshRaycastStrategy, this.AIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
                  },
                  withoutTargetZoneAction: () =>
                  {
                      this.escapeDestinationManager.EscapeToFarest(5, navMeshRaycastStrategy, this.AIFOVManager);
                  });
        }

        private void TargetZoneConsiderationBranch(Action withTargetZoneAction, Action withoutTargetZoneAction)
        {
            if (this.isEscapingFromProjectile)
            {
                withoutTargetZoneAction.Invoke();
            }
            else
            {
                withTargetZoneAction.Invoke();
            }
        }

        private void ResetAIProjectileEscapeManagerState()
        {
            if (this.isEscapingFromProjectile)
            {
                AIFOVManager.ResetFOV();
            }
            this.OnStateReset();
        }

        public override void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {
        }

        public override void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            this.SetIsEscapingFromProjectile(false);
        }

        #region Gizmo
        public override void GizmoTick()
        {
            this.escapeDestinationManager.GizmoTick();
        }
        #endregion
    }

}
