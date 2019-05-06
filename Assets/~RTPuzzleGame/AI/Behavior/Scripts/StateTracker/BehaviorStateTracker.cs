namespace RTPuzzle
{
    public interface BehaviorStateTracker
    {
        void AfterDestinationReached(IPuzzleAIBehavior<AbstractAIComponents> behavior);
        void OnEventProcessed(IPuzzleAIBehavior<AbstractAIComponents> behavior, PuzzleAIBehaviorExternalEvent externalEvent);
    }
}
