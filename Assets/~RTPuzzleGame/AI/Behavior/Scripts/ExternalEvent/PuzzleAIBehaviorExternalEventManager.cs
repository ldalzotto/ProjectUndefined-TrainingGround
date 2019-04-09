using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class PuzzleAIBehaviorExternalEventManager
    {
        protected IPuzzleAIBehavior<AbstractAIComponents> aiBehavior;

        protected abstract Dictionary<string, int> EventProcessingOrder { get; }

        public void Init(IPuzzleAIBehavior<AbstractAIComponents> aiBehavior)
        {
            this.aiBehavior = aiBehavior;
        }

        private List<PuzzleAIBehaviorExternalEvent> waitingToConsumeEvents = new List<PuzzleAIBehaviorExternalEvent>();

        public void ReceiveEvent(PuzzleAIBehaviorExternalEvent externalEvent)
        {
            if (Time.inFixedTimeStep)
            {
                this.waitingToConsumeEvents.Add(externalEvent);
            }
            else
            {
                this.ProcessEvent(externalEvent, aiBehavior);
            }
        }

        public void ConsumeEvents()
        {
            this.waitingToConsumeEvents.Sort(delegate (PuzzleAIBehaviorExternalEvent ev1, PuzzleAIBehaviorExternalEvent ev2)
            {
                var e1 = EventProcessingOrder[ev1.GetType().Name];
                var e2 = EventProcessingOrder[ev2.GetType().Name];
                return EventProcessingOrder[ev1.GetType().Name].CompareTo(EventProcessingOrder[ev2.GetType().Name]);
            });

            foreach (var externalEvent in this.waitingToConsumeEvents)
            {
                this.ProcessEvent(externalEvent, this.aiBehavior);
            }
            this.waitingToConsumeEvents.Clear();
        }

        public abstract void ProcessEvent(PuzzleAIBehaviorExternalEvent externalEvent, IPuzzleAIBehavior<AbstractAIComponents> aiBehavior);
        public abstract void AfterDestinationReached(IPuzzleAIBehavior<AbstractAIComponents> aiBehavior);

    }

    public interface PuzzleAIBehaviorExternalEvent { }
}
