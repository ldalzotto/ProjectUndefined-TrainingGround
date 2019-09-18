using UnityEngine;

namespace RTPuzzle
{
    public class EscapeWithoutTriggerStartAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 threatStartPoint;
        private float escapeSemiAngle;
        private float escapeDistance;

        public EscapeWithoutTriggerStartAIBehaviorEvent(Vector3 threatStartPoint, float escapeSemiAngle, float escapeDistance)
        {
            this.threatStartPoint = threatStartPoint;
            this.escapeSemiAngle = escapeSemiAngle;
            this.escapeDistance = escapeDistance;
        }

        public Vector3 ThreatStartPoint { get => threatStartPoint; }
        public float EscapeSemiAngle { get => escapeSemiAngle; }
        public float EscapeDistance { get => escapeDistance; }
    }

    public static class AgentEscapeAIEvents
    {
        public static void EscapeWithoutTrigger_Start(GenericPuzzleAIBehavior genericAiBehavior, GenericPuzzleAIBehaviorExternalEventManager GenericPuzzleAIBehaviorExternalEventManager,
            PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            Debug.Log(MyLog.Format("AI - EscapeWithoutTrigger_Start"));
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIEscapeWithoutTriggerManager>())
            {
                genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>().OnEscapeStart(PuzzleAIBehaviorExternalEvent.Cast<EscapeWithoutTriggerStartAIBehaviorEvent>());
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIEscapeWithoutTriggerManager>());
            }
        }
    }

}
