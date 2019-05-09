using UnityEngine;
using System.Collections;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIEscapeWithoutTriggerManager : AbstractAIEscapeWithoutTriggerManager
    {
        #region External Dependencies
        private NavMeshAgent escapingAgent;
        private AIFOVManager AIFOVManager;
        private PuzzleEventsManager puzzleEventsManager;
        private AiID aiID;
        private AIDestimationMoveManagerComponent playerAIDestimationMoveManagerComponent;
        #endregion

        private EscapeDestinationManager escapeDestinationManager;

        #region State
        private bool isEscaping;

        public AIEscapeWithoutTriggerManager(NavMeshAgent escapingAgent, AIFOVManager aIFOVManager, PuzzleEventsManager puzzleEventsManager, AiID aiID, AIDestimationMoveManagerComponent playerAIDestimationMoveManagerComponent)
        {
            this.escapingAgent = escapingAgent;
            AIFOVManager = aIFOVManager;
            this.puzzleEventsManager = puzzleEventsManager;
            this.aiID = aiID;
            this.playerAIDestimationMoveManagerComponent = playerAIDestimationMoveManagerComponent;
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
                this.AIFOVManager.IntersectFOV_FromEscapeDirection(escapeWithoutTriggerStartAIBehaviorEvent.ThreatStartPoint, this.escapingAgent.transform.position, escapeWithoutTriggerStartAIBehaviorEvent.EscapeSemiAngle);
                this.escapeDestinationManager.EscapeDestinationCalculationStrategy(this.EscapeCaluclation,
                        EscapeDestinationManager.OnDestinationCalculationFailed_ForceAIFear(this.puzzleEventsManager, this.aiID, EscapeDestinationManager.ForcedFearRemainingDistanceToFearTime(this.escapeDestinationManager, this.playerAIDestimationMoveManagerComponent)));
            }
            this.SetIsEscaping(true);
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            return this.escapeDestinationManager.Tick();
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
            this.escapeDestinationManager.EscapeToFarest(5, navMeshRaycastStrategy, this.AIFOVManager);
        }

        public override void OnStateReset()
        {
            this.escapeDestinationManager.OnStateReset();
            this.SetIsEscaping(false);
        }

    }
}

