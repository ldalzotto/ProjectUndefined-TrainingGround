using CoreGame;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class EscapeDestinationManager
    {

        private NavMeshAgent escapingAgent;
        private Nullable<Vector3> escapeDestination;

        public EscapeDestinationManager(NavMeshAgent escapingAgent)
        {
            this.escapingAgent = escapingAgent;
            this.EscapeDistanceManager = new EscapeDistanceManager(escapingAgent);
        }

        private EscapeDistanceManager EscapeDistanceManager;

        private NavMeshHit[] NavMeshhits;
        private Ray[] PhysicsRay;


        public void Tick()
        {
            this.EscapeDistanceManager.Tick();
        }

        #region External Events
        public void OnAgentDestinationReached()
        {
            this.EscapeDistanceManager.OnDestinationReached();
        }
        public void ResetDistanceComputation(float totalEscapeDistanceToTravel)
        {
            this.EscapeDistanceManager.ResetDistanceComputation(totalEscapeDistanceToTravel);
        }
        #endregion

        #region Logical conditions
        public bool IsDistanceReached()
        {
            return this.EscapeDistanceManager.IsDistanceReached;
        }
        #endregion

        #region Data retrieval
        public Vector3? EscapeDestination { get => escapeDestination; }
        #endregion

        #region Escape destination computation methods

        public void EscapeDestinationCalculationStrategy(Action<NavMeshRaycastStrategy> escapeDestinationCalculationMethod, Action ifAllFailsAction)
        {
            if (escapeDestinationCalculationMethod != null)
            {
                //if travelled escape distance is not reached
                escapeDestinationCalculationMethod.Invoke(NavMeshRaycastStrategy.RANDOM);
                if (this.escapingAgent.destination == this.EscapeDestination)
                {
                    //we try the end navmesh calculation strategy
                    escapeDestinationCalculationMethod.Invoke(NavMeshRaycastStrategy.END_ANGLES);
                    if (this.escapingAgent.destination == this.EscapeDestination)
                    {
                        if (ifAllFailsAction != null)
                        {
                            ifAllFailsAction.Invoke();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        ///     Calculate the escape point equals to the farest found between all sample.
        /// </para>
        /// </summary>
        /// <param name="navMeshRaycastStrategy"></param>
        /// <param name="aIFOVManager"></param>
        /// <returns></returns>
        public Vector3? EscapeToFarest(int sampleNb, NavMeshRaycastStrategy navMeshRaycastStrategy, AIFOVManager aIFOVManager)
        {
            NavMeshhits = new NavMeshHit[sampleNb];
            switch (navMeshRaycastStrategy)
            {
                case NavMeshRaycastStrategy.RANDOM:
                    NavMeshhits = aIFOVManager.NavMeshRaycastSample(sampleNb, escapingAgent.transform, this.EscapeDistanceManager.GetRemainingDistance());
                    break;
                case NavMeshRaycastStrategy.END_ANGLES:
                    NavMeshhits = aIFOVManager.NavMeshRaycastEndOfRanges(escapingAgent.transform, this.EscapeDistanceManager.GetRemainingDistance());
                    break;
            }

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

            this.escapeDestination = selectedPosition;
            return this.escapeDestination;
        }

        /// <summary>
        /// <para>
        ///     Calculate the escape point eqauls to the farest found between all navmesh hits. 
        ///     Navmesh hits are filtered if it crosses the <paramref name="avoidedCollider"/>.
        /// </para>
        /// </summary>
        /// <param name="navMeshRaycastStrategy"></param>
        /// <param name="aIFOVManager"></param>
        /// <param name="avoidedCollider"></param>
        /// <returns></returns>
        public Vector3? EscapeToFarestWithColliderAvoid(int sampleNb, NavMeshRaycastStrategy navMeshRaycastStrategy, AIFOVManager aIFOVManager, Collider avoidedCollider)
        {
            NavMeshhits = new NavMeshHit[sampleNb];
            PhysicsRay = new Ray[sampleNb];

            switch (navMeshRaycastStrategy)
            {
                case NavMeshRaycastStrategy.RANDOM:
                    NavMeshhits = aIFOVManager.NavMeshRaycastSample(sampleNb, this.escapingAgent.transform, this.EscapeDistanceManager.GetRemainingDistance());
                    break;
                case NavMeshRaycastStrategy.END_ANGLES:
                    NavMeshhits = aIFOVManager.NavMeshRaycastEndOfRanges(this.escapingAgent.transform, this.EscapeDistanceManager.GetRemainingDistance());
                    break;
            }

            for (var i = 0; i < NavMeshhits.Length; i++)
            {
                PhysicsRay[i] = new Ray(this.escapingAgent.transform.position, NavMeshhits[i].position - this.escapingAgent.transform.position);
            }
            Nullable<Vector3> selectedPosition = null;
            float currentDistanceToRaycastTarget = 0f;
            for (var i = 0; i < NavMeshhits.Length; i++)
            {
                if (i == 0)
                {
                    if (!PhysicsHelper.PhysicsRayInContactWithCollider(PhysicsRay[i], NavMeshhits[i].position, avoidedCollider))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(NavMeshhits[i].position, this.escapingAgent.transform.position);
                        selectedPosition = NavMeshhits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsHelper.PhysicsRayInContactWithCollider(PhysicsRay[i], NavMeshhits[i].position, avoidedCollider))
                    {
                        var computedDistance = Vector3.Distance(NavMeshhits[i].position, this.escapingAgent.transform.position);
                        if (currentDistanceToRaycastTarget < computedDistance)
                        {
                            selectedPosition = NavMeshhits[i].position;
                            currentDistanceToRaycastTarget = computedDistance;
                        }

                    }
                }
            }

            this.escapeDestination = selectedPosition;
            return this.escapeDestination;
        }
        #endregion

        #region Gizmo Tick
        public void GizmoTick()
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

    public enum NavMeshRaycastStrategy
    {
        RANDOM = 0,
        END_ANGLES = 1
    }

    class EscapeDistanceManager
    {
        private NavMeshAgent escapingAgnet;

        public EscapeDistanceManager(NavMeshAgent escapingAgent)
        {
            this.escapingAgnet = escapingAgent;
        }

        private bool isDistanceReached = true;
        private Nullable<Vector3> lastFrameAgentPosition;
        private float totalEscapeDistanceToTravel;
        private float distanceCounter;

        public bool IsDistanceReached { get => isDistanceReached; }

        public void ResetDistanceComputation(float totalEscapeDistanceToTravel)
        {
            this.isDistanceReached = false;
            this.lastFrameAgentPosition = null;
            this.distanceCounter = 0f;
            this.totalEscapeDistanceToTravel = totalEscapeDistanceToTravel;
        }

        public void Tick()
        {
            if (!this.isDistanceReached)
            {
                if (this.lastFrameAgentPosition.HasValue)
                {
                    this.distanceCounter += Vector3.Distance(this.lastFrameAgentPosition.Value, this.escapingAgnet.transform.position);
                    Debug.Log(Time.frameCount + " : " + this.distanceCounter);
                }
                this.lastFrameAgentPosition = this.escapingAgnet.transform.position;
            }
        }

        public void OnDestinationReached()
        {
            if (this.distanceCounter >= this.totalEscapeDistanceToTravel - this.escapingAgnet.stoppingDistance)
            {
                this.isDistanceReached = true;
            }
        }

        public float GetRemainingDistance()
        {
            return Mathf.Abs(this.totalEscapeDistanceToTravel - this.escapingAgnet.stoppingDistance - this.distanceCounter);
        }
    }

}
