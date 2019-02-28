﻿using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    [System.Serializable]
    public class AIRandomPatrolComponent : MonoBehaviour, AIComponentInitializerMessageReceiver
    {
        public float MaxDistance;

        public void InitializeContainer(AIComponents aIComponents)
        {
            aIComponents.AIRandomPatrolComponent = this;
        }

    }

    public class AIRandomPatrolComponentMananger
    {
        #region External Dependencies
        private NavMeshAgent patrollingAgent;
        #endregion

        private AIRandomPatrolComponent AIRandomPatrolComponent;

        private bool isMovingTowardsDestination;
        private NavMeshHit[] navMeshHits = new NavMeshHit[8];

        public AIRandomPatrolComponentMananger(NavMeshAgent patrollingAgent, AIRandomPatrolComponent AIRandomPatrolComponent)
        {
            this.patrollingAgent = patrollingAgent;
            this.AIRandomPatrolComponent = AIRandomPatrolComponent;
        }

        public Nullable<Vector3> TickComponent(RTPuzzle.AIFOVManager aIFOVManager)
        {
            if (!isMovingTowardsDestination)
            {
                return SetRandomDestination(aIFOVManager);
            }
            return null;
        }

        private Nullable<Vector3> SetRandomDestination(RTPuzzle.AIFOVManager aIFOVManager)
        {

            navMeshHits = aIFOVManager.NavMeshRaycastSample(8, patrollingAgent.transform, AIRandomPatrolComponent.MaxDistance);

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

        public void GizmoTick()
        {
            for (var i = 0; i < navMeshHits.Length; i++)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(navMeshHits[i].position, 1f);
                Gizmos.color = Color.white;
            }
        }

        #region External Events
        public void OnDestinationReached()
        {
            isMovingTowardsDestination = false;
        }
        #endregion

        #region Logical Conditions
        public bool IsPatrolling()
        {
            return isMovingTowardsDestination;
        }
        #endregion
    }
}