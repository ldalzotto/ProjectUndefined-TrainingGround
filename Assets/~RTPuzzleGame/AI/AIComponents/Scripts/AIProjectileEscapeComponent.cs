using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    [System.Serializable]
    public class AIProjectileEscapeComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public float EscapeDistance;

        public void InitializeContainer(AIComponents aIComponents)
        {
            aIComponents.AIProjectileEscapeComponent = this;
        }
    }

    public class AIProjectileEscapeManager
    {
        private AiID aiID;

        #region External Dependencies
        private NavMeshAgent escapingAgent;
        private AITargetZoneComponent AITargetZoneComponent;
        private AIFOVManager AIFOVManager;
        private PuzzleEventsManager PuzzleEventsManager;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region State
        private bool isEscapingFromTargetZone;
        private bool isEscapingFromProjectile;
        private bool isInTargetZone;
        private Nullable<Vector3> escapeDestination;
        #endregion

        private NavMeshHit[] noTargetZonehits;
        private Ray[] noTargetZonePhysicsRay;
        private NavMeshHit[] targetZonehits;
        private Ray[] targetZonePhysicsRay;

        #region Logical Conditions
        public bool IsEscaping()
        {
            return isEscapingFromTargetZone || isEscapingFromProjectile;
        }
        #endregion

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AITargetZoneComponent AITargetZoneComponent, AIFOVManager AIFOVManager, PuzzleEventsManager PuzzleEventsManager, AiID aiID)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
            this.AITargetZoneComponent = AITargetZoneComponent;
            this.AIFOVManager = AIFOVManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.aiID = aiID;
        }

        public void ClearEscapeDestination()
        {
            escapeDestination = null;
        }

        public Nullable<Vector3> ForceComputeEscapePoint()
        {
            isEscapingFromTargetZone = true;
            escapeDestination = ComputeEscapePoint((escapingAgent.transform.position - AITargetZoneComponent.TargetZone.transform.position).normalized, null);
            return escapeDestination;
        }

        public Nullable<Vector3> GetCurrentEscapeDirection()
        {
            return escapeDestination;
        }

        public void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            var escapeDirection = (escapingAgent.transform.position - collider.bounds.center).normalized;
            escapeDestination = ComputeEscapePoint(escapeDirection, LaunchProjectile.GetFromCollisionType(collisionType));
            isEscapingFromProjectile = true;
        }

        private Nullable<Vector3> ComputeEscapePoint(Vector3 escapeDirection, LaunchProjectile launchProjectile)
        {

            if (AITargetZoneComponent.IsInTargetZone && launchProjectile == null)
            {
                Debug.Log("EscapeFromExitZone");
                return EscapeFromExitZone(escapeDirection);
            }
            else
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
            }

            return null;

        }

        private Vector3? EscapeFromExitZone(Vector3 localEscapeDirection)
        {

            noTargetZonehits = new NavMeshHit[7];
            noTargetZonePhysicsRay = new Ray[7];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            // Debug.DrawRay(escapingAgent.transform.position, localEscapeDirection, Color.green, 1f);

            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - this.AITargetZoneComponent.TargetZoneConfigurationData.EscapeFOVSemiAngle,
                worldEscapeDirectionAngle + this.AITargetZoneComponent.TargetZoneConfigurationData.EscapeFOVSemiAngle);
            noTargetZonehits = AIFOVManager.NavMeshRaycastSample(7, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);

            for (var i = 0; i < noTargetZonehits.Length; i++)
            {
                noTargetZonePhysicsRay[i] = new Ray(escapingAgent.transform.position, noTargetZonehits[i].position - escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToForbidden = 0;
            for (var i = 0; i < noTargetZonehits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, AITargetZoneComponent.TargetZone.ZoneCollider))
                    {
                        currentDistanceToForbidden = Vector3.Distance(noTargetZonehits[i].position, AITargetZoneComponent.TargetZone.transform.position);
                        selectedPosition = noTargetZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, AITargetZoneComponent.TargetZone.ZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(noTargetZonehits[i].position, AITargetZoneComponent.TargetZone.transform.position);
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

        private Vector3? EscapeToFarestWithTargetZone(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            noTargetZonehits = new NavMeshHit[5];
            noTargetZonePhysicsRay = new Ray[5];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
            noTargetZonehits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);

            for (var i = 0; i < noTargetZonehits.Length; i++)
            {
                noTargetZonePhysicsRay[i] = new Ray(escapingAgent.transform.position, noTargetZonehits[i].position - escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < noTargetZonehits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, AITargetZoneComponent.TargetZone.ZoneCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(noTargetZonehits[i].position, escapingAgent.transform.position);
                        selectedPosition = noTargetZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, AITargetZoneComponent.TargetZone.ZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(noTargetZonehits[i].position, escapingAgent.transform.position);
                        if (currentDistanceToRaycastTarget < computedDistance)
                        {
                            selectedPosition = noTargetZonehits[i].position;
                            currentDistanceToRaycastTarget = computedDistance;
                        }

                    }
                }
            }

            return selectedPosition;
        }

        private Vector3? EscapeToFarest(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            noTargetZonehits = new NavMeshHit[5];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
            noTargetZonehits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);


            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < noTargetZonehits.Length; i++)
            {
                if (i == 0)
                {
                    currentDistanceToRaycastTarget = Vector3.Distance(noTargetZonehits[i].position, escapingAgent.transform.position);
                    selectedPosition = noTargetZonehits[i].position;
                }
                else
                {
                    var computedDistance = Vector3.Distance(noTargetZonehits[i].position, escapingAgent.transform.position);
                    if (currentDistanceToRaycastTarget < computedDistance)
                    {
                        selectedPosition = noTargetZonehits[i].position;
                        currentDistanceToRaycastTarget = computedDistance;
                    }
                }
            }

            return selectedPosition;
        }

        private bool PhysicsRayInContactWithCollider(Ray ray, Vector3 targetPoint, Collider collider)
        {
            var raycastHits = Physics.RaycastAll(ray, Vector3.Distance(ray.origin, targetPoint));
            for (var i = 0; i < raycastHits.Length; i++)
            {
                if (raycastHits[i].collider.GetInstanceID() == collider.GetInstanceID())
                {
                    return true;
                }
            }
            return false;
        }

        internal void OnDestinationReached()
        {
            AIFOVManager.ResetFOV();
            isEscapingFromTargetZone = false;
            isEscapingFromProjectile = false;
        }

        public void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {
        }

        public void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            isEscapingFromProjectile = false;
        }

        #region Gizmo
        public void GizmoTick()
        {
            if (escapeDestination.HasValue)
            {
                Gizmos.DrawWireSphere(escapeDestination.Value, 2f);
            }
            if (noTargetZonehits != null)
            {
                DrawHits(noTargetZonehits);
            }
            if (noTargetZonePhysicsRay != null)
            {
                DrawRays(noTargetZonePhysicsRay);
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
