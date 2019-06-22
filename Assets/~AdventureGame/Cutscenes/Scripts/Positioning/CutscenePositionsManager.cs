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
            foreach (var cutscenePositionType in this.GetComponentsInChildren<CutscenePositionsType>())
            {
                this.cutscenePositionsType[cutscenePositionType.CutsceneId] = cutscenePositionType;
            }
        }

        public CutscenePositionMarker GetCutscenePosition(CutsceneId cutsceneId, CutscenePositionMarkerID cutscenePositionMarkerID)
        {
            return this.cutscenePositionsType[cutsceneId].GetCutscenePosition(cutscenePositionMarkerID);
        }

    }
}
