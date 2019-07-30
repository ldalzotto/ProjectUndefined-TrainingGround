using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;
using GameConfigurationID;
using CoreGame;

namespace RTPuzzle
{
    public class AIPlayerEscapeManager : AbstractPlayerEscapeManager
    {
        #region External dependencies
        private PlayerManagerDataRetriever playerManagerDataRetriever;
        private AIFOVManager aIFOVManager;
        private Func<Collider[]> targetZoneTriggerColliderProvider;
        private PuzzleEventsManager puzzleEventsManager;
        private TransformMoveManagerComponentV2 aIDestimationMoveManagerComponent;
        #endregion

        private AiID aiID;

        #region AIBehabvior event manager
        private PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager;
        #endregion

        private NavMeshAgent selfAgent;
        AIPlayerEscapeComponent aIPlayerEscapeComponent;

        #region State
        private bool isNearPlayer;
        #endregion

        #region Internal Managers
        protected EscapeDestinationManager escapeDestinationManager;
        #endregion

        public void Init(NavMeshAgent selfAgent, PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager,
            PlayerManagerDataRetriever playerManagerDataRetriever, AIPlayerEscapeComponent aIPlayerEscapeComponent, AIFOVManager aIFOVManager, Func<Collider[]> targetZoneTriggerColliderProvider, AiID aiID,
            PuzzleEventsManager puzzleEventsManager, TransformMoveManagerComponentV2 aIDestimationMoveManagerComponent)
        {
            this.selfAgent = selfAgent;
            this.puzzleAIBehaviorExternalEventManager = puzzleAIBehaviorExternalEventManager;
            this.playerManagerDataRetriever = playerManagerDataRetriever;
            this.aIPlayerEscapeComponent = aIPlayerEscapeComponent;
            this.aIFOVManager = aIFOVManager;
            this.targetZoneTriggerColliderProvider = targetZoneTriggerColliderProvider;
            this.escapeDestinationManager = new EscapeDestinationManager(this.selfAgent);
            this.aiID = aiID;
            this.puzzleEventsManager = puzzleEventsManager;
            this.aIDestimationMoveManagerComponent = aIDestimationMoveManagerComponent;
            this.OnStateReset();
        }

        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            bool isInRange = Vector3.Distance(this.selfAgent.transform.position, this.playerManagerDataRetriever.GetPlayerRigidBody().position) <= this.aIPlayerEscapeComponent.PlayerDetectionRadius;
            //The event is triggered only when AI is not already escaping
            if (isInRange && !this.isNearPlayer && this.escapeDestinationManager.IsDistanceReached())
            {
                this.puzzleAIBehaviorExternalEventManager.ReceiveEvent(new PlayerEscapeStartAIBehaviorEvent(this.playerManagerDataRetriever.GetPlayerRigidBody().position, this.aIPlayerEscapeComponent));
            }
        }

        #region External Event
        public override void OnPlayerEscapeStart()
        {
            this.isNearPlayer = true;
            this.escapeDestinationManager.ResetDistanceComputation(this.aIPlayerEscapeComponent.EscapeDistance);
            this.aIFOVManager.IntersectFOV_FromEscapeDirection(this.playerManagerDataRetriever.GetPlayerRigidBody().position, selfAgent.transform.position, this.aIPlayerEscapeComponent.EscapeSemiAngle);
            this.CalculateEscapeDirection();
        }
        #endregion

        public override bool IsManagerEnabled()
        {
            return this.isNearPlayer;
        }

        public override void OnDestinationReached()
        {
            if (this.escapeDestinationManager.OnAgentDestinationReached())
            {
                Debug.Log(MyLog.Format("AI Player escape destination reached - state reset"));
                this.OnStateReset();
            }
            else
            {
                Debug.Log(MyLog.Format("AI Player escape destination reached - calculate escape direction. Remaining disance : " + this.escapeDestinationManager.GetRemainingDistance())) ;
                this.CalculateEscapeDirection();
            }
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            return this.escapeDestinationManager.Tick();
        }

        public override void OnStateReset()
        {
            this.escapeDestinationManager.OnStateReset();
            this.isNearPlayer = false;
        }

        private void CalculateEscapeDirection()
        {
            this.escapeDestinationManager.EscapeDestinationCalculationStrategy(
                escapeDestinationCalculationMethod: (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
                {
                    this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(7, navMeshRaycastStrategy, this.aIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
                },
                ifAllFailsAction: EscapeDestinationManager.OnDestinationCalculationFailed_ForceAIFear(this.puzzleEventsManager, this.aiID, EscapeDestinationManager.ForcedFearRemainingDistanceToFearTime(this.escapeDestinationManager, this.aIDestimationMoveManagerComponent))
             );
        }
    }
}
