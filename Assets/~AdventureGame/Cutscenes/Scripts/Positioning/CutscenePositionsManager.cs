using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class CutscenePositionsManager : MonoBehaviour
    {

        private Dictionary<CutsceneId, CutscenePositionsType> cutscenePositionsType;

        public void Init()
        {
            this.cutscenePositionsType = new Dictionary<CutsceneId, CutscenePositionsType>();
            var CutscenePositionsType = GameObject.FindObjectsOfType<CutscenePositionsType>();
            if (CutscenePositionsType != null)
            {
                foreach(var CutscenePositionType in CutscenePositionsType)
                {
                    this.AddPositions(CutscenePositionType);
                }
            }
        }

        private void AddPositions(CutscenePositionsType cutscenePositionsType)
        {
            this.cutscenePositionsType[cutscenePositionsType.CutsceneId] = cutscenePositionsType;
        }

        public CutscenePositionMarker GetCutscenePosition(CutsceneId cutsceneId, CutscenePositionMarkerID cutscenePositionMarkerID)
        {
            return this.cutscenePositionsType[cutsceneId].GetPosition(cutscenePositionMarkerID);
        }

    }
}
