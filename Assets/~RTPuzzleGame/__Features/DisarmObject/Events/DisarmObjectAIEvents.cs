namespace RTPuzzle
{
    public static class DisarmObjectAIEvents
    {
        public static void DisarmingObject_Enter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
         PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIDisarmObjectManager>())
            {
                if (!genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>()))
                {
                    genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>().OnDisarmingObjectStart(PuzzleAIBehaviorExternalEvent.Cast<DisarmingObjectEnterAIbehaviorEvent>().disarmObjectModule);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>());
                }

            }
        }

        public static void DisarmingObject_Exit(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIDisarmObjectManager>())
            {
                if (genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>())
                {
                    genericAiBehavior.GetAIManager<AbstractAIDisarmObjectManager>().OnDisarmingObjectExit(PuzzleAIBehaviorExternalEvent.Cast<DisarmingObjectExitAIbehaviorEvent>().disarmObjectModule);
                    if (!genericAiBehavior.IsManagerEnabled<AbstractAIDisarmObjectManager>())
                    {
                        genericAiBehavior.SetManagerState(null);
                    }
                }
            }
        }

    }

    public class DisarmingObjectEnterAIbehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public IDisarmObjectModuleDataRetrieval disarmObjectModule;

        public DisarmingObjectEnterAIbehaviorEvent(IDisarmObjectModuleDataRetrieval disarmObjectModule)
        {
            this.disarmObjectModule = disarmObjectModule;
        }
    }

    public class DisarmingObjectExitAIbehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public IDisarmObjectModuleDataRetrieval disarmObjectModule;

        public DisarmingObjectExitAIbehaviorEvent(IDisarmObjectModuleDataRetrieval disarmObjectModule)
        {
            this.disarmObjectModule = disarmObjectModule;
        }
    }
}

