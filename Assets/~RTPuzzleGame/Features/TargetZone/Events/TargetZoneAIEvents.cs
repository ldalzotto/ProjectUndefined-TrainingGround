using UnityEngine;

namespace RTPuzzle
{
    public class TargetZoneTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public ITargetZoneModuleDataRetriever ITargetZoneModuleDataRetriever;

        public TargetZoneTriggerEnterAIBehaviorEvent(ITargetZoneModuleDataRetriever ITargetZoneModuleDataRetriever)
        {
            this.ITargetZoneModuleDataRetriever = ITargetZoneModuleDataRetriever;
        }
        
    }

    public class TargetZoneTriggerStayAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        public ITargetZoneModuleDataRetriever ITargetZoneModuleDataRetriever;

        public TargetZoneTriggerStayAIBehaviorEvent(ITargetZoneModuleDataRetriever ITargetZoneModuleDataRetriever)
        {
            this.ITargetZoneModuleDataRetriever = ITargetZoneModuleDataRetriever;
        }
        
    }

    public static class TargetZoneAIEvents
    {
        public static void TargetZone_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAITargetZoneManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>()))
            {
                var targetZoneTriggerEnterAIBehaviorEvent = PuzzleAIBehaviorExternalEvent.Cast<TargetZoneTriggerEnterAIBehaviorEvent>();
                if (targetZoneTriggerEnterAIBehaviorEvent.ITargetZoneModuleDataRetriever != null)
                {
                    if (!genericAiBehavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>())
                    {
                        Debug.Log(MyLog.Format("Target zone reset FOV"));
                        genericAiBehavior.FovManagerCalcuation.ResetFOV();
                    }

                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerEnter"));
                    genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>().TriggerTargetZoneEscape(targetZoneTriggerEnterAIBehaviorEvent.ITargetZoneModuleDataRetriever);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>());
                }
            }
        }

        public static void TargetZone_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAITargetZoneManager>()
                        && !genericAiBehavior.IsCurrentManagerEquals(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>())
                        && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>()))
            {
                var targetZoneTriggerStayAIBehaviorEvent = PuzzleAIBehaviorExternalEvent.Cast<TargetZoneTriggerStayAIBehaviorEvent>();
                if (!genericAiBehavior.IsManagerEnabled<AbstractAIProjectileEscapeManager>())
                {
                    Debug.Log(MyLog.Format("Target zone reset FOV"));
                    genericAiBehavior.FovManagerCalcuation.ResetFOV();
                }
                if (targetZoneTriggerStayAIBehaviorEvent.ITargetZoneModuleDataRetriever != null)
                {
                    Debug.Log(MyLog.Format("AI - OnTargetZoneTriggerStay"));
                    genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>().TriggerTargetZoneEscape(targetZoneTriggerStayAIBehaviorEvent.ITargetZoneModuleDataRetriever);
                    genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAITargetZoneManager>());
                }
            }
        }
    }
}
