namespace RTPuzzle
{
    public class ProjectileStateTracker : BehaviorStateTracker
    {

        private bool hasFirstProjectileHitted;

        public bool HasFirstProjectileHitted { get => hasFirstProjectileHitted; }

        public void AfterDestinationReached(IPuzzleAIBehavior<AbstractAIComponents> behavior)
        {
            var genericPuzzleBehavior = (GenericPuzzleAIBehavior)behavior;
            if (!genericPuzzleBehavior.IsEscapingFromProjectile() && !genericPuzzleBehavior.IsEscapingFromExitZone())
            {
                this.hasFirstProjectileHitted = false;
            }
        }

        public void OnEventProcessed(IPuzzleAIBehavior<AbstractAIComponents> behavior)
        {
            var genericPuzzleBehavior = (GenericPuzzleAIBehavior)behavior;
            if (genericPuzzleBehavior.IsEscapingFromProjectile())
            {
                this.hasFirstProjectileHitted = true;
            }
        }
    }

}
