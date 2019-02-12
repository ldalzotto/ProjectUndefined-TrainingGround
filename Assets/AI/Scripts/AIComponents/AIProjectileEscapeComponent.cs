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
        private AIWarningZoneComponent AIWarningZoneComponent;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region State
        private bool isEscaping;
        private bool isInWarningZone;
        private Nullable<Vector3> escapeDestination;
        #endregion

        private NavMeshHit[] hits = new NavMeshHit[7];
        private Ray[] physicsRay = new Ray[7];

        public bool IsEscaping { get => isEscaping; }

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent, AIWarningZoneComponent AIWarningZoneComponent)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
            this.AIWarningZoneComponent = AIWarningZoneComponent;
        }

        public void ClearEscapeDestination()
        {
            escapeDestination = null;
        }

        public void GizmoTick()
        {
            if (escapeDestination.HasValue)
            {
                Gizmos.DrawWireSphere(escapeDestination.Value, 2f);

                for (var i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(hit.position, 1f);
                    Gizmos.color = Color.white;
                }

                for (var i = 0; i < physicsRay.Length; i++)
                {
                    var ray = physicsRay[i];

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawRay(new Ray(ray.origin, ray.direction * 1000));
                    Gizmos.color = Color.white;
                }
            }
        }

        public Nullable<Vector3> ForceComputeEscapePoint()
        {
            isEscaping = true;
            escapeDestination = ComputeEscapePoint((AIWarningZoneComponent.WarningPoint.position - escapingAgent.transform.position).normalized);
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
                isEscaping = true;
                var escapeDirection = (escapingAgent.transform.position - collider.bounds.center).normalized;
                escapeDestination = ComputeEscapePoint(escapeDirection);
            }
        }

        private Nullable<Vector3> ComputeEscapePoint(Vector3 escapeDirection)
        {
            var escapeDirectionProjected = Vector3.ProjectOnPlane(escapeDirection, escapingAgent.transform.up);
            Debug.DrawRay(escapingAgent.transform.position, escapeDirectionProjected * 10, Color.red);


            if (AIWarningZoneComponent.IsInWarningZone)
            {
                return EscapeFromExitZone(escapeDirectionProjected);
            }
            else
            {
                return EscapeToFarestNotInExitZone(escapeDirectionProjected);
            }

        }

        private Vector3? EscapeFromExitZone(Vector3 escapeDirectionProjected)
        {
            hits = new NavMeshHit[7];
            physicsRay = new Ray[7];

            //in warning -> angle of escape detection is greater
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.identity, AIProjectileEscapeComponent.EscapeDistance, out hits[0]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, -45, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[1]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, -90, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[2]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, -110, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[3]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, 45, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[4]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, 90, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[5]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, 110, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[6]);

            for (var i = 0; i < hits.Length; i++)
            {
                physicsRay[i] = new Ray(escapingAgent.transform.position, hits[i].position - escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToForbidden = 0;
            for (var i = 0; i < hits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIWarningZoneComponent.WarningZoneCollider))
                    {
                        currentDistanceToForbidden = Vector3.Distance(hits[i].position, AIWarningZoneComponent.WarningPoint.position);
                        selectedPosition = hits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIWarningZoneComponent.WarningZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(hits[i].position, AIWarningZoneComponent.WarningPoint.position);
                        if (currentDistanceToForbidden < computedDistance)
                        {
                            selectedPosition = hits[i].position;
                            currentDistanceToForbidden = computedDistance;
                        }

                    }
                }
            }

            return selectedPosition;
        }

        private Vector3? EscapeToFarestNotInExitZone(Vector3 escapeDirectionProjected)
        {
            hits = new NavMeshHit[5];
            physicsRay = new Ray[5];

            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.identity, AIProjectileEscapeComponent.EscapeDistance, out hits[0]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, -45, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[1]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, -90, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[2]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, 45, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[3]);
            NavMeshRayCaster.CastNavMeshRay(escapingAgent.transform.position, escapeDirectionProjected, Quaternion.Euler(0, 90, 0), AIProjectileEscapeComponent.EscapeDistance, out hits[4]);

            for (var i = 0; i < hits.Length; i++)
            {
                physicsRay[i] = new Ray(escapingAgent.transform.position, hits[i].position - escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < hits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIWarningZoneComponent.WarningZoneCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(hits[i].position, escapingAgent.transform.position);
                        selectedPosition = hits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIWarningZoneComponent.WarningZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(hits[i].position, escapingAgent.transform.position);
                        if (currentDistanceToRaycastTarget < computedDistance)
                        {
                            selectedPosition = hits[i].position;
                            currentDistanceToRaycastTarget = computedDistance;
                        }

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
            isEscaping = false;
        }

        public void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {
        }

        public void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTPProjectile)
            {
                isEscaping = false;
            }
        }



    }

}
