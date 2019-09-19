using UnityEngine;

namespace RTPuzzle
{
    public partial class AttractiveObjectModule : IAttractiveObjectModuleEvent
    {
        #region IAttractiveObjectModuleEvent
        public void OnAIAttractedStart(AIObjectDataRetriever AIObjectDataRetriever)
        {
            this.CurrentlyAttractedAI.Add(AIObjectDataRetriever);
        }

        public void OnAIAttractedEnd(AIObjectDataRetriever AIObjectDataRetriever)
        {
            this.CurrentlyAttractedAI.Remove(AIObjectDataRetriever);
        }

        public void OnAITriggerEnter(AIObjectDataRetriever AIObjectDataRetriever)
        {
            AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new AttractiveObjectTriggerEnterAIBehaviorEvent(this.transform.position, this));
        }

        public void OnAITriggerStay(AIObjectDataRetriever AIObjectDataRetriever)
        {
            AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new AttractiveObjectTriggerStayAIBehaviorEvent(this.transform.position, this));
        }

        public void OnAITriggerExit(AIObjectDataRetriever AIObjectDataRetriever)
        {
            AIObjectDataRetriever.GetAIBehavior().ReceiveEvent(new AttractiveObjectTriggerExitAIBehaviorEvent(this));
        }

        public void OnAttractiveObjectPlayerActionExecuted(RaycastHit attractiveObjectWorldPositionHit)
        {
            this.transform.position = attractiveObjectWorldPositionHit.point;

            //TODO make the rotation relative to the player
            this.transform.LookAt(this.transform.position + Vector3.forward);
        }
        #endregion
    }

}
