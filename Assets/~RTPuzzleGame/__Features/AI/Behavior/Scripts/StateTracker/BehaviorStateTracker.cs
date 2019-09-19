namespace RTPuzzle
{
    public interface BehaviorStateTracker
    {
        void AfterDestinationReached(IPuzzleAIBehavior behavior);
        void OnEventProcessed(IPuzzleAIBehavior behavior, PuzzleAIBehaviorExternalEvent externalEvent);
    }
}
