using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;

namespace RTPuzzle
{
    public class AIPlayerEscapeManager : AbstractPlayerEscapeManager
    {
        #region External dependencies
        private PlayerManagerDataRetriever playerManagerDataRetriever;
        private AIFOVManager aIFOVManager;
        private Func<Collider[]> targetZoneTriggerColliderProvider;
        #endregion

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

        public AIPlayerEscapeManager(NavMeshAgent selfAgent, PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager,
            PlayerManagerDataRetriever playerManagerDataRetriever, AIPlayerEscapeComponent aIPlayerEscapeComponent, AIFOVManager aIFOVManager, Func<Collider[]> targetZoneTriggerColliderProvider)
        {
            this.selfAgent = selfAgent;
            this.puzzleAIBehaviorExternalEventManager = puzzleAIBehaviorExternalEventManager;
            this.playerManagerDataRetriever = playerManagerDataRetriever;
            this.aIPlayerEscapeComponent = aIPlayerEscapeComponent;
            this.aIFOVManager = aIFOVManager;
            this.targetZoneTriggerColliderProvider = targetZoneTriggerColliderProvider;
            this.escapeDestinationManager = new EscapeDestinationManager(this.selfAgent);
        }

        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            bool isInRange = false;
            if (!this.isNearPlayer || (this.isNearPlayer && this.escapeDestinationManager.IsDistanceReached()))
            {
                isInRange = Vector3.Distance(this.selfAgent.transform.position, this.playerManagerDataRetriever.GetPlayerRigidBody().position) <= this.aIPlayerEscapeComponent.PlayerDetectionRadius;
            }
            if (isInRange && !this.isNearPlayer && !this.escapeDestinationManager.IsDistanceReached())
            {
                this.puzzleAIBehaviorExternalEventManager.ReceiveEvent(new PlayerEscapeStartAIBehaviorEvent());
            }
        }

        #region External Event
        public override void OnPlayerEscapeStart()
        {
            this.isNearPlayer = true;
            this.aIFOVManager.IntersectFOV_FromEscapeDirection(this.playerManagerDataRetriever.GetPlayerRigidBody().position, selfAgent.transform.position, this.aIPlayerEscapeComponent.EscapeSemiAngle);
            this.escapeDestinationManager.ResetDistanceComputation(this.aIPlayerEscapeComponent.EscapeDistance);
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
                this.OnStateReset();
            }
            else
            {
                this.CalculateEscapeDirection();
            }
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            return this.escapeDestinationManager.Tick();
        }

        public override void OnStateReset()
        {
            this.isNearPlayer = false;
            this.escapeDestinationManager.ResetDistanceComputation(this.aIPlayerEscapeComponent.EscapeDistance);
        }

        private void CalculateEscapeDirection()
        {
            this.escapeDestinationManager.EscapeDestinationCalculationStrategy(
                escapeDestinationCalculationMethod: (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
                {
                    this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(7, navMeshRaycastStrategy, this.aIFOVManager, this.targetZoneTriggerColliderProvider.Invoke());
                },
                ifAllFailsAction: this.OnStateReset
             );
        }
    }
}
