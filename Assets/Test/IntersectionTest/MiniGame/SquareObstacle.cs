using CoreGame;
using System.Collections.Generic;
using UnityEngine;

public class SquareObstacle : MonoBehaviour
{
    [Tooltip("Avoid tracking of value every frame. But obstacle frustum will never be updated")]
    public bool IsStatic = true;
    private List<FrustumV2> FaceFrustums;

    #region Internal Managers
    private SquareObstacleChangeTracker SquareObstacleChangeTracker;
    #endregion

#if UNITY_EDITOR
    public bool DebugGizmo;
    public bool DebugIntersection;
    public Transform IntersectionPoint;
    private List<FrustumPointsWorldPositions> FrustumToDisplay;
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
        this.SquareObstacleChangeTracker = new SquareObstacleChangeTracker(this);

        this.CreateAndAddFrustum(Quaternion.Euler(0, 0, 0), 1);
        this.CreateAndAddFrustum(Quaternion.Euler(0, 0, 0), -1);
        this.CreateAndAddFrustum(Quaternion.Euler(0, 90, 0), 1);
        this.CreateAndAddFrustum(Quaternion.Euler(0, 90, 0), -1);
    }

    public bool Tick(float d)
    {
        if (this.IsStatic)
        {
            return false;
        }
        else
        {
            return this.SquareObstacleChangeTracker.Tick(d);
        }
    }

    private void CreateAndAddFrustum(Quaternion deltaRotation, float F1FaceZOffset)
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

    public List<FrustumPointsWorldPositions> ComputeOcclusionFrustums(Vector3 worldPositionStartAngleDefinition)
    {

#if UNITY_EDITOR
        if (this.DebugGizmo)
        {
            if (this.FrustumToDisplay == null)
            {
                FrustumToDisplay = new List<FrustumPointsWorldPositions>();
            }
            else
            {
                this.FrustumToDisplay.Clear();
            }
        }
#endif

        List<FrustumPointsWorldPositions> frustumPointsWorldPositions = new List<FrustumPointsWorldPositions>();

        foreach (var faceFrustum in this.FaceFrustums)
        {
            faceFrustum.WorldPosition = this.transform.position;
            faceFrustum.WorldRotation = this.transform.rotation;
            faceFrustum.LossyScale = this.transform.lossyScale;
            faceFrustum.SetLocalStartAngleProjection(worldPositionStartAngleDefinition);
            this.ComputeSideFrustum(frustumPointsWorldPositions, faceFrustum);
        }

        return frustumPointsWorldPositions;
    }

    public List<FrustumPointsWorldPositions> ComputeOcclusionFrustums_FromDedicatedThread(SquareObstacleFrustumCalculationResult obstacleCalucation)
    {

#if UNITY_EDITOR
        if (this.DebugGizmo)
        {
            if (this.FrustumToDisplay == null)
            {
                FrustumToDisplay = new List<FrustumPointsWorldPositions>();
            }
            else
            {
                this.FrustumToDisplay.Clear();
            }
        }
#endif

        List<FrustumPointsWorldPositions> frustumPointsWorldPositions = new List<FrustumPointsWorldPositions>();

        foreach (var faceFrustum in this.FaceFrustums)
        {
            faceFrustum.WorldPosition = obstacleCalucation.ObstaclePosition;
            faceFrustum.WorldRotation = obstacleCalucation.ObstacleRotation;
            faceFrustum.LossyScale = obstacleCalucation.ObstacleLossyScale;
            faceFrustum.SetLocalStartAngleProjection(obstacleCalucation.WorldPositionStartAngleDefinition);
            this.ComputeSideFrustum(frustumPointsWorldPositions, faceFrustum);
        }

        return frustumPointsWorldPositions;
    }


    private void ComputeSideFrustum(List<FrustumPointsWorldPositions> frustumBufferDatas, FrustumV2 frustum)
    {
        frustum.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
        frustumBufferDatas.Add(new FrustumPointsWorldPositions(C1, C2, C3, C4, C5, C6, C7, C8));
#if UNITY_EDITOR
        if (this.DebugGizmo)
        {
            FrustumToDisplay.Add(new FrustumPointsWorldPositions(C1, C2, C3, C4, C5, C6, C7, C8));
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

[System.Serializable]
public struct FrustumPointsWorldPositions
{
    public Vector3 FC1;
    public Vector3 FC2;
    public Vector3 FC3;
    public Vector3 FC4;
    public Vector3 FC5;
    public Vector3 FC6;
    public Vector3 FC7;
    public Vector3 FC8;

    public FrustumPointsWorldPositions(Vector3 fC1, Vector3 fC2, Vector3 fC3, Vector3 fC4, Vector3 fC5, Vector3 fC6, Vector3 fC7, Vector3 fC8)
    {
        FC1 = fC1;
        FC2 = fC2;
        FC3 = fC3;
        FC4 = fC4;
        FC5 = fC5;
        FC6 = fC6;
        FC7 = fC7;
        FC8 = fC8;
    }

    public static int GetByteSize()
    {
        return (8 * 3 * sizeof(float));
    }

}

class SquareObstacleChangeTracker
{
    private SquareObstacle SquareObstacleRef;

    public SquareObstacleChangeTracker(SquareObstacle squareObstacleRef)
    {
        SquareObstacleRef = squareObstacleRef;
    }

    private Vector3 lastFramePosition;
    private Quaternion lastFrameRotation;
    private Vector3 lastFrameScale;

    public bool Tick(float d)
    {
        bool hasChanged = false;
        if (this.lastFramePosition != this.SquareObstacleRef.transform.position ||
            this.lastFrameRotation != this.SquareObstacleRef.transform.rotation ||
            this.lastFrameScale != this.SquareObstacleRef.transform.lossyScale)
        {
            hasChanged = true;
        }
        this.lastFramePosition = this.SquareObstacleRef.transform.position;
        this.lastFrameRotation = this.SquareObstacleRef.transform.rotation;
        this.lastFrameScale = this.SquareObstacleRef.transform.lossyScale;
        return hasChanged;
    }
}