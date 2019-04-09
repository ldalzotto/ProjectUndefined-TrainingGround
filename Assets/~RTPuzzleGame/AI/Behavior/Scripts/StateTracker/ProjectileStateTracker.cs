namespace RTPuzzle
{
    public class ProjectileStateTracker : BehaviorStateTracker<GenericPuzzleAIBehavior, GenericPuzzleAIComponents>
    {

        private bool hasFirstProjectileHitted;

        public bool HasFirstProjectileHitted { get => hasFirstProjectileHitted; }

        public void AfterDestinationReached(GenericPuzzleAIBehavior behavior)
        {
            if (!behavior.IsEscapingFromProjectile() && !behavior.IsEscapingFromExitZone())
            {
                this.hasFirstProjectileHitted = false;
            }
        }

        public void OnEventProcessed(GenericPuzzleAIBehavior behavior)
        {
            if (behavior.IsEscapingFromProjectile())
            {
                this.hasFirstProjectileHitted = true;
            }
        }
    }

}
