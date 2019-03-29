using CoreGame;
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
        Func<TargetZone> levelTargetZoneProvider;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        private NavMeshHit[] NavMeshhits;
        private Ray[] PhysicsRay;

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AIFOVManager AIFOVManager, PuzzleEventsManager PuzzleEventsManager, AiID aiID, Func<TargetZone> levelTargetZoneProvider)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
            this.AIFOVManager = AIFOVManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.aiID = aiID;
            this.levelTargetZoneProvider = levelTargetZoneProvider;
        }

        public override void ClearEscapeDestination()
        {
            escapeDestination = null;
        }

        public override Nullable<Vector3> GetCurrentEscapeDirection()
        {
            return escapeDestination;
        }

        private void SetIsEscapingFromProjectile(bool value)
        {
            if (this.isEscapingFromProjectile && !value)
            {
                this.PuzzleEventsManager.OnAiAffectedByProjectileEnd(this.aiID);
            }
            this.isEscapingFromProjectile = value;
        }

        public override void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            var escapeDirection = (escapingAgent.transform.position - collider.transform.position).normalized;
            escapeDestination = ComputeEscapePoint(escapeDirection, LaunchProjectile.GetFromCollisionType(collisionType));
            this.SetIsEscapingFromProjectile(true);
        }

        private Nullable<Vector3> ComputeEscapePoint(Vector3 escapeDirection, LaunchProjectile launchProjectile)
        {
            if (launchProjectile != null)
            {
                if (isEscapingFromProjectile)
                {
                    //if already escaping from projectile
                    Debug.Log("EscapeToFarest");
                    this.PuzzleEventsManager.OnAiHittedByProjectile(this.aiID, 2);
                    return EscapeToFarest(escapeDirection, launchProjectile.LaunchProjectileInherentData.EscapeSemiAngle);
                }
                else
                {
                    Debug.Log("EscapeToFarestWithTargetZone");
                    this.PuzzleEventsManager.OnAiHittedByProjectile(this.aiID, 1);
                    return EscapeToFarestWithTargetZone(escapeDirection, launchProjectile.LaunchProjectileInherentData.EscapeSemiAngle);
                }
            }

            return null;

        }

        private Vector3? EscapeToFarestWithTargetZone(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            NavMeshhits = new NavMeshHit[5];
            PhysicsRay = new Ray[5];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
            NavMeshhits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);

            for (var i = 0; i < NavMeshhits.Length; i++)
            {
                PhysicsRay[i] = new Ray(escapingAgent.transform.position, NavMeshhits[i].position - escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < NavMeshhits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsHelper.PhysicsRayInContactWithCollider(PhysicsRay[i], NavMeshhits[i].position, this.levelTargetZoneProvider.Invoke().ZoneCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(NavMeshhits[i].position, escapingAgent.transform.position);
                        selectedPosition = NavMeshhits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsHelper.PhysicsRayInContactWithCollider(PhysicsRay[i], NavMeshhits[i].position, this.levelTargetZoneProvider.Invoke().ZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(NavMeshhits[i].position, escapingAgent.transform.position);
                        if (currentDistanceToRaycastTarget < computedDistance)
                        {
                            selectedPosition = NavMeshhits[i].position;
                            currentDistanceToRaycastTarget = computedDistance;
                        }

                    }
                }
            }

            return selectedPosition;
        }

        private Vector3? EscapeToFarest(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            NavMeshhits = new NavMeshHit[5];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
            NavMeshhits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);

            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < NavMeshhits.Length; i++)
            {
                if (i == 0)
                {
                    currentDistanceToRaycastTarget = Vector3.Distance(NavMeshhits[i].position, escapingAgent.transform.position);
                    selectedPosition = NavMeshhits[i].position;
                }
                else
                {
                    var computedDistance = Vector3.Distance(NavMeshhits[i].position, escapingAgent.transform.position);
                    if (currentDistanceToRaycastTarget < computedDistance)
                    {
                        selectedPosition = NavMeshhits[i].position;
                        currentDistanceToRaycastTarget = computedDistance;
                    }
                }
            }

            return selectedPosition;
        }

        public override void OnDestinationReached()
        {
            AIFOVManager.ResetFOV();
            this.SetIsEscapingFromProjectile(false);
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
            if (escapeDestination.HasValue)
            {
                Gizmos.DrawWireSphere(escapeDestination.Value, 2f);
            }
            if (NavMeshhits != null)
            {
                DrawHits(NavMeshhits);
            }
            if (PhysicsRay != null)
            {
                DrawRays(PhysicsRay);
            }

        }

        private void DrawRays(Ray[] rays)
        {
            for (var i = 0; i < rays.Length; i++)
            {
                var ray = rays[i];

                Gizmos.color = Color.yellow;
                Gizmos.DrawRay(new Ray(ray.origin, ray.direction * 1000));
                Gizmos.color = Color.white;
            }
        }

        private void DrawHits(NavMeshHit[] hits)
        {
            for (var i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(hit.position, 1f);
                Gizmos.color = Color.white;
            }
        }
        #endregion


    }
}
