using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public interface IAIFearEventListener
    {
        void PZ_EVT_AI_FearedStunned_Start(AIObjectDataRetriever AIObjectDataRetriever);
        void PZ_EVT_AI_FearedForced(AIObjectDataRetriever AIObjectDataRetriever, float fearTime);
        void PZ_EVT_AI_FearedStunned_Ended(AIObjectDataRetriever AIObjectDataRetriever);
    }
}
