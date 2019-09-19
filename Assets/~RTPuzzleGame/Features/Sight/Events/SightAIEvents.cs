using UnityEngine;

namespace RTPuzzle
{
    public class SightInRangeEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private ColliderWithCollisionType colliderWithCollisionType;

        public SightInRangeEnterAIBehaviorEvent(ColliderWithCollisionType colliderWithCollisionType)
        {
            this.colliderWithCollisionType = colliderWithCollisionType;
        }

        public ColliderWithCollisionType ColliderWithCollisionType { get => colliderWithCollisionType; }
    }

    public class SightInRangeExitAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private ColliderWithCollisionType colliderWithCollisionType;

        public SightInRangeExitAIBehaviorEvent(ColliderWithCollisionType colliderWithCollisionType)
        {
            this.colliderWithCollisionType = colliderWithCollisionType;
        }

        public ColliderWithCollisionType ColliderWithCollisionType { get => colliderWithCollisionType; }
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
