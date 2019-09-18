using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class AIPositionsManager : MonoBehaviour
    {
        private Dictionary<AIPositionMarkerID, AIPositionMarker> aiPositionsType;

        public void Init()
        {
            this.aiPositionsType = new Dictionary<AIPositionMarkerID, AIPositionMarker>();
            var AIPositionMarkers = GameObject.FindObjectsOfType<AIPositionMarker>();
            if (AIPositionMarkers != null)
            {
                foreach (var AIPositionMarker in AIPositionMarkers)
                {
                    this.AddPositions(AIPositionMarker);
                }
            }
        }

        private void AddPositions(AIPositionMarker AIPositionMarker)
        {
            this.aiPositionsType[AIPositionMarker.PositionMarkerID] = AIPositionMarker;
        }

        public AIPositionMarker GetPosition(AIPositionMarkerID AIPositionMarkerID)
        {
            this.aiPositionsType.TryGetValue(AIPositionMarkerID, out AIPositionMarker AIPositionMarker);
            return AIPositionMarker;
        }
    }
}
