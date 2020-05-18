﻿using UnityEngine;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CoreGame
{
    public class AbstractPositionMarker<ID> : MonoBehaviour where ID : Enum
    {

        [FormerlySerializedAs("CutscenePositionMarkerID")]
        [CustomEnum(isCreateable: true)]
        public ID PositionMarkerID;

#if UNITY_EDITOR
        [Header("GIZMO")]
        public float DirectionLineLength = 10f;
        public bool SingleGizmoEnabled = true;

        private void OnDrawGizmos()
        {
            if (this.SingleGizmoEnabled)
            {
                this.GizmoTick();
            }
        }

        public void GizmoTick()
        {
            var labelStyle = GUI.skin.GetStyle("Label");
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.yellow;

            Handles.Label(this.transform.position + new Vector3(0, 3, 0), PositionMarkerID.ToString(), labelStyle);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, 1f);
            Gizmos.DrawLine(this.transform.position, this.transform.position + (DirectionLineLength * this.transform.forward));
        }
#endif
    }

}
