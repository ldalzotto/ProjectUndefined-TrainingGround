using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static AIMovementDefinitions;

namespace RTPuzzle
{
    public class AIDestinationMoveManager
    {
        private TransformMoveManagerComponentV3 AIDestimationMoveManagerComponent;
        private NavMeshAgent objectAgent;
        private Action OnDestinationReachedEvent;

        public AIDestinationMoveManager(TransformMoveManagerComponentV3 aIDestimationMoveManagerComponent, NavMeshAgent objectAgent, Action OnDestinationReachedEvent)
        {
            AIDestimationMoveManagerComponent = aIDestimationMoveManagerComponent;
            this.objectAgent = objectAgent;
            this.OnDestinationReachedEvent = OnDestinationReachedEvent;
            this.currentSpeedAttenuationFactor = AIMovementSpeedDefinition.RUN;
        }

        #region State
        private float CurrentTimeAttenuated;
        private Vector3? currentDestination;

        //Used to change the agent speed
        private AIMovementSpeedDefinition currentSpeedAttenuationFactor;
        #endregion

        public void Tick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            this.CurrentTimeAttenuated = d * timeAttenuationFactor;
            this.UpdateAgentTransform(d, timeAttenuationFactor, ref NPCAIDestinationContext);

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

        #region External Events
        public void SetDestination(ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            if (NPCAIDestinationContext.TargetPosition.HasValue)
            {
                var worldDestination = NPCAIDestinationContext.TargetPosition.Value;
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
                }
            }
            else
            {
                this.StopAgent();
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

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            this.currentSpeedAttenuationFactor = AIMovementSpeedDefinition;
        }

        #endregion
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

        private void UpdateAgentTransform(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            objectAgent.speed = AIDestimationMoveManagerComponent.SpeedMultiplicationFactor * AIMovementSpeedAttenuationFactorLookup[this.currentSpeedAttenuationFactor];

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
                    AIDestimationMoveManagerComponent.IsPositionUpdateConstrained && Quaternion.Angle(this.objectAgent.transform.rotation, targetRotation) <= this.AIDestimationMoveManagerComponent.TransformPositionUpdateConstraints.MinAngleThatAllowThePositionUpdate;
            }
            else if (NPCAIDestinationContext.TargetRotation.HasValue)
            {
                var targetRotation = NPCAIDestinationContext.TargetRotation.Value;
                this.objectAgent.transform.rotation = Quaternion.Slerp(this.objectAgent.transform.rotation, targetRotation, this.AIDestimationMoveManagerComponent.RotationSpeed * d * timeAttenuationFactor);
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
        
    }

    public class NPCAIDestinationContext
    {
        public Vector3? TargetPosition;
        public Quaternion? TargetRotation;

        public void Clear()
        {
            this.TargetPosition = null;
            this.TargetRotation = null;
        }
    }
}

public static class AIMovementDefinitions
{
    public enum AIMovementSpeedDefinition
    {
        RUN = 0,
        WALK = 1,
        ZERO = 2
    }

    public static Dictionary<AIMovementSpeedDefinition, float> AIMovementSpeedAttenuationFactorLookup = new Dictionary<AIMovementSpeedDefinition, float>()
    {
        { AIMovementSpeedDefinition.ZERO, 0f },
        { AIMovementSpeedDefinition.WALK, 0.5f },
        { AIMovementSpeedDefinition.RUN, 1f }
    };
}