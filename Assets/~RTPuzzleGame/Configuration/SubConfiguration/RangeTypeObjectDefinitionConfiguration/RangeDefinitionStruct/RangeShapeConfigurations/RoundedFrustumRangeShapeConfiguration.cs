using UnityEngine;
using CoreGame;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class RoundedFrustumRangeShapeConfiguration : RangeShapeConfiguration
    {
        public FrustumV2 frustum;

#if UNITY_EDITOR
        public override void HandleDraw(Vector3 worldPosition, Quaternion worldRotation, Vector3 lossyScale)
        {
            this.frustum.SetCalculationDataForFaceBasedCalculation(worldPosition, worldRotation, lossyScale);
            this.frustum.CalculateFrustumPoints(out Vector3 FC1, out Vector3 FC2, out Vector3 FC3, out Vector3 FC4, out Vector3 FC5, out Vector3 FC6, out Vector3 FC7, out Vector3 FC8);

            var oldGizmoColor = Handles.color;
            Handles.color = MyColors.HotPink;
            this.DrawFace(FC1, FC2, FC3, FC4);
            this.DrawFace(FC1, FC5, FC6, FC2);
            this.DrawFace(FC2, FC6, FC7, FC3);
            this.DrawFace(FC3, FC7, FC8, FC4);
            this.DrawFace(FC4, FC8, FC5, FC1);
            this.DrawFace(FC5, FC6, FC7, FC8);

            Handles.DrawWireDisc(worldPosition, worldRotation * Vector3.up, this.frustum.F2.FaceOffsetFromCenter.z / 2f);

            Handles.color = oldGizmoColor;
        }
        private void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
        {
            Handles.DrawLine(C1, C2);
            Handles.DrawLine(C2, C3);
            Handles.DrawLine(C3, C4);
            Handles.DrawLine(C4, C1);
        }

    }
#endif
}