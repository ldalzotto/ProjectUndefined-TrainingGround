using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IAttractiveObjectModuleEvent 
    {
        void OnAIAttractedStart(AIObjectDataRetriever AIObjectDataRetriever);
        void OnAIAttractedEnd(AIObjectDataRetriever AIObjectDataRetriever);

        void OnAITriggerEnter(AIObjectDataRetriever AIObjectDataRetriever);
        void OnAITriggerStay(AIObjectDataRetriever AIObjectDataRetriever);
        void OnAITriggerExit(AIObjectDataRetriever AIObjectDataRetriever);

        void OnAttractiveObjectPlayerActionExecuted(RaycastHit attractiveObjectWorldPositionHit);
    }
}
