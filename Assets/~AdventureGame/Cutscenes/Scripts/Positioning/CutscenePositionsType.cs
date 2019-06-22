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

        private void Start()
        {
            GameObject.FindObjectOfType<CutscenePositionsManager>().AddPositions(this);
        }

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

#if UNITY_EDITOR
        [Header("Gizmo")]
        public bool ShowGizmo;

        private void OnDrawGizmos()
        {
            if (this.ShowGizmo)
            {
                foreach (var cutscenePositionMarker in this.GetComponentsInChildren<CutscenePositionMarker>())
                {
                    cutscenePositionMarker.GizmoTick();
                }
            }
        }
#endif

    }
}
