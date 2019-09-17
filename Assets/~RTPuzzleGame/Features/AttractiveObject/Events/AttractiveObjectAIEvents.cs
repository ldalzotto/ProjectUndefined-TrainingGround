using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public static class AttractiveObjectAIEvents 
    {
        public static void AttractiveObject_TriggerEnter(GenericPuzzleAIBehavior genericAiBehavior, PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIAttractiveObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>()))
            {
                Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerEnter"));
                genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>().ComponentTriggerEnter(PuzzleAIBehaviorExternalEvent.Cast<AttractiveObjectTriggerEnterAIBehaviorEvent>().AttractivePosition,
                        PuzzleAIBehaviorExternalEvent.Cast<AttractiveObjectTriggerEnterAIBehaviorEvent>().AttractiveObjectType);
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>());
            }
        }

        public static void AttractiveObject_TriggerStay(GenericPuzzleAIBehavior genericAiBehavior, PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIAttractiveObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>()))
            {
                //Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerStay"));
                genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>().ComponentTriggerStay(PuzzleAIBehaviorExternalEvent.Cast<AttractiveObjectTriggerStayAIBehaviorEvent>().AttractivePosition,
                        PuzzleAIBehaviorExternalEvent.Cast<AttractiveObjectTriggerStayAIBehaviorEvent>().AttractiveObjectType);
                genericAiBehavior.SetManagerState(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>());
            }
        }

        public static void AttractiveObject_TriggerExit(GenericPuzzleAIBehavior genericAiBehavior, PuzzleAIBehaviorExternalEvent PuzzleAIBehaviorExternalEvent)
        {
            if (genericAiBehavior.IsManagerInstanciated<AbstractAIAttractiveObjectManager>() && genericAiBehavior.IsManagerAllowedToBeActive(genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>()))
            {
                Debug.Log(MyLog.Format("AI - OnAttractiveObjectTriggerExit"));
                genericAiBehavior.GetAIManager<AbstractAIAttractiveObjectManager>().ComponentTriggerExit(PuzzleAIBehaviorExternalEvent.Cast<AttractiveObjectTriggerExitAIBehaviorEvent>().AttractiveObjectType);
                if (!genericAiBehavior.IsManagerEnabled<AbstractAIAttractiveObjectManager>())
                {
                    genericAiBehavior.SetManagerState(null);
                    genericAiBehavior.ForceUpdateAIBehavior();
                }
            }
        }
    }

    public class AttractiveObjectTriggerEnterAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 attractivePosition;
        private AttractiveObjectModule attractiveObjectType;

        public AttractiveObjectTriggerEnterAIBehaviorEvent(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType)
        {
            this.attractivePosition = attractivePosition;
            this.attractiveObjectType = attractiveObjectType;
        }

        public Vector3 AttractivePosition { get => attractivePosition; }
        public AttractiveObjectModule AttractiveObjectType { get => attractiveObjectType; }
    }

    public class AttractiveObjectTriggerStayAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {
        private Vector3 attractivePosition;
        private AttractiveObjectModule attractiveObjectType;

        public AttractiveObjectTriggerStayAIBehaviorEvent(Vector3 attractivePosition, AttractiveObjectModule attractiveObjectType)
        {
            this.attractivePosition = attractivePosition;
            this.attractiveObjectType = attractiveObjectType;
        }

        public Vector3 AttractivePosition { get => attractivePosition; }
        public AttractiveObjectModule AttractiveObjectType { get => attractiveObjectType; }
    }

    public class AttractiveObjectTriggerExitAIBehaviorEvent : PuzzleAIBehaviorExternalEvent
    {

        private AttractiveObjectModule attractiveObjectType;

        public AttractiveObjectTriggerExitAIBehaviorEvent(AttractiveObjectModule attractiveObjectType)
        {
            this.attractiveObjectType = attractiveObjectType;
        }

        public AttractiveObjectModule AttractiveObjectType { get => attractiveObjectType; }
    }

}
