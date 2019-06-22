using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{
    public class CutscenePositionsType : MonoBehaviour
    {
        [CustomEnum()]
        public CutsceneId CutsceneId;

        private Dictionary<CutscenePositionMarkerID, CutscenePositionMarker> cutscenePositionMarkers;
        private bool hasBeenInit = false;

        public CutscenePositionMarker GetCutscenePosition(CutscenePositionMarkerID cutscenePositionMarkerID)
        {
            if (!this.hasBeenInit)
            {
                this.Init();
            }

            return this.cutscenePositionMarkers[cutscenePositionMarkerID];
        }

        private void Init()
        {
            this.cutscenePositionMarkers = new Dictionary<CutscenePositionMarkerID, CutscenePositionMarker>();
            foreach (var cutscenePositionMarker in this.GetComponentsInChildren<CutscenePositionMarker>())
            {
                this.cutscenePositionMarkers[cutscenePositionMarker.CutscenePositionMarkerID] = cutscenePositionMarker;
            }
        }

    }
}
