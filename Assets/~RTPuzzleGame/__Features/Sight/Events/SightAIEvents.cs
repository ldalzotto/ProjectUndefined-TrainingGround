using UnityEngine;

namespace RTPuzzle
{
    public class SightInRangeEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private CollisionType collisionType;

        public SightInRangeEnterAIBehaviorEvent(CollisionType CollisionType)
        {
            this.collisionType = CollisionType;
        }

        public CollisionType CollisionType { get => collisionType; }
    }

    public class SightInRangeExitAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private CollisionType collisionType;

        public SightInRangeExitAIBehaviorEvent(CollisionType CollisionType)
        {
            this.collisionType = CollisionType;
        }

        public CollisionType CollisionType { get => collisionType; }
    }


    public static class SightAIEvents
    {
        public static void SightInRange_Enter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
       PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIMoveTowardPlayerManager>())
            {
                Debug.Log(MyLog.Format("AI - Sight in range enter."));
                if (genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>().OnSightInRangeEnter(PuzzleAIBehaviorExternalEvent.Cast<SightInRangeEnterAIBehaviorEvent>()))
                {
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>());
                }
            }
        }

        public static void SightInRange_Exit(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIMoveTowardPlayerManager>())
            {
                Debug.Log(MyLog.Format("AI - Sight in range exit."));
                genericAiBehavior.GetAIManager<AbstractAIMoveTowardPlayerManager>().OnSightInRangeExit(PuzzleAIBehaviorExternalEvent.Cast<SightInRangeExitAIBehaviorEvent>());
            }
        }
    }


}
