using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    class NPCAIDestinationMoveManager
    {
        private AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;
        private NavMeshAgent objectAgent;
        private Transform objectTransform;
        private Action OnDestinationReachedEvent;

        public NPCAIDestinationMoveManager(AIDestimationMoveManagerComponent aIDestimationMoveManagerComponent, NavMeshAgent objectAgent, Transform objectTransform, Action OnDestinationReachedEvent)
        {
            AIDestimationMoveManagerComponent = aIDestimationMoveManagerComponent;
            this.objectAgent = objectAgent;
            this.objectTransform = objectTransform;
            this.OnDestinationReachedEvent = OnDestinationReachedEvent;
        }

        private Vector3? currentDestination;

        public void Tick(float d)
        {
            if (objectAgent.velocity.normalized != Vector3.zero)
            {
                objectTransform.rotation = Quaternion.Slerp(objectTransform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(objectAgent.velocity.normalized, Vector3.up)),
                    d * AIDestimationMoveManagerComponent.AIRotationSpeed);
            }

            objectAgent.speed = AIDestimationMoveManagerComponent.SpeedMultiplicationFactor;
            objectTransform.position = objectAgent.nextPosition;

            //is destination reached
            if (this.currentDestination.HasValue)
            {
                if ((!objectAgent.pathPending && objectAgent.remainingDistance <= objectAgent.stoppingDistance && (!objectAgent.hasPath || objectAgent.velocity.sqrMagnitude == 0f)))
                {
                    this.currentDestination = null;
                    Debug.Log(Time.frameCount + " : Destination reached !");
                    this.OnDestinationReachedEvent.Invoke();
                }
            }
        }

        #region External Events
        public void SetDestination(Vector3 destination)
        {
            //     Debug.Log("Set to AI : "+ objectAgent.name + " the destination : " + destination);
            this.currentDestination = destination;
            objectAgent.SetDestination(destination);
        }
        public void EnableAgent()
        {
            objectAgent.isStopped = false;
        }
        public void DisableAgent()
        {
            objectAgent.isStopped = true;
            objectAgent.nextPosition = objectAgent.transform.position;
        }

        #endregion
    }

    [System.Serializable]
    public class AIDestimationMoveManagerComponent
    {
        public float AIRotationSpeed;
        public float SpeedMultiplicationFactor;
    }
}
