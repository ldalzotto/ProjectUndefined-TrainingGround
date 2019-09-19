﻿using GameConfigurationID;
using System;
using System.Collections.Generic;
using UnityEngine;

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
            {typeof(DisarmingObjectEnterAIbehaviorEvent).Name, 7 },
            {typeof(DisarmingObjectExitAIbehaviorEvent).Name, 8 },
            {typeof(AttractiveObjectTriggerExitAIBehaviorEvent).Name, 9 },
            {typeof(AttractiveObjectDestroyedAIBehaviorEvent).Name, 10 },
            {typeof(AttractiveObjectTriggerStayAIBehaviorEvent).Name, 11 },
            {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent).Name, 12 },
            {typeof(SightInRangeEnterAIBehaviorEvent).Name, 13 },
            {typeof(SightInRangeExitAIBehaviorEvent).Name, 14 }
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
                {typeof(FearedStartAIBehaviorEvent),  Feared_Start},
                {typeof(FearedForcedAIBehaviorEvent), Feared_Forced },
                {typeof(FearedEndAIBehaviorEvent), Feared_End },
                {typeof(AttractiveObjectTriggerEnterAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerEnter },
                {typeof(AttractiveObjectTriggerStayAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerStay },
                {typeof(AttractiveObjectTriggerExitAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_TriggerExit },
                {typeof(AttractiveObjectDestroyedAIBehaviorEvent), AttractiveObjectAIEvents.AttractiveObject_Destroyed },
                {typeof(TargetZoneTriggerEnterAIBehaviorEvent), TargetZoneAIEvents.TargetZone_TriggerEnter },
                {typeof(TargetZoneTriggerStayAIBehaviorEvent), TargetZoneAIEvents.TargetZone_TriggerStay },
                {typeof(PlayerEscapeStartAIBehaviorEvent), AgentEscapeAIEvents.PlayerEscape_Start },
                {typeof(SightInRangeEnterAIBehaviorEvent), SightAIEvents.SightInRange_Enter },
                {typeof(SightInRangeExitAIBehaviorEvent), SightAIEvents.SightInRange_Exit },
                {typeof(DisarmingObjectEnterAIbehaviorEvent), DisarmObjectAIEvents.DisarmingObject_Enter },
                {typeof(DisarmingObjectExitAIbehaviorEvent), DisarmObjectAIEvents.DisarmingObject_Exit },
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

        private static void Feared_Start(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearStarted(PuzzleAIBehaviorExternalEvent.Cast<FearedStartAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        private static void Feared_Forced(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIFearStunManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIFearStunManager>().OnFearedForced(PuzzleAIBehaviorExternalEvent.Cast<FearedForcedAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIFearStunManager>());
            }
        }

        private static void Feared_End(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
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
}