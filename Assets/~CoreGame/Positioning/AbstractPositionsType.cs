using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace CoreGame
{
    public class AbstractPositionsType<ID, PositionMarker> : MonoBehaviour where ID : Enum where PositionMarker : AbstractPositionMarker<ID>
    {
        private Dictionary<ID, PositionMarker> positionMarkers;

        private bool hasBeenPopulated = false;

        public PositionMarker GetPosition(ID cutscenePositionMarkerID)
        {
            if (!this.hasBeenPopulated)
            {
                this.RetrievePositions();
            }

            return this.positionMarkers[cutscenePositionMarkerID];
        }

        private void RetrievePositions()
        {
            this.positionMarkers = new Dictionary<ID, PositionMarker>();
            foreach (var cutscenePositionMarker in this.GetComponentsInChildren<PositionMarker>())
            {
                this.positionMarkers[cutscenePositionMarker.PositionMarkerID] = cutscenePositionMarker;
            }
        }

#if UNITY_EDITOR
        [Header("Gizmo")]
        public bool ShowGizmo;

        private void OnDrawGizmos()
        {
            if (this.ShowGizmo)
            {
                foreach (var cutscenePositionMarker in this.GetComponentsInChildren<PositionMarker>())
                {
                    cutscenePositionMarker.GizmoTick();
                }
            }
        }
#endif
    }

}
