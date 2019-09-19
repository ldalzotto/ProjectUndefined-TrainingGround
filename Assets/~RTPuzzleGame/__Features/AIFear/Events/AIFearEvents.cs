using System;

namespace RTPuzzle
{
    public class FearedStartAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public FearedStartAIBehaviorEvent(Action eventProcessedCallback)
        {
            this.eventProcessedCallback = eventProcessedCallback;
        }
    }

    public class FearedForcedAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private float fearedTime;

        public FearedForcedAIBehaviorEvent(float fearedTime)
        {
            this.fearedTime = fearedTime;
        }

        public float FearedTime { get => fearedTime; set => fearedTime = value; }
    }

    public class FearedEndAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public FearedEndAIBehaviorEvent(Action eventProcessedCallback)
        {
            this.eventProcessedCallback = eventProcessedCallback;
        }
    }


    public static class AIFearEvents
    {

        public static void Feared_Start(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearStarted(PuzzleAIBehaviorExternalEvent.Cast<FearedStartAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        public static void Feared_Forced(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearedForced(PuzzleAIBehaviorExternalEvent.Cast<FearedForcedAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        public static void Feared_End(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.SetManagerState(null);
                // to not have inactive frame.
                genericAiBehavior.ForceUpdateAIBehavior();
            }
        }
    }
}
