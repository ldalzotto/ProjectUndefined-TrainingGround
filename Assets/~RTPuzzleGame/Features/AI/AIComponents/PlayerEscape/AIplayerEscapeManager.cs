using CoreGame;
using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIPlayerEscapeManager : AbstractPlayerEscapeManager
    {
        #region External dependencies
        private PlayerManagerDataRetriever playerManagerDataRetriever;
        private IFovManagerCalcuation fovManagerCalcuation;
        private InteractiveObjectContainer InteractiveObjectContainer;
        private PuzzleEventsManager puzzleEventsManager;
        private TransformMoveManagerComponentV3 aIDestimationMoveManagerComponent;
        #endregion

        private AIObjectDataRetriever AIObjectDataRetriever;

        #region AIBehabvior event manager
        private PuzzleAIBehaviorExternalEventManager puzzleAIBehaviorExternalEventManager;
        #endregion

        private NavMeshAgent selfAgent;

        #region State
        private bool isNearPlayer;
        #endregion

        #region Internal Managers
        protected EscapeDestinationManager escapeDestinationManager;

        public AIPlayerEscapeManager(AIPlayerEscapeComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }
        #endregion

        public override void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.selfAgent = AIBheaviorBuildInputData.selfAgent;
            this.puzzleAIBehaviorExternalEventManager = AIBheaviorBuildInputData.GenericPuzzleAIBehaviorExternalEventManager;
            this.playerManagerDataRetriever = AIBheaviorBuildInputData.PlayerManagerDataRetriever;
            this.fovManagerCalcuation = AIBheaviorBuildInputData.FovManagerCalcuation;
            this.InteractiveObjectContainer = AIBheaviorBuildInputData.InteractiveObjectContainer;
            this.escapeDestinationManager = new EscapeDestinationManager(this.selfAgent);
            this.AIObjectDataRetriever = AIBheaviorBuildInputData.AIObjectDataRetriever();
            this.puzzleEventsManager = AIBheaviorBuildInputData.PuzzleEventsManager;
            this.aIDestimationMoveManagerComponent = AIBheaviorBuildInputData.TransformMoveManagerComponent;
            this.OnStateReset();
        }

        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
            bool isInRange = Vector3.Distance(this.selfAgent.transform.position, this.playerManagerDataRetriever.GetPlayerRigidBody().position) <= this.AssociatedAIComponent.PlayerDetectionRadius;
            //The event is triggered only when AI is not already escaping
            if (isInRange && !this.isNearPlayer && this.escapeDestinationManager.IsDistanceReached())
            {
                this.puzzleAIBehaviorExternalEventManager.ReceiveEvent(new PlayerEscapeStartAIBehaviorEvent(this.playerManagerDataRetriever.GetPlayerRigidBody().position, this.AssociatedAIComponent));
            }
        }

        #region External Event
        public override void OnPlayerEscapeStart()
        {
            this.isNearPlayer = true;
            this.escapeDestinationManager.ResetDistanceComputation(this.AssociatedAIComponent.EscapeDistance);
            this.fovManagerCalcuation.IntersectFOV_FromEscapeDirection(this.playerManagerDataRetriever.GetPlayerRigidBody().position, selfAgent.transform.position, this.AssociatedAIComponent.EscapeSemiAngle);
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
                Debug.Log(MyLog.Format("AI Player escape destination reached - calculate escape direction. Remaining disance : " + this.escapeDestinationManager.GetRemainingDistance()));
                this.CalculateEscapeDirection();
            }
        }

        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            NPCAIDestinationContext.TargetPosition = this.escapeDestinationManager.Tick();
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
                    this.escapeDestinationManager.EscapeToFarestWithCollidersAvoid(7, navMeshRaycastStrategy, this.fovManagerCalcuation, TargetZoneHelper.GetTargetZonesTriggerColliders(this.InteractiveObjectContainer));
                },
                ifAllFailsAction: EscapeDestinationManager.OnDestinationCalculationFailed_ForceAIFear(this.puzzleEventsManager, this.AIObjectDataRetriever, EscapeDestinationManager.ForcedFearRemainingDistanceToFearTime(this.escapeDestinationManager, this.aIDestimationMoveManagerComponent))
             );
        }
    }
}
