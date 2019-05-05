using static RTPuzzle.AIBehaviorManagerContainer;

namespace RTPuzzle
{
    public class ProjectileStateTracker : BehaviorStateTracker
    {

        private bool hasFirstProjectileHitted;

        public bool HasFirstProjectileHitted { get => hasFirstProjectileHitted; }

        public void AfterDestinationReached(IPuzzleAIBehavior<AbstractAIComponents> behavior)
        {
            var genericPuzzleBehavior = (GenericPuzzleAIBehavior)behavior;
            this.hasFirstProjectileHitted = genericPuzzleBehavior.IsEscapingFromProjectileWithTargetZones();
        }

        public void OnEventProcessed(IPuzzleAIBehavior<AbstractAIComponents> behavior)
        {
            var genericPuzzleBehavior = (GenericPuzzleAIBehavior)behavior;
            if (genericPuzzleBehavior.IsEscapingFromProjectileWithTargetZones())
            {
                this.hasFirstProjectileHitted = true;
            }
        }
    }

}
