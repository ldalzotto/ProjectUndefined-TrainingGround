using CoreGame;
using GameConfigurationID;

namespace RTPuzzle
{
    public class AIPositionsType : AbstractPositionsType<AIPositionMarkerID, AIPositionMarker>
    {
        [CustomEnum()]
        public AiID AiID;
    }
}
