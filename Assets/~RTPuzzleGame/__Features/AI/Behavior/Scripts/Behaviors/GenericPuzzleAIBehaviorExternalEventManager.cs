using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class GenericPuzzleAIBehaviorExternalEventManager : PuzzleAIBehaviorExternalEventManager
    {

        private Dictionary<string, int> GenericPuzzleAIEventProcessingOrder = new Dictionary<string, int>()
        {
            {typeof(FearedEndAIBehaviorEvent).Name, 1 },
            {typeof(FearedStartAIBehaviorEvent).Name, 2 },
            {typeof(FearedForcedAIBehaviorEvent).Name, 3 },
            {typeof(TargetZoneTriggerStayAIBehaviorEvent).Name, 4 },
            {typeof(TargetZoneTriggerEnterAIBehaviorEvent).Name, 5 },
            {typeof(ProjectileTriggerEnterAIBehaviorEvent).Name, 6 },
            {typeof(AttractiveObjectTriggerExitAIBehaviorEvent).Name, 9 },
            {typeof(AttractiveObjectDestroyedAIBehaviorEvent).Name, 10 },
            {typeof(AttractiveObjectTriggerStayAIBehaviorEvent).Name, 11 },
            {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent).Name, 12 },
        };

        private BehaviorStateTrackerContainer trackerContainer = new BehaviorStateTrackerContainer(new Dictionary<Type, BehaviorStateTracker>()
            {
                {typeof(EscapeWhileIgnoringTargetZoneTracker), new EscapeWhileIgnoringTargetZoneTracker() }
            }
        );

        protected override Dictionary<string, int> EventProcessingOrder => GenericPuzzleAIEventProcessingOrder;

        protected override BehaviorStateTrackerContainer BehaviorStateTrackerContainer => trackerContainer;

        private Dictionary<Type, Action<GenericPuzzleAIBehavior, GenericPuzzleAIBehaviorExternalEventManager, PuzzleAIBehaviorExternalEvent>> EventsLookup =
            new Dictionary<Type, Action<GenericPuzzleAIBehavior, GenericPuzzleAIBehaviorExternalEventManager, PuzzleAIBehaviorExternalEvent>>()
        {
                {typeof(ProjectileTriggerEnterAIBehaviorEvent), LaunchProjectileAIEvents.Projectile_TriggerEnter },
                {typeof(EscapeWithoutTriggerStartAIBehaviorEvent), AgentEscapeAIEvents.EscapeWithoutTrigger_Start },
                {typeof(FearedStartAIBehaviorEvent),  AIFearEvents.Feared_Start},
                {typeof(FearedForcedAIBehaviorEvent),AIFearEvents. Feared_Forced },
                {typeof(FearedEndAIBehaviorEvent), AIFearEvents.Feared_End },
                {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerEnter },
                {typeof(AttractiveObjectTriggerStayAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerStay },
                {typeof(AttractiveObjectTriggerExitAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerExit },
                {typeof(AttractiveObjectDestroyedAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_Destroyed },
                {typeof(TargetZoneTriggerEnterAIBehaviorEvent), TargetZoneAIEvents.TargetZone_TriggerEnter },
                {typeof(TargetZoneTriggerStayAIBehaviorEvent), TargetZoneAIEvents.TargetZone_TriggerStay },
                {typeof(PlayerEscapeStartAIBehaviorEvent), AgentEscapeAIEvents.PlayerEscape_Start },
        };

        public override void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior aiBehavior)
        {
            var genericAiBehavior = (GenericPuzzleAIBehavior)aiBehavior;

            this.EventsLookup.TryGetValue(externalEvent.GetType(), out Action<GenericPuzzleAIBehavior, GenericPuzzleAIBehaviorExternalEventManager, PuzzleAIBehaviorExternalEvent> eventProcessing);
            if (eventProcessing != null)
            {
                eventProcessing.Invoke(genericAiBehavior, this, externalEvent);
            }
        }

        private void EventTypeCheck<T>(GenericPuzzleAIBehavior genericAiBehavior, PuzzleAIBehaviorExternalEvent ev, Action<GenericPuzzleAIBehavior, T> processEventAction) where T : PuzzleAIBehaviorExternalEvent
        {
            if (ev.GetType() == typeof(T))
            {
                if (processEventAction != null)
                {
                    processEventAction.Invoke(genericAiBehavior, (T)ev);
                }
            }
        }

    }


}
