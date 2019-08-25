using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameConfigurationID;

namespace RTPuzzle
{
    public class AIPositionsManager : MonoBehaviour
    {
        private Dictionary<AIObjectID, AIPositionsType> aiPositionsType;

        public void Init()
        {
            this.aiPositionsType = new Dictionary<AIObjectID, AIPositionsType>();
            var AIPositionsType = GameObject.FindObjectsOfType<AIPositionsType>();
            if (AIPositionsType != null)
            {
                foreach (var AIPositionType in AIPositionsType)
                {
                    this.AddPositions(AIPositionType);
                }
            }
        }
        
        private void AddPositions(AIPositionsType AIPositionsType)
        {
            this.aiPositionsType[AIPositionsType.AiID] = AIPositionsType;
        }

        public AIPositionsType GetAIPositions(AIObjectID aiID)
        {
            this.aiPositionsType.TryGetValue(aiID, out AIPositionsType aiPositionsType);
            return aiPositionsType;
        }
    }
}
