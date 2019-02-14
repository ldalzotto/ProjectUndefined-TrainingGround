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
        private AIFOVManager AIFOVManager;
        #endregion

        #region Internal Dependencies
        private AIProjectileEscapeComponent AIProjectileEscapeComponent;
        #endregion

        #region State
        private bool isEscaping;
        private bool isInWarningZone;
        private Nullable<Vector3> escapeDestination;
        #endregion

        private NavMeshHit[] noWarningZonehits;
        private Ray[] noWarningZonePhysicsRay;
        private NavMeshHit[] warningZonehits;
        private Ray[] warningZonePhysicsRay;

        public bool IsEscaping { get => isEscaping; }

        public AIProjectileEscapeManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent AIProjectileEscapeComponent,
                AIWarningZoneComponent AIWarningZoneComponent, AIFOVManager AIFOVManager)
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
            noWarningZonehits = new NavMeshHit[7];
            noWarningZonePhysicsRay = new Ray[7];

            var escapeDirectionAngle = Vector3.SignedAngle(escapeDirectionProjected, escapingAgent.transform.forward, escapingAgent.transform.up);
            AIFOVManager.SetAvailableFROVRange(escapeDirectionAngle - 110, escapeDirectionAngle + 110);
            noWarningZonehits = AIFOVManager.NavMeshRaycastSample(7, escapingAgent.transform, Vector3.zero, AIProjectileEscapeComponent.EscapeDistance);

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
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.WarningZoneCollider))
                    {
                        currentDistanceToForbidden = Vector3.Distance(noWarningZonehits[i].position, AIWarningZoneComponent.WarningPoint.position);
                        selectedPosition = noWarningZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.WarningZoneCollider))
                    {
                        var computedDistance = Vector3.Distance(noWarningZonehits[i].position, AIWarningZoneComponent.WarningPoint.position);
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

        private Vector3? EscapeToFarestNotInExitZone(Vector3 escapeDirectionProjected)
        {
            noWarningZonehits = new NavMeshHit[5];
            noWarningZonePhysicsRay = new Ray[5];

            var localEscapeDirectionAngle = Vector3.SignedAngle(escapeDirectionProjected, escapingAgent.transform.forward, escapingAgent.transform.up);
            //  var localToWorldForwardAngle = Vector3.SignedAngle(escapingAgent.transform.forward, Vector3.forward, escapingAgent.transform.up);
            AIFOVManager.SetAvailableFROVRange(localEscapeDirectionAngle - 90, localEscapeDirectionAngle + 90);
            noWarningZonehits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, Vector3.zero, AIProjectileEscapeComponent.EscapeDistance);

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
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.WarningZoneCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(noWarningZonehits[i].position, escapingAgent.transform.position);
                        selectedPosition = noWarningZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noWarningZonePhysicsRay[i], noWarningZonehits[i].position, AIWarningZoneComponent.WarningZoneCollider))
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
