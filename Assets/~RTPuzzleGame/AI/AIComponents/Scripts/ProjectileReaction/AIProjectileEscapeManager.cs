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

        #region Internal Managers
        private EscapeDistanceManager EscapeDistanceManager;
        #endregion

        #region Internal State
        private bool escapingWithTargetZone;
        #endregion

        #region Internal Events
        private void OnDestinationSetFromProjectileContact(Vector3 nextDestination)
        {
            this.EscapeDistanceManager.OnDestinationSetFromProjectileContact(nextDestination);
        }
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

            this.EscapeDistanceManager = new EscapeDistanceManager(escapingAgent, this.AIProjectileEscapeComponent);
        }

        public override Nullable<Vector3> TickComponent()
        {
            this.EscapeDistanceManager.TickComponent();
            return escapeDestination;
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
            }
            this.isEscapingFromProjectile = value;
        }

        public override void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            var escapeDirection = (escapingAgent.transform.position - collider.transform.position).normalized;
            escapeDestination = ComputeEscapePoint(escapeDirection, LaunchProjectile.GetFromCollisionType(collisionType));
            this.SetIsEscapingFromProjectile(true);
        }

        private Nullable<Vector3> ComputeEscapePoint(Vector3 localEscapeDirection, LaunchProjectile launchProjectile)
        {
            Nullable<Vector3> nextDestination = null;
            if (launchProjectile != null)
            {
                IntersectFOV(localEscapeDirection, launchProjectile.LaunchProjectileInherentData.EscapeSemiAngle);
                if (isEscapingFromProjectile)
                {
                    //if already escaping from projectile
                    Debug.Log("EscapeToFarest");
                    this.PuzzleEventsManager.OnAiHittedByProjectile(this.aiID, 2);
                    nextDestination = EscapeToFarest(AIProjectileEscapeComponent.EscapeDistance);
                }
                else
                {
                    Debug.Log("EscapeToFarestWithTargetZone");
                    this.PuzzleEventsManager.OnAiHittedByProjectile(this.aiID, 1);
                    nextDestination = EscapeToFarestWithTargetZone(AIProjectileEscapeComponent.EscapeDistance);
                    this.escapingWithTargetZone = true;
                }
            }

            if (nextDestination.HasValue)
            {
                Debug.Log("Destination set projectile");
                this.OnDestinationSetFromProjectileContact(nextDestination.Value);
            }

            return nextDestination;

        }

        private Vector3? EscapeToFarestWithTargetZone(float maxEscapeDistance)
        {
            NavMeshhits = new NavMeshHit[5];
            PhysicsRay = new Ray[5];


            NavMeshhits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, maxEscapeDistance);

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

        private void IntersectFOV(Vector3 localEscapeDirection, float semiAngleEscape)
        {
            var worldEscapeDirectionAngle = FOVLocalToWorldTransformations.AngleFromDirectionInFOVSpace(localEscapeDirection, escapingAgent);
            AIFOVManager.IntersectFOV(worldEscapeDirectionAngle - semiAngleEscape, worldEscapeDirectionAngle + semiAngleEscape);
        }

        private Vector3? EscapeToFarest(float maxEscapeDistance)
        {
            NavMeshhits = new NavMeshHit[5];
            NavMeshhits = AIFOVManager.NavMeshRaycastSample(5, escapingAgent.transform, maxEscapeDistance);
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
            this.EscapeDistanceManager.OnOnDestinationReached();
            if (this.EscapeDistanceManager.IsDistanceReached)
            {
                //if travelled escape distance is reached, we reset
                ResetAIProjectileEscapeManagerState();
            }
            else
            {
                //if travelled escape distance is not reached
                Nullable<Vector3> remainingEscapeDestination = null;
                if (this.escapingWithTargetZone)
                {
                    remainingEscapeDestination = this.EscapeToFarestWithTargetZone(this.EscapeDistanceManager.GetRemainingDistance());
                }
                else
                {
                    remainingEscapeDestination = this.EscapeToFarest(this.EscapeDistanceManager.GetRemainingDistance());
                }

                Debug.Log("Escape destination : " + this.escapeDestination.ToString() + " remaining destination : " + remainingEscapeDestination.Value.ToString());

                if (this.escapingAgent.destination != remainingEscapeDestination)
                {
                    this.escapeDestination = remainingEscapeDestination;
                }
                else //we cancel the remaining destination
                {
                    this.ResetAIProjectileEscapeManagerState();
                }
            }
        }

        private void ResetAIProjectileEscapeManagerState()
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

    class EscapeDistanceManager
    {
        private NavMeshAgent escapingAgnet;
        private AIProjectileEscapeComponent aIProjectileEscapeComponent;

        public EscapeDistanceManager(NavMeshAgent escapingAgent, AIProjectileEscapeComponent aIProjectileEscapeComponent)
        {
            this.escapingAgnet = escapingAgent;
            this.aIProjectileEscapeComponent = aIProjectileEscapeComponent;
        }

        private bool isDistanceReached = true;
        private Nullable<Vector3> lastFrameAgentPosition;
        private float distanceCounter;

        public bool IsDistanceReached { get => isDistanceReached; }

        public void OnDestinationSetFromProjectileContact(Vector3 nextDestination)
        {
            this.isDistanceReached = false;
            this.lastFrameAgentPosition = null;
            this.distanceCounter = 0f;
        }

        public void TickComponent()
        {
            if (this.lastFrameAgentPosition.HasValue)
            {
                this.distanceCounter += Vector3.Distance(this.lastFrameAgentPosition.Value, this.escapingAgnet.transform.position);
            }
            this.lastFrameAgentPosition = this.escapingAgnet.transform.position;
        }

        public void OnOnDestinationReached()
        {
            if (this.distanceCounter >= this.aIProjectileEscapeComponent.EscapeDistance - this.escapingAgnet.stoppingDistance)
            {
                this.isDistanceReached = true;
            }
        }

        public float GetRemainingDistance()
        {
            return Mathf.Abs(this.aIProjectileEscapeComponent.EscapeDistance - this.escapingAgnet.stoppingDistance - this.distanceCounter);
        }
    }

}
