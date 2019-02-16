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
        #region External Dependencies
        private NavMeshAgent escapingAgent;
        private AITargetZoneComponent AIWarningZoneComponent;
        private AIFOVManager AIFOVManager;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region State
        private bool isEscapingFromWarningZone;
        private bool isEscapingFromProjectile;
        private bool isInWarningZone;
        private Nullable<Vector3> escapeDestination;
        #endregion

        private NavMeshHit[] noWarningZonehits;
        private Ray[] noWarningZonePhysicsRay;
        private NavMeshHit[] warningZonehits;
        private Ray[] warningZonePhysicsRay;

        #region Logical Conditions
        public bool IsEscaping()
        {
            return isEscapingFromWarningZone || isEscapingFromProjectile;
        }
        #endregion

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AITargetZoneComponent AIWarningZoneComponent, AIFOVManager AIFOVManager)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
            this.AIWarningZoneComponent = AIWarningZoneComponent;
            this.AIFOVManager = AIFOVManager;
        }

        public void ClearEscapeDestination()
        {
            escapeDestination = null;
        }

        public Nullable<Vector3> ForceComputeEscapePoint()
        {
            isEscapingFromWarningZone = true;
            escapeDestination = ComputeEscapePoint((AIWarningZoneComponent.TargetZone.transform.position - escapingAgent.transform.position).normalized, null);
            return escapeDestination;
        }

        public Nullable<Vector3> GetCurrentEscapeDirection()
        {
            return escapeDestination;
        }

        public void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTPProjectile)
            {
                var escapeDirection = (escapingAgent.transform.position - collider.bounds.center).normalized;
                escapeDestination = ComputeEscapePoint(escapeDirection, LaunchProjectile.GetFromCollisionType(collisionType));
                isEscapingFromProjectile = true;
            }
        }

        private Nullable<Vector3> ComputeEscapePoint(Vector3 escapeDirection, LaunchProjectile launchProjectile)
        {

            if (AIWarningZoneComponent.IsInTargetZone && launchProjectile == null)
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
                        return EscapeToFarest(escapeDirection, launchProjectile.LaunchProjectileInherentData.EscapeSemiAngle);
                    }
                    else
                    {
                        Debug.Log("EscapeToFarestWithWarningZone");
                        return EscapeToFarestWithWarningZone(escapeDirection, launchProjectile.LaunchProjectileInherentData.EscapeSemiAngle);
                    }

                }
            }

            return null;

        }

        private Vector3? EscapeFromExitZone(Vector3 localEscapeDirection)
        {
            noWarningZonehits = new NavMeshHit[7];
            noWarningZonePhysicsRay = new Ray[7];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVWorld(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - 110, worldEscapeDirectionAngle + 110);
            noWarningZonehits = AIFOVManager.NavMeshRaycastSample(7, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);

            for (var i = 0; i < noWarningZonehits.Length; i++)
            {
                noWarningZonePhysicsRay[i] = new Ray(escapingAgent.transform.position, noWarningZonehits[i].position - escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToForbidden = 0;
            for (var i = 0; i < noWarningZonehits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.TargetZone.ZoneCollider))
                    {
                        currentDistanceToForbidden = Vector3.Distance(noWarningZonehits[i].position, AIWarningZoneComponent.TargetZone.transform.position);
                        selectedPosition = noWarningZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.TargetZone.ZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(noWarningZonehits[i].position, AIWarningZoneComponent.TargetZone.transform.position);
                        if (currentDistanceToForbidden < computedDistance)
                        {
                            selectedPosition = noWarningZonehits[i].position;
                            currentDistanceToForbidden = computedDistance;
                        }

                    }
                }
            }

            return selectedPosition;
        }

        private Vector3? EscapeToFarestWithWarningZone(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            noWarningZonehits = new NavMeshHit[5];
            noWarningZonePhysicsRay = new Ray[5];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVWorld(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
            noWarningZonehits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);

            for (var i = 0; i < noWarningZonehits.Length; i++)
            {
                noWarningZonePhysicsRay[i] = new Ray(escapingAgent.transform.position, noWarningZonehits[i].position - escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < noWarningZonehits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.TargetZone.ZoneCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(noWarningZonehits[i].position, escapingAgent.transform.position);
                        selectedPosition = noWarningZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.TargetZone.ZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(noWarningZonehits[i].position, escapingAgent.transform.position);
                        if (currentDistanceToRaycastTarget < computedDistance)
                        {
                            selectedPosition = noWarningZonehits[i].position;
                            currentDistanceToRaycastTarget = computedDistance;
                        }

                    }
                }
            }

            return selectedPosition;
        }

        private Vector3? EscapeToFarest(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            noWarningZonehits = new NavMeshHit[5];

            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVWorld(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
            noWarningZonehits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, AIProjectileEscapeComponent.EscapeDistance);


            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < noWarningZonehits.Length; i++)
            {
                if (i == 0)
                {
                    currentDistanceToRaycastTarget = Vector3.Distance(noWarningZonehits[i].position, escapingAgent.transform.position);
                    selectedPosition = noWarningZonehits[i].position;
                }
                else
                {
                    var computedDistance = Vector3.Distance(noWarningZonehits[i].position, escapingAgent.transform.position);
                    if (currentDistanceToRaycastTarget < computedDistance)
                    {
                        selectedPosition = noWarningZonehits[i].position;
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
            isEscapingFromWarningZone = false;
            isEscapingFromProjectile = false;
        }

        public void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {
        }

        public void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTPProjectile)
            {
                isEscapingFromProjectile = false;
            }
        }

        #region Gizmo
        public void GizmoTick()
        {
            if (escapeDestination.HasValue)
            {
                Gizmos.DrawWireSphere(escapeDestination.Value, 2f);
            }
            if (noWarningZonehits != null)
            {
                DrawHits(noWarningZonehits);
            }
            if (noWarningZonePhysicsRay != null)
            {
                DrawRays(noWarningZonePhysicsRay);
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
