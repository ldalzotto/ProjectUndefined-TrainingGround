namespace RTPuzzle
{
    public interface BehaviorStateTracker<T, C> where C : AbstractAIComponents where T : IPuzzleAIBehavior<C> 
    {
        void AfterDestinationReached(T behavior);
        void OnEventProcessed(T behavior);
    }
}
