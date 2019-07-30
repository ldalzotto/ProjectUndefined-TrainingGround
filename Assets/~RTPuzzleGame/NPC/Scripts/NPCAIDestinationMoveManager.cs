using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class NPCAIDestinationMoveManager
    {
        private AIDestimationMoveManagerComponent AIDestimationMoveManagerComponent;
        private NavMeshAgent objectAgent;
        private Action OnDestinationReachedEvent;

        public NPCAIDestinationMoveManager(AIDestimationMoveManagerComponent aIDestimationMoveManagerComponent, NavMeshAgent objectAgent, Action OnDestinationReachedEvent)
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

        private void UpdateAgentTransform()
        {
            objectAgent.speed = AIDestimationMoveManagerComponent.SpeedMultiplicationFactor;
            //  Debug.Log(MyLog.Format("AGENT MOVE : old " + objectTransform.position.ToString("F4") + " new " + objectAgent.nextPosition.ToString("F4")));
            objectAgent.transform.position = objectAgent.nextPosition;


            // We use a minimal velocity amplitude to avoid precision loss occured by the navmesh agent velocity calculation.
            if (this.objectAgent.hasPath && !this.objectAgent.isStopped)
            {
                //if target is too close, we look to destination
                float distanceToDestination = Vector3.Distance(this.objectAgent.transform.position, this.objectAgent.destination);

                if (this.objectAgent.transform.position != this.objectAgent.destination && distanceToDestination <= 5f)
                {
                    this.objectAgent.transform.rotation = Quaternion.LookRotation((this.objectAgent.destination - this.objectAgent.transform.position), Vector3.up);
                }
                else
                {
                    this.objectAgent.transform.rotation = Quaternion.LookRotation((this.objectAgent.path.corners[1] - this.objectAgent.path.corners[0]).normalized, Vector3.up);
                }
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

    [System.Serializable]
    public class AIDestimationMoveManagerComponent
    {
        public float SpeedMultiplicationFactor;
    }
}
