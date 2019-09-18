using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public class AIScriptedPatrolComponentManager : AbstractAIPatrolComponentManager, IAIPatrolGraphEventListener
    {
        private AIObjectTypeSpeedSetter AIObjectTypeSpeedSetter;

        private bool isPatrolling;

        private SequencedActionPlayer PatrolGraphPlayer;

        private Transform anchorPosition;

        public AIScriptedPatrolComponentManager(AIPatrolComponent associatedAIComponent) : base(associatedAIComponent)
        {
        }

        public override void Init(AIBheaviorBuildInputData AIBheaviorBuildInputData)
        {
            base.Init(AIBheaviorBuildInputData);
            this.AIObjectTypeSpeedSetter = AIBheaviorBuildInputData.AIObjectTypeSpeedSetter();
            var AIPatrolGraph = PuzzleGameSingletonInstances.PuzzleGameConfigurationManager.AIPatrolGraphConfiguration()[this.AssociatedAIComponent.AIPatrolGraphID].AIPatrolGraph;
            this.PatrolGraphPlayer = new SequencedActionPlayer(AIPatrolGraph.GetRootActions(), new AIPatrolActionInput(this, AIPatrolActionInput.BuildParameters(AIBheaviorBuildInputData.AssociatedInteractiveObject)), null);
            this.PatrolGraphPlayer.Play();
        }
        
        public override void GizmoTick() { }

        public override void OnDestinationReached()
        {
            if (this.IsManagerEnabled())
            {
                foreach (var action in this.PatrolGraphPlayer.GetCurrentActions(includeWorkflowNested: true))
                {
                    if (action.GetType() == typeof(AIMoveToAction))
                    {
                        var AIMoveToAction = (AIMoveToAction)action;
                        AIMoveToAction.OnDestinationReached();
                    }
                }
            }
        }

        public override void OnManagerTick(float d, float timeAttenuationFactor, ref NPCAIDestinationContext NPCAIDestinationContext)
        {
            AIObjectTypeSpeedSetter.SetSpeedAttenuationFactor(this.AssociatedAIComponent.AISpeed);
            this.isPatrolling = true;
            this.PatrolGraphPlayer.Tick(d * timeAttenuationFactor);
            if (this.anchorPosition != null)
            {
                NPCAIDestinationContext.TargetPosition = this.anchorPosition.position;
                NPCAIDestinationContext.TargetRotation = this.anchorPosition.rotation;
            }
        }

        public override void OnStateReset()
        {
            this.isPatrolling = false;
        }

        protected override bool IsPatrolling()
        {
            return isPatrolling;
        }

        public void DestinationSettedFromPatrolGraph(Transform destination)
        {
            this.anchorPosition = destination;
        }
    }

    public interface IAIPatrolGraphEventListener
    {
        void DestinationSettedFromPatrolGraph(Transform destination);
    }
}
