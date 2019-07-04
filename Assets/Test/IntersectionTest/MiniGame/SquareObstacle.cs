using CoreGame;
using System.Collections.Generic;
using UnityEngine;

public class SquareObstacle : MonoBehaviour
{
    private List<FrustumV2> FaceFrustums;

#if UNITY_EDITOR
    public bool DebugGizmo;
    public bool DebugIntersection;
    public Transform IntersectionPoint;
    private List<FrustumBufferData> FrustumToDisplay;
#endif

    public static SquareObstacle FromCollisionType(CollisionType collisionType)
    {
        if (collisionType.IsObstacle)
        {
            return collisionType.GetComponent<SquareObstacle>();
        }
        return null;
    }

    public void Init()
    {
        this.FaceFrustums = new List<FrustumV2>();

        this.CreateAnnAddFrustum(Quaternion.Euler(0, 0, 0), 1);
        this.CreateAnnAddFrustum(Quaternion.Euler(0, 0, 0), -1);
        this.CreateAnnAddFrustum(Quaternion.Euler(0, 90, 0), 1);
        this.CreateAnnAddFrustum(Quaternion.Euler(0, 90, 0), -1);
    }

    private void CreateAnnAddFrustum(Quaternion deltaRotation, float F1FaceZOffset)
    {
        var frustum = new FrustumV2();
        frustum.FaceDistance = 9999f;
        frustum.F1.Width = 1f;
        frustum.F1.Height = 100f;
        frustum.UseFaceDefinition = false;
        frustum.UsePointDefinition = true;
        frustum.WorldRotation = this.transform.rotation;
        frustum.DeltaRotation = deltaRotation;
        frustum.F1.FaceOffsetFromCenter.z = F1FaceZOffset;
        this.FaceFrustums.Add(frustum);
    }

    public List<FrustumBufferData> ComputeOcclusionFrustums(Vector3 worldPositionStartAngleDefinition, int basePositionBufferIndex)
    {
        List<FrustumBufferData> frustumBufferDatas = new List<FrustumBufferData>();

        foreach (var faceFrustum in this.FaceFrustums)
        {
            faceFrustum.WorldPosition = this.transform.position;
            faceFrustum.WorldRotation = this.transform.rotation;
            faceFrustum.LossyScale = this.transform.lossyScale;
            faceFrustum.SetLocalStartAngleProjection(worldPositionStartAngleDefinition);
            this.ComputeSideFrustum(frustumBufferDatas, faceFrustum, basePositionBufferIndex);
        }

        return frustumBufferDatas;
    }

    private void ComputeSideFrustum(List<FrustumBufferData> frustumBufferDatas, FrustumV2 frustum, int basePositionBufferIndex)
    {
        frustum.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
        frustumBufferDatas.Add(new FrustumBufferData(C1, C2, C3, C4, C5, C6, C7, C8, basePositionBufferIndex));
#if UNITY_EDITOR
        if (this.DebugGizmo)
        {
            if (this.FrustumToDisplay == null)
            {
                FrustumToDisplay = new List<FrustumBufferData>();
            }
            FrustumToDisplay.Add(new FrustumBufferData(C1, C2, C3, C4, C5, C6, C7, C8, basePositionBufferIndex));
        }
#endif
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (this.DebugGizmo)
        {

            var oldGizmoColor = Gizmos.color;

            if (this.DebugIntersection && this.FaceFrustums != null && this.IntersectionPoint != null)
            {
                bool isIsideOcclusionFrustum = false;
                foreach (var faceFrustum in this.FaceFrustums)
                {
                    isIsideOcclusionFrustum = isIsideOcclusionFrustum || Intersection.PointInsideFrustum(faceFrustum, this.IntersectionPoint.transform.position);
                }
                if (isIsideOcclusionFrustum)
                {
                    Gizmos.color = Color.green;
                }
                else { Gizmos.color = Color.red; }
            }

            if (this.FrustumToDisplay != null)
            {
                foreach (var Frustum in FrustumToDisplay)
                {
                    this.DrawFace(Frustum.FC1, Frustum.FC2, Frustum.FC3, Frustum.FC4);
                    this.DrawFace(Frustum.FC1, Frustum.FC5, Frustum.FC6, Frustum.FC2);
                    this.DrawFace(Frustum.FC2, Frustum.FC6, Frustum.FC7, Frustum.FC3);
                    this.DrawFace(Frustum.FC3, Frustum.FC7, Frustum.FC8, Frustum.FC4);
                    this.DrawFace(Frustum.FC4, Frustum.FC8, Frustum.FC5, Frustum.FC1);
                    this.DrawFace(Frustum.FC5, Frustum.FC6, Frustum.FC7, Frustum.FC8);
                }
                FrustumToDisplay.Clear();
            }

            Gizmos.color = oldGizmoColor;
        }
    }

    private void DrawFace(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4)
    {
        Gizmos.DrawLine(C1, C2);
        Gizmos.DrawLine(C2, C3);
        Gizmos.DrawLine(C3, C4);
        Gizmos.DrawLine(C4, C1);
    }
#endif
}
