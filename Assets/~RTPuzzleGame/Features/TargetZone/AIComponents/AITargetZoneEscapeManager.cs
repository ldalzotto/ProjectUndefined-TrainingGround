using GameConfigurationID;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{

    public class AITargetZoneEscapeManager : AbstractAITargetZoneManager
    {
        private AIObjectID aiID;
        private Collider aiCollider;

        #region External Dependencies
        private NavMeshAgent agent;
        private PuzzleGameConfigurationManager puzzleGameConfigurationManager;
        private InteractiveObjectContainer InteractiveObjectContainer;
        #endregion

        #region Internal Dependencies
        private IFovManagerCalcuation FovManagerCalcuation;
        #endregion

        #region Internal Managers
        private EscapeDestinationManager EscapeDestinationManager;

        #endregion

        public AITargetZoneEscapeManager(AITargetZoneComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public override void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            this.agent = AIBheaviorBuildInputData.selfAgent;
            this.aiCollider = AIBheaviorBuildInputData.aiCollider;

            this.InteractiveObjectContainer = PuzzleGameSingletonInstances.InteractiveObjectContainer;
            this.puzzleGameConfigurationManager = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager;
            this.FovManagerCalcuation = AIBheaviorBuildInputData.FovManagerCalcuation;
            this.EscapeDestinationManager = new EscapeDestinationManager(this.agent);
            this.aiID = AIBheaviorBuildInputData.aiID;
        }

        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            NPCAIDestinationContext.TargetPosition = this.EscapeDestinationManager.Tick();
        }

        public override void OnDestinationReached()
        {
            if (this.EscapeDestinationManager.OnAgentDestinationReached())
            {
                if (this.isEscapingFromTargetZone)
                {
                    var overlappingTargetZone = this.IsAIOverlappingWithATargetZone();
                    if (overlappingTargetZone != null)
                    {
                        this.TriggerTargetZoneEscape(overlappingTargetZone);
                    }
                    else
                    {
                        this.OnStateReset();
                        isEscapingFromTargetZone = false;
                    }
                }
                else
                {
                    this.OnStateReset();
                    isEscapingFromTargetZone = false;
                }
            }
            else
            {
                this.CalculateEscapeDirection();
            }
        }

        public override void OnStateReset()
        {
            this.EscapeDestinationManager.OnStateReset();
            isEscapingFromTargetZone = false;
        }

        public override void TriggerTargetZoneEscape(ITargetZoneModuleDataRetriever ITargetZoneModuleDataRetriever)
        {
            Debug.Log(Time.frameCount + "Target zone trigger : " + ITargetZoneModuleDataRetriever.GetTargetZoneID());

            var targetZoneConfigurationData = this.puzzleGameConfigurationManager.TargetZonesConfiguration()[ITargetZoneModuleDataRetriever.GetTargetZoneID()];

            this.isEscapingFromTargetZone = true;
            this.EscapeDestinationManager.ResetDistanceComputation(this.AssociatedAIComponent.TargetZoneEscapeDistance);

            FovManagerCalcuation.IntersectFOV_FromEscapeDirection(ITargetZoneModuleDataRetriever.GetTransform().position, agent.transform.position, targetZoneConfigurationData.EscapeFOVSemiAngle);

            this.CalculateEscapeDirection();
        }

        private void CalculateEscapeDirection()
        {
            this.EscapeDestinationManager.EscapeDestinationCalculationStrategy(
                escapeDestinationCalculationMethod: (NavMeshRaycastStrategy navMeshRaycastStrategy) =>
                {
                    this.EscapeDestinationManager.EscapeToFarest(7, navMeshRaycastStrategy, this.FovManagerCalcuation);
                },
                ifAllFailsAction: this.OnStateReset
             );
        }

        private TargetZoneModule IsAIOverlappingWithATargetZone()
        {
            var overlappingTargetZones = TargetZoneHelper.GetAllTargetZonesWhereDistanceCheckOverlaps(this.aiCollider.bounds, this.InteractiveObjectContainer);
            if (overlappingTargetZones != null && overlappingTargetZones.Count > 0)
            {
                return overlappingTargetZones[0];
            }
            return null;
        }

    }

}
