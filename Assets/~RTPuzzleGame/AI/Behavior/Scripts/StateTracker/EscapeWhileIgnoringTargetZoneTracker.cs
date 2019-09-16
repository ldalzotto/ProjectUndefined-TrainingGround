namespace RTPuzzle
{
    public class EscapeWhileIgnoringTargetZoneTracker : BehaviorStateTracker
    {
        private bool isEscapingWhileIgnoringTargets;

        public bool IsEscapingWhileIgnoringTargets { get => isEscapingWhileIgnoringTargets; }

        public void AfterDestinationReached(IPuzzleAIBehavior behavior)
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

        public void OnEventProcessed(IPuzzleAIBehavior behavior, PuzzleAIBehaviorExternalEvent externalEvent)
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
            return genericBehavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>() ||
                    genericBehavior.IsManagerEnabled<AbstractAIEscapeWithoutTriggerManager>() ||
                    genericBehavior.IsManagerEnabled<AbstractPlayerEscapeManager>();
        }
    }

}
