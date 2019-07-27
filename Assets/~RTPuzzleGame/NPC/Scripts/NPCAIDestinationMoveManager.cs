using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class NPCAIDestinationMoveManager
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

        private Vector3 lastSuccessfulWorldDestination;

        private int lastFrameSuccessfulNextDestinationFrameNb;
        private Vector3 lastFrameSuccessfulNextDestination;
        private Vector3? manuallyCalculatedVelocity;

        public Vector3? ManuallyCalculatedVelocity { get => manuallyCalculatedVelocity; }

        #region External Events
        public void SetDestination(Vector3 worldDestination)
        {
            //When a different path is calculated, we manually reset the path and calculate the next destination
            //The input world destination may not be exactly on NavMesh.
            //So we do comparison between world destination
            if (this.lastSuccessfulWorldDestination != worldDestination)
            {
                this.currentDestination = worldDestination;
                NavMeshPath path = new NavMeshPath();
                objectAgent.ResetPath();
                objectAgent.CalculatePath(worldDestination, path);
                objectAgent.SetPath(path);

                //If direction change is occuring when current destination has been reached
                //We manually calculate next position to avoid a frame where AI is standing still
                if (objectAgent.transform.position == objectAgent.nextPosition)
                {
                    this.ManuallyUpdateAgent();
                    //objectAgent.transform.rotation = Quaternion.LookRotation(objectAgent.transform.position - objectAgent.nextPosition, objectAgent.transform.up);
                }
                this.lastSuccessfulWorldDestination = worldDestination;
                this.lastFrameSuccessfulNextDestinationFrameNb = Time.frameCount;
                this.lastFrameSuccessfulNextDestination = objectAgent.nextPosition;
            }
        }

        private void ManuallyUpdateAgent()
        {
            Debug.Log(MyLog.Format("ManuallyUpdateAgent"));
            NavMeshHit pathHit;
            objectAgent.SamplePathPosition(NavMesh.AllAreas, objectAgent.speed * this.CurrentTimeAttenuated, out pathHit);
            if (this.CurrentTimeAttenuated > 0)
            {
                this.manuallyCalculatedVelocity = (pathHit.position - objectAgent.transform.position) / this.CurrentTimeAttenuated;
                objectAgent.velocity = this.manuallyCalculatedVelocity.Value;
            }
            objectAgent.nextPosition = pathHit.position;
        }

        private void UpdateAgentTransform()
        {
            // We use either manually calculated velocity or calculated by unity
            Vector3 velocityUsed = this.manuallyCalculatedVelocity.HasValue ? this.manuallyCalculatedVelocity.Value : this.objectAgent.velocity;
            this.manuallyCalculatedVelocity = null;

            // We use a minimal velocity amplitude to avoid precision loss occured by the navmesh agent velocity calculation.
            if (Vector3.Distance(Vector3.zero, (velocityUsed / AIDestimationMoveManagerComponent.SpeedMultiplicationFactor)) >= AIDestimationMoveManagerComponent.RotationFollow_VelocityThreshold)
            {
                objectTransform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(velocityUsed.normalized, Vector3.up));
            }

            objectAgent.speed = AIDestimationMoveManagerComponent.SpeedMultiplicationFactor;
            //  Debug.Log(MyLog.Format("AGENT MOVE : old " + objectTransform.position.ToString("F4") + " new " + objectAgent.nextPosition.ToString("F4")));
            objectTransform.position = objectAgent.nextPosition;
        }

        public void EnableAgent()
        {
            objectAgent.isStopped = false;
        }
        public void DisableAgent()
        {
            // Debug.Log(MyLog.Format("Agent disabled"));
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
