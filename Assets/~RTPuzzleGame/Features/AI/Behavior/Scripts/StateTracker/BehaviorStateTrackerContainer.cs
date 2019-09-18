using System;
using System.Collections.Generic;

namespace RTPuzzle
{
    public class BehaviorStateTrackerContainer
    {
        private Dictionary<Type, BehaviorStateTracker> behaviorStateTrackers;

        public BehaviorStateTrackerContainer(Dictionary<Type, BehaviorStateTracker> behaviorStateTrackers)
        {
            this.behaviorStateTrackers = behaviorStateTrackers;
        }

        public Dictionary<Type, BehaviorStateTracker> BehaviorStateTrackers { get => behaviorStateTrackers; }

        public T GetBehavior<T>()
        {
            return (T)this.behaviorStateTrackers[typeof(T)];
        }
    }

}
