using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using GameConfigurationID;
using CoreGame;

namespace RTPuzzle
{
    public class AIEscapeWithoutTriggerManager : AbstractAIEscapeWithoutTriggerManager
    {
        #region External Dependencies
        private NavMeshAgent escapingAgent;
        private IFovManagerCalcuation FovManagerCalcuation;
        private PuzzleEventsManager puzzleEventsManager;
        private AIObjectID aiID;
        private TransformMoveManagerComponentV3 playerAIDestimationMoveManagerComponent;
        #endregion

        private EscapeDestinationManager escapeDestinationManager;

        #region State
        private bool isEscaping;

        public AIEscapeWithoutTriggerManager(AIEscapeWithoutTriggerComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public override void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.escapingAgent = AIBheaviorBuildInputData.selfAgent;
            FovManagerCalcuation = AIBheaviorBuildInputData.FovManagerCalcuation;
            this.puzzleEventsManager = AIBheaviorBuildInputData.PuzzleEventsManager;
            this.aiID = AIBheaviorBuildInputData.aiID;
            this.playerAIDestimationMoveManagerComponent = AIBheaviorBuildInputData.TransformMoveManagerComponent;
            this.escapeDestinationManager = new EscapeDestinationManager(this.escapingAgent);
        }
        
        #endregion

        public override void BeforeManagersUpdate(float d, float timeAttenuationFactor)
        {
        }

        public override bool IsManagerEnabled()
        {
            return this.isEscaping;
        }

        public override void OnDestinationReached()
        {
            if (this.escapeDestinationManager.OnAgentDestinationReached())
            {
                this.OnStateReset();
            }
            else
            {
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.EscapeCaluclation,
                        EscapeDestinationManager.OnDestinationCalculationFailed_ForceAIFear(this.puzzleEventsManager, this.aiID, EscapeDestinationManager.ForcedFearRemainingDistanceToFearTime(this.escapeDestinationManager, this.playerAIDestimationMoveManagerComponent)));
            }
        }

        public override void OnEscapeStart(EscapeWithoutTriggerStartAIBehaviorEvent escapeWithoutTriggerStartAIBehaviorEvent)
        {
            if (escapeWithoutTriggerStartAIBehaviorEvent != null && escapeWithoutTriggerStartAIBehaviorEvent.ThreatStartPoint != null)
            {
                this.escapeDestinationManager.ResetDistanceComputation(escapeWithoutTriggerStartAIBehaviorEvent.EscapeDistance);
                this.FovManagerCalcuation.IntersectFOV_FromEscapeDirection(escapeWithoutTriggerStartAIBehaviorEvent.ThreatStartPoint, this.escapingAgent.transform.position, escapeWithoutTriggerStartAIBehaviorEvent.EscapeSemiAngle);
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.EscapeCaluclation,
                        EscapeDestinationManager.OnDestinationCalculationFailed_ForceAIFear(this.puzzleEventsManager, this.aiID, EscapeDestinationManager.ForcedFearRemainingDistanceToFearTime(this.escapeDestinationManager, this.playerAIDestimationMoveManagerComponent)));
            }
            this.SetIsEscaping(true);
        }

        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            NPCAIDestinationContext.TargetPosition = this.escapeDestinationManager.Tick();
        }

        private void SetIsEscaping(bool value)
        {
            if (value && !this.isEscaping)
            {
                this.puzzleEventsManager.PZ_EVT_AI_EscapeWithoutTarget_Start(this.aiID);
            }
            if (!value && this.isEscaping)
            {
                this.puzzleEventsManager.PZ_EVT_AI_EscapeWithoutTarget_End(this.aiID);
            }
            this.isEscaping = value;
        }

        private void EscapeCaluclation(NavMeshRaycastStrategy navMeshRaycastStrategy)
        {
            Debug.Log("EscapeToFarest");
            this.escapeDestinationManager.EscapeToFarest(5, navMeshRaycastStrategy, this.FovManagerCalcuation);
        }

        public override void OnStateReset()
        {
            this.escapeDestinationManager.OnStateReset();
            this.SetIsEscaping(false);
        }

    }
}

