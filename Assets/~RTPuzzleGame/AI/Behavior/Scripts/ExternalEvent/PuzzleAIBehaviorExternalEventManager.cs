using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    /// <summary>
    /// All changes of <see cref="AIBehaviorManagerContainer"/> behaviors interal state are handled by a <see cref="PuzzleAIBehaviorExternalEventManager"/>.
    /// The external events are recevied from <see cref="PuzzleAIBehavior{C}"/>.
    /// The events are processed in the defined order. Also, events occuring in physics step are stacked to be processed at the end of frame.
    /// </summary>
    public abstract class PuzzleAIBehaviorExternalEventManager
    {
        protected IPuzzleAIBehavior aiBehavior;

        protected abstract Dictionary<string, int> EventProcessingOrder { get; }

        public void Init(IPuzzleAIBehavior aiBehavior)
        {
            this.aiBehavior = aiBehavior;
        }

        private List<PuzzleAIBehaviorExternalEvent> waitingToConsumeEvents = new List<PuzzleAIBehaviorExternalEvent>();
        private List<PuzzleAIBehaviorExternalEvent> endBufferWaitingToConsumeEvent = new List<PuzzleAIBehaviorExternalEvent>();
        private bool isConsumingEvents = false;

        protected abstract BehaviorStateTrackerContainer BehaviorStateTrackerContainer { get; }

#if UNITY_EDITOR
        public BehaviorStateTrackerContainer GetBehaviorStateTrackerContainer() { return BehaviorStateTrackerContainer; }
#endif

        public void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent)
        {
            //Is the event pushed while consuming events ?
            if (this.isConsumingEvents)
            {
                //then we push the event to a buffer that will be consumed at the end of initial consume step
                this.endBufferWaitingToConsumeEvent.Add(externalEvent);
            }
            else
            {
                //If the event is occuring on physics engine timestep
                if (Time.inFixedTimeStep)
                {
                    //we push the events to be consumed after the fixed timestep by AI Behavior
                    this.waitingToConsumeEvents.Add(externalEvent);
                }
                else
                {
                    this.A_ProcessEvent(externalEvent, aiBehavior);
                }
            }

        }

        /// <summary>
        /// Consume all events stacked during the fixed timestep stage.
        /// </summary>
        public void ConsumeEventsQueued()
        {
            ConsumeWaitingEvents();

            //If the end buffer has been swapped, immediately consume events
            if (this.SwapEndBuffer())
            {
                this.ConsumeEventsQueued();
            }
        }

        private void ConsumeWaitingEvents()
        {
            this.isConsumingEvents = true;
            this.waitingToConsumeEvents.Sort(delegate (PuzzleAIBehaviorExternalEvent ev1, PuzzleAIBehaviorExternalEvent ev2)
            {
                return EventProcessingOrder[ev1.GetType().Name].CompareTo(EventProcessingOrder[ev2.GetType().Name]) * -1;
            });

            foreach (var externalEvent in this.waitingToConsumeEvents)
            {
                this.A_ProcessEvent(externalEvent, this.aiBehavior);
            }
            this.waitingToConsumeEvents.Clear();
            this.isConsumingEvents = false;
        }

        private bool SwapEndBuffer()
        {
            if (this.endBufferWaitingToConsumeEvent.Count > 0)
            {
                this.waitingToConsumeEvents.AddRange(this.endBufferWaitingToConsumeEvent);
                this.endBufferWaitingToConsumeEvent.Clear();
                return true;
            }
            return false;
        }

        private void A_ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior aiBehavior)
        {
            this.ProcessEvent(externalEvent, aiBehavior);

            //Executing behavior tracker event end call back
            foreach (var behaviorStateTracker in this.BehaviorStateTrackerContainer.BehaviorStateTrackers.Values)
            {
                behaviorStateTracker.OnEventProcessed(aiBehavior, externalEvent);
            }

            //Executing event end call back
            if (externalEvent.EventProcessedCallback != null)
            {
                externalEvent.EventProcessedCallback.Invoke();
            }
        }

        public abstract void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior aiBehavior);
        public void AfterDestinationReached(IPuzzleAIBehavior aiBehavior)
        {
            foreach (var behaviorStateTracker in this.BehaviorStateTrackerContainer.BehaviorStateTrackers.Values)
            {
                behaviorStateTracker.AfterDestinationReached(aiBehavior);
            }
        }

    }

    public abstract class PuzzleAIBehaviorExternalEvent
    {
        protected Action eventProcessedCallback;

        public Action EventProcessedCallback { get => eventProcessedCallback; }

        public T Cast<T>() where T : PuzzleAIBehaviorExternalEvent
        {
            return (T)this;
        }
    }
}
