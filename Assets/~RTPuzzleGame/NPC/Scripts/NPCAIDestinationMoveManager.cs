using CoreGame;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class NPCAIDestinationMoveManager
    {
        private TransformMoveManagerComponentV2 AIDestimationMoveManagerComponent;
        private NavMeshAgent objectAgent;
        private Action OnDestinationReachedEvent;

        public NPCAIDestinationMoveManager(TransformMoveManagerComponentV2 aIDestimationMoveManagerComponent, NavMeshAgent objectAgent, Action OnDestinationReachedEvent)
        {
            AIDestimationMoveManagerComponent = aIDestimationMoveManagerComponent;
            this.objectAgent = objectAgent;
            this.OnDestinationReachedEvent = OnDestinationReachedEvent;
        }

        private float CurrentTimeAttenuated;
        private Vector3? currentDestination;

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.CurrentTimeAttenuated = d * timeAttenuationFactor;
            this.UpdateAgentTransform(d, timeAttenuationFactor);

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
                }
                this.lastSuccessfulWorldDestination = worldDestination;
                this.lastFrameSuccessfulNextDestinationFrameNb = Time.frameCount;
                this.lastFrameSuccessfulNextDestination = objectAgent.nextPosition;
            }
        }

        public void StopAgent()
        {
            if (this.objectAgent.hasPath)
            {
                this.objectAgent.ResetPath();
                this.objectAgent.isStopped = true;
                this.OnDestinationReachedEvent.Invoke();
            }
        }

        private void ManuallyUpdateAgent()
        {
            Debug.Log(MyLog.Format("ManuallyUpdateAgent"));
            NavMeshHit pathHit;
            objectAgent.SamplePathPosition(NavMesh.AllAreas, objectAgent.speed * this.CurrentTimeAttenuated, out pathHit);
            if (this.CurrentTimeAttenuated > 0)
            {
                objectAgent.velocity = (pathHit.position - objectAgent.transform.position) / this.CurrentTimeAttenuated;
            }
            objectAgent.nextPosition = pathHit.position;
        }

        private void UpdateAgentTransform(float d, float timeAttenuationFactor)
        {
            objectAgent.speed = AIDestimationMoveManagerComponent.SpeedMultiplicationFactor;

            bool updatePosition = true;
            // We use a minimal velocity amplitude to avoid precision loss occured by the navmesh agent velocity calculation.
            if (this.objectAgent.hasPath && !this.objectAgent.isStopped)
            {
                //if target is too close, we look to destination
                float distanceToDestination = Vector3.Distance(objectAgent.nextPosition, this.objectAgent.destination);

                Quaternion targetRotation;

                if (objectAgent.nextPosition != this.objectAgent.destination && distanceToDestination <= 5f)
                {
                    targetRotation = Quaternion.LookRotation((this.objectAgent.destination - objectAgent.nextPosition), Vector3.up);
                }
                else
                {
                    targetRotation = Quaternion.LookRotation((this.objectAgent.path.corners[1] - this.objectAgent.path.corners[0]).normalized, Vector3.up);
                }

                this.objectAgent.transform.rotation = Quaternion.Slerp(this.objectAgent.transform.rotation, targetRotation, this.AIDestimationMoveManagerComponent.RotationSpeed * d * timeAttenuationFactor);

                updatePosition = (!AIDestimationMoveManagerComponent.IsPositionUpdateConstrained) ||                    
                    AIDestimationMoveManagerComponent.IsPositionUpdateConstrained && Quaternion.Angle(this.objectAgent.transform.rotation, targetRotation) <= this.AIDestimationMoveManagerComponent.MinAngleThatAllowThePositionUpdate;
            }

            if (updatePosition)
            {
                objectAgent.transform.position = objectAgent.nextPosition;
            }
            else
            {
                objectAgent.nextPosition = objectAgent.transform.position;
            }
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
}
