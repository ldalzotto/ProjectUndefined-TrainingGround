using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class EscapeWhileIgnoringTargetZoneTracker : BehaviorStateTracker
    {
        private bool isEscapingWhileIgnoringTargets;

        public bool IsEscapingWhileIgnoringTargets { get => isEscapingWhileIgnoringTargets; }

        public void AfterDestinationReached(IPuzzleAIBehavior<AbstractAIComponents> behavior)
        {
            this.isEscapingWhileIgnoringTargets = false;
            if (behavior.GetType() == typeof(GenericPuzzleAIBehavior))
            {
                var genericBehavior = (GenericPuzzleAIBehavior)behavior;
                if (this.IsGenericBehaviorInAStateThatAllowsEscapingWhileIgnoringTargets(genericBehavior))
                {
                    this.isEscapingWhileIgnoringTargets = true;
                }
            }
        }

        public void OnEventProcessed(IPuzzleAIBehavior<AbstractAIComponents> behavior, PuzzleAIBehaviorExternalEvent externalEvent)
        {
            this.isEscapingWhileIgnoringTargets = false;
            if (behavior.GetType() == typeof(GenericPuzzleAIBehavior))
            {
                var genericBehavior = (GenericPuzzleAIBehavior)behavior;
                if (this.IsGenericBehaviorInAStateThatAllowsEscapingWhileIgnoringTargets(genericBehavior))
                {
                    this.isEscapingWhileIgnoringTargets = true;
                }
            }
        }

        private bool IsGenericBehaviorInAStateThatAllowsEscapingWhileIgnoringTargets(GenericPuzzleAIBehavior genericBehavior)
        {
            return genericBehavior.IsEscapingFromProjectileWithTargetZones() || genericBehavior.IsEscapingWithoutTarget() || genericBehavior.IsEscapingFromPlayer();
        }
    }

}
