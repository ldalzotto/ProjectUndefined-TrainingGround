using CoreGame;
using GameConfigurationID;

namespace RTPuzzle
{
    public class AIPositionsType : AbstractPositionsType<AIPositionMarkerID, AIPositionMarker>
    {
        [CustomEnum()]
        public AIObjectID AiID;
    }
}
