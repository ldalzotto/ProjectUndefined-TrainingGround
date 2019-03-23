using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "AIProjectileEscapeComponent", menuName = "Configuration/PuzzleGame/AIComponentsConfiguration/AIProjectileEscapeComponent", order = 1)]
    public class AIProjectileEscapeComponent : ScriptableObject
    {
        public float EscapeDistance;

    }

    public class AIProjectileEscapeManager
    {
        private AiID aiID;

        #region External Dependencies
        private NavMeshAgent escapingAgent;
        private AITargetZoneComponent AITargetZoneComponent;
        private TargetZoneContainer TargetZoneContainer;
        private AIFOVManager AIFOVManager;
        private PuzzleEventsManager PuzzleEventsManager;
        private AITargetZoneComponentManagerDataRetrieval aITargetZoneComponentManagerDataRetrieval;
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
                AITargetZoneComponent AITargetZoneComponent, AIFOVManager AIFOVManager, PuzzleEventsManager PuzzleEventsManager, AiID aiID, 
                AITargetZoneComponentManagerDataRetrieval aITargetZoneComponentManagerDataRetrieval)
        {
            this.AIProjectileEscapeComponent = AIProjectileEscapeComponent;
            this.escapingAgent = escapingAgent;
            this.AITargetZoneComponent = AITargetZoneComponent;
            this.AIFOVManager = AIFOVManager;
            this.PuzzleEventsManager = PuzzleEventsManager;
            this.aiID = aiID;
            this.aITargetZoneComponentManagerDataRetrieval = aITargetZoneComponentManagerDataRetrieval;
        }

        public void ClearEscapeDestination()
        {
            escapeDestination = null;
        }

        public Nullable<Vector3> ForceComputeEscapePoint()
        {
            isEscapingFromTargetZone = true;
            escapeDestination = ComputeEscapePoint((escapingAgent.transform.position - this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().transform.position).normalized, null);
            return escapeDestination;
        }

        public Nullable<Vector3> GetCurrentEscapeDirection()
        {
            return escapeDestination;
        }

        private void SetIsEscapingFromProjectile(bool value)
        {
            if(this.isEscapingFromProjectile && !value)
            {
                this.PuzzleEventsManager.OnAiAffectedByProjectileEnd(this.aiID);
            }
            this.isEscapingFromProjectile = value;
        }

        public void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            var escapeDirection = (escapingAgent.transform.position - collider.transform.position/*.bounds.center*/).normalized;
            escapeDestination = ComputeEscapePoint(escapeDirection, LaunchProjectile.GetFromCollisionType(collisionType));
            this.SetIsEscapingFromProjectile(true);
        }

        private Nullable<Vector3> ComputeEscapePoint(Vector3 escapeDirection, LaunchProjectile launchProjectile)
        {

            if (this.aITargetZoneComponentManagerDataRetrieval.IsInTargetZone() && launchProjectile == null)
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

            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - this.aITargetZoneComponentManagerDataRetrieval.GetTargetZoneConfigurationData().EscapeFOVSemiAngle,
                worldEscapeDirectionAngle + this.aITargetZoneComponentManagerDataRetrieval.GetTargetZoneConfigurationData().EscapeFOVSemiAngle);
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
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().ZoneCollider))
                    {
                        currentDistanceToForbidden = Vector3.Distance(noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().transform.position);
                        selectedPosition = noTargetZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().ZoneCollider))
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
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().ZoneCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(noTargetZonehits[i].position, escapingAgent.transform.position);
                        selectedPosition = noTargetZonehits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsRayInContactWithCollider(noTargetZonePhysicsRay[i], noTargetZonehits[i].position, this.aITargetZoneComponentManagerDataRetrieval.GetTargetZone().ZoneCollider))
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
            this.SetIsEscapingFromProjectile(false);
        }

        public void OnLaunchProjectileDestroyed(LaunchProjectile launchProjectile)
        {
        }

        public void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            this.SetIsEscapingFromProjectile(false);
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
