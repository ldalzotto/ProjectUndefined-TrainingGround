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
        protected IPuzzleAIBehavior<AbstractAIComponents> aiBehavior;

        protected abstract Dictionary<string, int> EventProcessingOrder { get; }

        public void Init(IPuzzleAIBehavior<AbstractAIComponents> aiBehavior)
        {
            this.aiBehavior = aiBehavior;
        }

        private List<PuzzleAIBehaviorExternalEvent> waitingToConsumeEvents = new List<PuzzleAIBehaviorExternalEvent>();

        protected abstract BehaviorStateTrackerContainer BehaviorStateTrackerContainer { get; }

        public void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent)
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

        /// <summary>
        /// Consume all events stacked during the fixed timestep stage.
        /// </summary>
        public void ConsumeEventsQueued()
        {
            this.waitingToConsumeEvents.Sort(delegate (PuzzleAIBehaviorExternalEvent ev1, PuzzleAIBehaviorExternalEvent ev2)
            {
                return EventProcessingOrder[ev1.GetType().Name].CompareTo(EventProcessingOrder[ev2.GetType().Name]) * -1;
            });

            foreach (var externalEvent in this.waitingToConsumeEvents)
            {
                this.A_ProcessEvent(externalEvent, this.aiBehavior);
            }
            this.waitingToConsumeEvents.Clear();
        }

        private void A_ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior<AbstractAIComponents> aiBehavior)
        {
            // Debug.Log(MyLog.Format("Processing Event : " + externalEvent.GetType().Name) + " state : " + aiBehavior.ToString());
            this.ProcessEvent(externalEvent, aiBehavior);
            foreach (var behaviorStateTracker in this.BehaviorStateTrackerContainer.BehaviorStateTrackers.Values)
            {
                behaviorStateTracker.OnEventProcessed(aiBehavior);
            }
            // Debug.Log(MyLog.Format("After processing Event : " + externalEvent.GetType().Name) + " state : " + aiBehavior.ToString());
        }

        public abstract void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior<AbstractAIComponents> aiBehavior);
        public void AfterDestinationReached(IPuzzleAIBehavior<AbstractAIComponents> aiBehavior)
        {
            foreach (var behaviorStateTracker in this.BehaviorStateTrackerContainer.BehaviorStateTrackers.Values)
            {
                behaviorStateTracker.AfterDestinationReached(aiBehavior);
            }
        }

    }

    public interface PuzzleAIBehaviorExternalEvent { }
}
