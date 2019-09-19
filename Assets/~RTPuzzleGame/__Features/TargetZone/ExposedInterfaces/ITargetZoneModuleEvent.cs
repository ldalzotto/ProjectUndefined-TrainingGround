using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface ITargetZoneModuleEvent
    {
        void OnAITriggerEnter(AIObjectDataRetriever AIObjectDataRetriever);
        void OnAITriggerStay(AIObjectDataRetriever AIObjectDataRetriever);
    }
}
