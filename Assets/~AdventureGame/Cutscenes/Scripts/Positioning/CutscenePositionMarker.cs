using GameConfigurationID;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    public class CutscenePositionMarker : MonoBehaviour
    {
        [CustomEnum(isCreateable: true)]
        public CutscenePositionMarkerID CutscenePositionMarkerID;

        [Header("GIZMO")]
        public float DirectionLineLength = 10f;

#if UNITY_EDITOR
        public void GizmoTick()
        {
            var labelStyle = GUI.skin.GetStyle("Label");
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = Color.yellow;

            Handles.Label(this.transform.position + new Vector3(0, 3, 0), CutscenePositionMarkerID.ToString(), labelStyle);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(this.transform.position, 1f);
            Gizmos.DrawLine(this.transform.position, this.transform.position + (DirectionLineLength * this.transform.forward));
        }
#endif
    }
}