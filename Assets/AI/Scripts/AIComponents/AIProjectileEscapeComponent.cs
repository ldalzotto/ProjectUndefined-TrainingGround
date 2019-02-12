using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    [System.Serializable]
    public class AIProjectileEscapeComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public float EscapeDistance;
        public Transform AvoidPoint;
        public float EscapeZoneMinDistance;
        public Collider AvoidCollider;

        public void InitializeContainer(AIComponents aIComponents)
        {
            aIComponents.AIProjectileEscapeComponent = this;
        }
    }

    public class AIProjectileEscapeManager
    {
        #region External Dependencies
        private NavMeshAgent escapingAgent;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region State
        private bool isEscaping;
        private Nullable<Vector3> escapeDestination;
        #endregion

        private NavMeshHit[] hits = new NavMeshHit[7];
        private Ray[] physicsRay = new Ray[7];

        private Action<Vector3> SetAgentPosition;

        public bool IsEscaping { get => isEscaping; }

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
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

        public Nullable<Vector3> TickComponent()
        {
            return escapeDestination;
        }

        public void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTPProjectile)
            {
                isEscaping = true;
                escapeDestination = ComputeEscapePoint(collider);
            }
        }

        private Nullable<Vector3> ComputeEscapePoint(Collider collider)
        {
            var escapeDirection = (escapingAgent.transform.position - collider.bounds.center).normalized;
            var escapeDirectionProjected = Vector3.ProjectOnPlane(escapeDirection, escapingAgent.transform.up);
            Debug.DrawRay(escapingAgent.transform.position, escapeDirectionProjected * 10, Color.red);


            bool isInWarningZone = Vector3.Distance(escapingAgent.transform.position, AIProjectileEscapeComponent.AvoidPoint.position) < AIProjectileEscapeComponent.EscapeZoneMinDistance;

            if (isInWarningZone)
            {
                hits = new NavMeshHit[7];
                physicsRay = new Ray[7];

                //in warning -> angle of escape detection is greater
                CastRay(escapeDirectionProjected, Quaternion.identity, out hits[0]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, -45, 0), out hits[1]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, -90, 0), out hits[2]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, -110, 0), out hits[3]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, 45, 0), out hits[4]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, 90, 0), out hits[5]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, 110, 0), out hits[6]);

                for (var i = 0; i < hits.Length; i++)
                {
                    physicsRay[i] = new Ray(escapingAgent.transform.position, hits[i].position - escapingAgent.transform.position);
                }
                return EscapeFromExitZone();
            }
            else
            {
                hits = new NavMeshHit[5];
                physicsRay = new Ray[5];

                CastRay(escapeDirectionProjected, Quaternion.identity, out hits[0]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, -45, 0), out hits[1]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, -90, 0), out hits[2]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, 45, 0), out hits[3]);
                CastRay(escapeDirectionProjected, Quaternion.Euler(0, 90, 0), out hits[4]);

                for (var i = 0; i < hits.Length; i++)
                {
                    physicsRay[i] = new Ray(escapingAgent.transform.position, hits[i].position - escapingAgent.transform.position);
                }
                return EscapeToFarestNotInExitZone();
            }

        }

        private Vector3? EscapeFromExitZone()
        {
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToForbidden = 0;
            for (var i = 0; i < hits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIProjectileEscapeComponent.AvoidCollider))
                    {
                        currentDistanceToForbidden = Vector3.Distance(hits[i].position, AIProjectileEscapeComponent.AvoidPoint.position);
                        selectedPosition = hits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIProjectileEscapeComponent.AvoidCollider))
                    {
                        var computedDistance = Vector3.Distance(hits[i].position, AIProjectileEscapeComponent.AvoidPoint.position);
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

        private Vector3? EscapeToFarestNotInExitZone()
        {
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < hits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIProjectileEscapeComponent.AvoidCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(hits[i].position, escapingAgent.transform.position);
                        selectedPosition = hits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(physicsRay[i], hits[i].position, AIProjectileEscapeComponent.AvoidCollider))
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

        private void CastRay(Vector3 escapeDirectionProjected, Quaternion rotation, out NavMeshHit hit)
        {
            var rayTargetPosition = escapingAgent.transform.position + (rotation * (escapeDirectionProjected * AIProjectileEscapeComponent.EscapeDistance));
            NavMesh.Raycast(escapingAgent.transform.position, rayTargetPosition, out hit, NavMesh.AllAreas);
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
