using CoreGame;
using RTPuzzle;

namespace InteractiveObjects
{
    public class AIPatrolSystem : AInteractiveObjectSystem
    {
        private SequencedActionPlayer SequencedActionPlayer;

        public AIPatrolSystem(CoreInteractiveObject AssociatedCoreInteractiveObject, AIPatrolSystemDefinition AIPatrolSystemDefinition)
        {
            this.SequencedActionPlayer = new SequencedActionPlayer(AIPatrolSystemDefinition.AIPatrolGraph.AIPatrolGraphActions(AssociatedCoreInteractiveObject), null, OnCutsceneEnded: null);
            this.SequencedActionPlayer.Play();
        }

        public override void Tick(float d, float timeAttenuationFactor)
        {
            this.SequencedActionPlayer.Tick(d * timeAttenuationFactor);
        }

        public void OnAIDestinationReached()
        {
            foreach (var currentAction in this.SequencedActionPlayer.GetCurrentActions(includeWorkflowNested: true))
            {
                var destinationReachedListeningNode = currentAction as IActionAbortedOnDestinationReached;
                if (destinationReachedListeningNode != null)
                {
                    destinationReachedListeningNode.OnDestinationReached();
                }
            }
        }
    }
}
