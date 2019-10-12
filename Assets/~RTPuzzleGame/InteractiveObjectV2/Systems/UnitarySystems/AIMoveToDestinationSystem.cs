using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static InteractiveObjectTest.AIMovementDefinitions;

namespace InteractiveObjectTest
{
    #region Callback Events
    public delegate void OnAIInteractiveObjectDestinationReachedDelegate();
    #endregion

    public class AIMoveToDestinationSystem : AInteractiveObjectSystem
    {
        private AISpeedEventDispatcher AISpeedEventDispatcher;
        private AIDestinationMoveManager AIDestinationMoveManager;

        public AIMoveToDestinationSystem(CoreInteractiveObject CoreInteractiveObject, AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData,
            OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached)
        {
            this.AIDestinationMoveManager = new AIDestinationMoveManager(CoreInteractiveObject.InteractiveGameObject.Agent, AIInteractiveObjectInitializerData, OnAIInteractiveObjectDestinationReached);
            this.AISpeedEventDispatcher = new AISpeedEventDispatcher(CoreInteractiveObject, AIInteractiveObjectInitializerData);
        }

        public override void TickAlways(float d)
        {
            this.AIDestinationMoveManager.TickDestinationReached();
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            this.AIDestinationMoveManager.EnableAgent();
            this.AIDestinationMoveManager.Tick(d, timeAttenuationFactor);
        }

        public override void TickWhenTimeIsStopped()
        {
            this.AIDestinationMoveManager.DisableAgent();
        }

        public override void AfterTicks()
        {
            this.AISpeedEventDispatcher.AfterTicks();
        }

        public void SetDestination(AIDestination AIDestination)
        {
            this.AIDestinationMoveManager.SetDestination(AIDestination);
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition) { this.AIDestinationMoveManager.SetSpeedAttenuationFactor(AIMovementSpeedDefinition); }

        public void ClearPath() { this.AIDestinationMoveManager.ClearPath(); }
    }

    class AIDestinationMoveManager
    {
        private OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached;
        private NavMeshAgent objectAgent;

        public AIDestinationMoveManager(NavMeshAgent objectAgent, AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData, OnAIInteractiveObjectDestinationReachedDelegate OnAIInteractiveObjectDestinationReached)
        {
            this.objectAgent = objectAgent;
            this.lastSuccessfulWorldDestination = new Vector3(9999999, 99999999, 9999999);
            this.AIInteractiveObjectInitializerData = AIInteractiveObjectInitializerData;
            this.OnAIInteractiveObjectDestinationReached = OnAIInteractiveObjectDestinationReached;
            this.currentSpeedAttenuationFactor = AIMovementSpeedDefinition.RUN;
        }

        #region Configuration Data
        private AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData;
        #endregion

        #region State
        private float CurrentTimeAttenuated;
        private AIDestination? currentDestination;
        //Used to change the agent speed
        private AIMovementSpeedDefinition currentSpeedAttenuationFactor;
        #endregion

        #region destination reached manual update
        private int FrameWereOccuredTheLastDestinationReached = -1;
        #endregion

        public void TickDestinationReached()
        {
            //is destination reached
            if (this.currentDestination.HasValue)
            {
                if ((!objectAgent.pathPending && objectAgent.remainingDistance <= objectAgent.stoppingDistance && (!objectAgent.hasPath || objectAgent.velocity.sqrMagnitude == 0f)))
                {
                    this.currentDestination = null;
                    this.FrameWereOccuredTheLastDestinationReached = Time.frameCount;
                    Debug.Log(MyLog.Format("Destination reached !"));
                    this.OnAIInteractiveObjectDestinationReached.Invoke();
                }
            }
        }

        public void Tick(float d, float timeAttenuationFactor)
        {
            this.CurrentTimeAttenuated = d * timeAttenuationFactor;
            this.UpdateAgentTransform(d, timeAttenuationFactor);
        }

        private Vector3 lastSuccessfulWorldDestination;

        #region External Events
        public void SetDestination(AIDestination AIDestination)
        {
            //When a different path is calculated, we manually reset the path and calculate the next destination
            //The input world destination may not be exactly on NavMesh.
            //So we do comparison between world destination
            if (this.lastSuccessfulWorldDestination != AIDestination.WorldPosition)
            {
                this.currentDestination = AIDestination;
                objectAgent.ResetPath();
                NavMeshPath path = CreateValidNavMeshPathWithFallback(objectAgent, AIDestination.WorldPosition, 50);

                objectAgent.SetPath(path);

                //If direction change is occuring when current destination has been reached
                //We manually calculate next position to avoid a frame where AI is standing still
                if (this.FrameWereOccuredTheLastDestinationReached == Time.frameCount)
                {
                    this.ManuallyUpdateAgent();
                }
                this.lastSuccessfulWorldDestination = AIDestination.WorldPosition;
            }
        }

        public void StopAgent()
        {
            if (this.objectAgent.hasPath)
            {
                this.objectAgent.ResetPath();
                this.objectAgent.isStopped = true;
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
            objectAgent.speed = 0f;
        }

        public void ClearPath()
        {
            this.currentDestination = null;
            this.lastSuccessfulWorldDestination = new Vector3(9999999, -9999999, 999999);
            objectAgent.ResetPath();
        }

        public void SetSpeedAttenuationFactor(AIMovementSpeedDefinition AIMovementSpeedDefinition)
        {
            this.currentSpeedAttenuationFactor = AIMovementSpeedDefinition;
        }

        #endregion

        private void UpdateAgentTransform(float d, float timeAttenuationFactor)
        {
            objectAgent.speed = this.AIInteractiveObjectInitializerData.SpeedMultiplicationFactor * AIMovementSpeedAttenuationFactorLookup[this.currentSpeedAttenuationFactor];

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

                this.objectAgent.transform.rotation = Quaternion.Slerp(this.objectAgent.transform.rotation, targetRotation, this.AIInteractiveObjectInitializerData.RotationSpeed * d * timeAttenuationFactor);

                updatePosition =
                    Quaternion.Angle(this.objectAgent.transform.rotation, targetRotation) <= this.AIInteractiveObjectInitializerData.MinAngleThatAllowThePositionUpdate;
            }
            else if (this.currentDestination.HasValue && this.currentDestination.Value.Rotation.HasValue)
            {
                var targetRotation = this.currentDestination.Value.Rotation.Value;
                this.objectAgent.transform.rotation = Quaternion.Slerp(this.objectAgent.transform.rotation, targetRotation, this.AIInteractiveObjectInitializerData.RotationSpeed * d * timeAttenuationFactor);
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

        private void ManuallyUpdateAgent()
        {
            //   Debug.Log(MyLog.Format("ManuallyUpdateAgent"));
            Vector3 velocitySetted = default;
            NavMeshHit pathHit;
            objectAgent.SamplePathPosition(NavMesh.AllAreas, objectAgent.speed * this.CurrentTimeAttenuated, out pathHit);
            if (this.CurrentTimeAttenuated > 0)
            {
                objectAgent.velocity = (pathHit.position - objectAgent.transform.position) / this.CurrentTimeAttenuated;
            }
            objectAgent.nextPosition = pathHit.position;
        }

        private static NavMeshPath CreateValidNavMeshPathWithFallback(NavMeshAgent agent, Vector3 WorldDestination, float fallbackNearestPointDistance)
        {
            var path = new NavMeshPath();
            agent.CalculatePath(WorldDestination, path);
            if (path.status == NavMeshPathStatus.PathInvalid)
            {
                if (Physics.Raycast(WorldDestination, WorldDestination + (Vector3.down * fallbackNearestPointDistance), out RaycastHit hit, fallbackNearestPointDistance, 1 << LayerMask.NameToLayer(LayerConstants.PUZZLE_GROUND_LAYER)))
                {
                    agent.CalculatePath(hit.point, path);
                }
            }
            return path;
        }

    }

    class AISpeedEventDispatcher
    {
        private CoreInteractiveObject AssociatedInteractiveObject;
        private AbstractAIInteractiveObjectInitializerData AIInteractiveObjectInitializerData;

        public AISpeedEventDispatcher(CoreInteractiveObject associatedInteractiveObject, AbstractAIInteractiveObjectInitializerData aIInteractiveObjectInitializerData)
        {
            AssociatedInteractiveObject = associatedInteractiveObject;
            AIInteractiveObjectInitializerData = aIInteractiveObjectInitializerData;
        }

        public void AfterTicks()
        {
            var currentSpeed = this.AssociatedInteractiveObject.InteractiveGameObject.Agent.speed / this.AIInteractiveObjectInitializerData.SpeedMultiplicationFactor;
            this.AssociatedInteractiveObject.OnAnimationObjectSetUnscaledSpeedMagnitude(new AnimationObjectSetUnscaledSpeedMagnitudeEvent { UnscaledSpeedMagnitude = currentSpeed });
        }
    }

    public struct AIDestination
    {
        public Vector3 WorldPosition;
        public Quaternion? Rotation;
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
}
