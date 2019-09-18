using GameConfigurationID;

namespace RTPuzzle
{
    public interface IAgentEscapeEventListener
    {
        void PZ_EVT_AI_EscapingStart(AIObjectDataRetriever AIObjectDataRetriever, AnimationID playedAnimation);
        void PZ_EVT_AI_NoMoreEscaping(AIObjectDataRetriever AIObjectDataRetriever);
    }
}
