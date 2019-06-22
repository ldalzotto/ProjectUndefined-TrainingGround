using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class CutscenePositionsManager : MonoBehaviour
    {

        private Dictionary<CutsceneId, CutscenePositionsType> cutscenePositionsType = new Dictionary<CutsceneId, CutscenePositionsType>();

        public void AddPositions(CutscenePositionsType cutscenePositionsType)
        {
            this.cutscenePositionsType[cutscenePositionsType.CutsceneId] = cutscenePositionsType;
        }

        public CutscenePositionMarker GetCutscenePosition(CutsceneId cutsceneId, CutscenePositionMarkerID cutscenePositionMarkerID)
        {
            return this.cutscenePositionsType[cutsceneId].GetCutscenePosition(cutscenePositionMarkerID);
        }

    }
}
