using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class EscapeDestinationManager
    {

        public const float DESTINATION_CALCUALTION_ROUNDING_DISTANCE = 0.001f;

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


        public Vector3? Tick()
        {
            this.EscapeDistanceManager.Tick();
            return this.escapeDestination;
        }

        #region External Events
        public bool OnAgentDestinationReached()
        {
            this.EscapeDistanceManager.OnDestinationReached();
            return this.IsDistanceReached();
        }
        public void ResetDistanceComputation(float totalEscapeDistanceToTravel)
        {
            this.EscapeDistanceManager.ResetDistanceComputation(totalEscapeDistanceToTravel);
        }
        public void OnStateReset()
        {
            this.EscapeDistanceManager.ResetState();
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
        public float GetRemainingDistance()
        {
            return this.EscapeDistanceManager.GetRemainingDistance();
        }
        #endregion

        #region Escape destination computation methods

        public void EscapeDestinationCalculationStrategy(Action<NavMeshRaycastStrategy> escapeDestinationCalculationMethod, Action ifAllFailsAction)
        {
            if (escapeDestinationCalculationMethod != null)
            {
                escapeDestinationCalculationMethod.Invoke(NavMeshRaycastStrategy.RANDOM);

                //if destination calculation failed or the calculated destination is the same as AI position
                if (!this.EscapeDestination.HasValue ||
                            Vector3.Distance(this.escapingAgent.transform.position, this.EscapeDestination.Value) <= DESTINATION_CALCUALTION_ROUNDING_DISTANCE)
                {
                    //we try the end navmesh calculation strategy
                    escapeDestinationCalculationMethod.Invoke(NavMeshRaycastStrategy.END_ANGLES);
                    if (!this.EscapeDestination.HasValue ||
                            Vector3.Distance(this.escapingAgent.transform.position, this.EscapeDestination.Value) <= DESTINATION_CALCUALTION_ROUNDING_DISTANCE)
                    {
                        if (ifAllFailsAction != null)
                        {
                            ifAllFailsAction.Invoke();
                        }
                    }
                }

                if (this.EscapeDestination.HasValue)
                {
                    Debug.Log(MyLog.Format("Escape destination distance : " + Vector3.Distance(this.escapingAgent.transform.position, this.EscapeDestination.Value)));
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
        public Vector3? EscapeToFarestWithCollidersAvoid(int sampleNb, NavMeshRaycastStrategy navMeshRaycastStrategy, AIFOVManager aIFOVManager, Collider[] collidersToAvoid)
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
                    if (!PhysicsHelper.PhysicsRayInContactWithColliders(PhysicsRay[i], NavMeshhits[i].position, collidersToAvoid))
                    {
                        currentDistanceToRaycastTarget = Vector3.Distance(NavMeshhits[i].position, this.escapingAgent.transform.position);
                        selectedPosition = NavMeshhits[i].position;
                    }
                }
                else
                {
                    if (!PhysicsHelper.PhysicsRayInContactWithColliders(PhysicsRay[i], NavMeshhits[i].position, collidersToAvoid))
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

        #region Calculation failed fallback
        public static Action OnDestinationCalculationFailed_ForceAIFear(PuzzleEventsManager puzzleEventsManager, AiID aiID, float fearTime)
        {
            return () => { puzzleEventsManager.PZ_EVT_AI_FearedForced(aiID, fearTime); };
        }

        public static float ForcedFearRemainingDistanceToFearTime(EscapeDestinationManager escapeDestinationManager, AIDestimationMoveManagerComponent aIDestimationMoveManagerComponent)
        {
            return escapeDestinationManager.GetRemainingDistance() / aIDestimationMoveManagerComponent.SpeedMultiplicationFactor;
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
                    //  Debug.Log(Time.frameCount + " : " + this.distanceCounter);
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

        public void ResetState()
        {
            this.isDistanceReached = true;
        }

        public float GetRemainingDistance()
        {
            return Mathf.Abs(this.totalEscapeDistanceToTravel - this.escapingAgnet.stoppingDistance - this.distanceCounter);
        }
    }

}
