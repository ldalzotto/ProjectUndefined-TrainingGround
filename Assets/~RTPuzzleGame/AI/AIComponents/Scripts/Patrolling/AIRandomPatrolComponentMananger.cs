using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AIRandomPatrolComponentMananger : AbstractAIPatrolComponentManager
    {

        private bool isMovingTowardsDestination;
        private NavMeshHit[] navMeshHits = new NavMeshHit[8];

        private Vector3? currentDestination;

        public void Init(NavMeshAgent patrollingAgent, AIPatrolComponent AIRandomPatrolComponent, AIFOVManager aIFOVManager)
        {
            this.BaseInit(patrollingAgent, AIRandomPatrolComponent, aIFOVManager);
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            if (!isMovingTowardsDestination)
            {
                this.currentDestination = SetRandomDestination();
            }
            return this.currentDestination.Value;
        }

        private Nullable<Vector3> SetRandomDestination()
        {

            navMeshHits = this.AIFOVManager.NavMeshRaycastSample(8, patrollingAgent.transform, this.AIPatrolComponent.MaxDistance);

            var maxDistance = 0f;
            Nullable<NavMeshHit> selectedHit = null; ;
            for (var i = 0; i < navMeshHits.Length; i++)
            {
                if (i == 0)
                {
                    maxDistance = navMeshHits[i].distance;
                    selectedHit = navMeshHits[i];
                }
                else
                {
                    if (navMeshHits[i].distance > maxDistance)
                    {
                        maxDistance = navMeshHits[i].distance;
                        selectedHit = navMeshHits[i];
                    }
                }
            }

            if (selectedHit == null)
            {
                return null;
            }
            else
            {
                isMovingTowardsDestination = true;
                return selectedHit.Value.position;
            }
        }

        public override void GizmoTick()
        {
            for (var i = 0; i < navMeshHits.Length; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(navMeshHits[i].position, 1f);
                Gizmos.color = Color.white;
            }
        }

        #region External Events
        public override void OnDestinationReached()
        {
            this.OnStateReset();
        }
        public override void OnStateReset()
        {
            isMovingTowardsDestination = false;
        }
        #endregion

        #region Logical Conditions
        protected override bool IsPatrolling()
        {
            return isMovingTowardsDestination;
        }
        #endregion
    }

}
