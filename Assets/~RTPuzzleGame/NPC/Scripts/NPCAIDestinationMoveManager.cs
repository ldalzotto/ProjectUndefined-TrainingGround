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

        private float CurrentTimeAttenuated;
        private Vector3? currentDestination;

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.CurrentTimeAttenuated = d * timeAttenuationFactor;
            this.UpdateAgentTransform();

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
            //When a different path is calculated, we manually reset the path and calculate the next destination
            if (objectAgent.destination != destination)
            {
                this.currentDestination = destination;
                NavMeshPath path = new NavMeshPath();
                objectAgent.ResetPath();
                objectAgent.CalculatePath(destination, path);
                objectAgent.SetPath(path);
                NavMeshHit pathHit;
                objectAgent.SamplePathPosition(NavMesh.AllAreas, objectAgent.speed * this.CurrentTimeAttenuated, out pathHit);
                objectAgent.nextPosition = pathHit.position;
                this.UpdateAgentTransform();
            }
        }

        private void UpdateAgentTransform()
        {
            // We use a minimal velocity amplitude to avoid precision loss occured by the navmesh agent velocity calculation.
            if (Vector3.Distance(Vector3.zero, (objectAgent.velocity / AIDestimationMoveManagerComponent.SpeedMultiplicationFactor)) >= AIDestimationMoveManagerComponent.RotationFollow_VelocityThreshold)
            {
                objectTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(objectAgent.velocity.normalized, Vector3.up));
            }

            objectAgent.speed = AIDestimationMoveManagerComponent.SpeedMultiplicationFactor;
            objectTransform.position = objectAgent.nextPosition;
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
        [Tooltip("This value is the minimum value of speed, starting from the transform rotation will follow the agent direction. If this value were not " +
            "present, the precission loss of agent speed calculation would cause the rotation to go crazy when speed is very low.")]
        [Range(0, 1)]
        public float RotationFollow_VelocityThreshold;
        public float SpeedMultiplicationFactor;
    }
}
