using CoreGame;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTPuzzle
{
    [System.Serializable]
    public class FrustumRangeShapeConfiguration : RangeShapeConfiguration
    {
        public FrustumV2 frustum;

#if UNITY_EDITOR
        public override void HandleDraw(Vector3 worldPosition, Quaternion worldRotation, Vector3 lossyScale)
        {
            this.frustum.SetCalculationDataForFaceBasedCalculation(worldPosition, worldRotation, lossyScale);
            var FrustumPointsPositions = this.frustum.CalculateFrustumPointsWorldPosV2();

            var oldGizmoColor = Handles.color;
            Handles.color = MyColors.HotPink;
            this.DrawFace(FrustumPointsPositions.FC1, FrustumPointsPositions.FC2, FrustumPointsPositions.FC3, FrustumPointsPositions.FC4);
            this.DrawFace(FrustumPointsPositions.FC1, FrustumPointsPositions.FC5, FrustumPointsPositions.FC6, FrustumPointsPositions.FC2);
            this.DrawFace(FrustumPointsPositions.FC2, FrustumPointsPositions.FC6, FrustumPointsPositions.FC7, FrustumPointsPositions.FC3);
            this.DrawFace(FrustumPointsPositions.FC3, FrustumPointsPositions.FC7, FrustumPointsPositions.FC8, FrustumPointsPositions.FC4);
            this.DrawFace(FrustumPointsPositions.FC4, FrustumPointsPositions.FC8, FrustumPointsPositions.FC5, FrustumPointsPositions.FC1);
            this.DrawFace(FrustumPointsPositions.FC5, FrustumPointsPositions.FC6, FrustumPointsPositions.FC7, FrustumPointsPositions.FC8);
            Handles.color = oldGizmoColor;
        }
        private void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
        {
            Handles.DrawLine(C1, C2);
            Handles.DrawLine(C2, C3);
            Handles.DrawLine(C3, C4);
            Handles.DrawLine(C4, C1);
        }
#endif
    }
}

