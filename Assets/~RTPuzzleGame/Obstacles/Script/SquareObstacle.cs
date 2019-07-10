
using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
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
        private Dictionary<FrustumV2, NormalGizmo> FrustumFacesNormal;
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
            this.CreateAndAddFrustum(Quaternion.Euler(0, 180, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, -90, 0), 1);
        }

        #region Logical Conditions
        public bool IsWorldPositionPointContainedInOcclusionFrustum(Vector3 comparisonWorldPoint)
        {
            bool isIsideOcclusionFrustum = false;
            foreach (var faceFrustum in this.FaceFrustums)
            {
                isIsideOcclusionFrustum = isIsideOcclusionFrustum || Intersection.PointInsideFrustum(faceFrustum, comparisonWorldPoint);
            }
            return isIsideOcclusionFrustum;
        }
        #endregion

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
            frustum.F1.Height = 1f;
            frustum.WorldRotation = this.transform.rotation;
            frustum.DeltaRotation = deltaRotation;
            frustum.F1.FaceOffsetFromCenter.z = F1FaceZOffset;
            this.FaceFrustums.Add(frustum);
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
                if (this.FrustumFacesNormal == null)
                {
                    this.FrustumFacesNormal = new Dictionary<FrustumV2, NormalGizmo>();
                }
                else
                {
                    this.FrustumFacesNormal.Clear();
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

                faceFrustum.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);

                Vector3 frontFaceNormal = Vector3.Cross(C2 - C1, C4 - C1).normalized;

                if (Vector3.Dot(frontFaceNormal, C1 - obstacleCalucation.WorldPositionStartAngleDefinition) < 0)
                {
                    continue;
                }

                frustumPointsWorldPositions.Add(new FrustumPointsWorldPositions(C1, C2, C3, C4, C5, C6, C7, C8));
#if UNITY_EDITOR
                if (this.DebugGizmo)
                {
                    FrustumToDisplay.Add(new FrustumPointsWorldPositions(C1, C2, C3, C4, C5, C6, C7, C8));
                    this.FrustumFacesNormal.Add(faceFrustum, new NormalGizmo(frontFaceNormal, C1));
                }
#endif
            }

            // Debug.Log(frustumPointsWorldPositions.Count);

            return frustumPointsWorldPositions;

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

                    int counter = 0;
                    foreach (var Frustum in FrustumToDisplay)
                    {
                        if (counter == 0) { Gizmos.color = Color.red; }
                        else if (counter == 1) { Gizmos.color = Color.green; }
                        else if (counter == 2) { Gizmos.color = Color.blue; }
                        else if (counter == 3) { Gizmos.color = Color.yellow; }

                        this.DrawFace(Frustum.FC1, Frustum.FC2, Frustum.FC3, Frustum.FC4);
                        this.DrawFace(Frustum.FC1, Frustum.FC5, Frustum.FC6, Frustum.FC2);
                        this.DrawFace(Frustum.FC2, Frustum.FC6, Frustum.FC7, Frustum.FC3);
                        this.DrawFace(Frustum.FC3, Frustum.FC7, Frustum.FC8, Frustum.FC4);
                        this.DrawFace(Frustum.FC4, Frustum.FC8, Frustum.FC5, Frustum.FC1);
                        this.DrawFace(Frustum.FC5, Frustum.FC6, Frustum.FC7, Frustum.FC8);

                        counter++;
                    }
                }

                if (this.FrustumFacesNormal != null)
                {
                    int counter = 0;
                    foreach (var FrustumFaceNormal in this.FrustumFacesNormal)
                    {
                        if (counter == 0) { Gizmos.color = Color.red; }
                        else if (counter == 1) { Gizmos.color = Color.green; }
                        else if (counter == 2) { Gizmos.color = Color.blue; }
                        else if (counter == 3) { Gizmos.color = Color.yellow; }
                        Gizmos.DrawLine(FrustumFaceNormal.Value.StartPoint, FrustumFaceNormal.Value.StartPoint + FrustumFaceNormal.Value.NormalizedNormal);
                        counter++;
                    }
                }

                if (this.DebugIntersection && this.FaceFrustums != null)
                {
                    if (this.IsWorldPositionPointContainedInOcclusionFrustum(this.IntersectionPoint.transform.position))
                    {
                        Gizmos.color = Color.green;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }

                    Gizmos.DrawWireSphere(this.IntersectionPoint.transform.position, 1f);
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

#if UNITY_EDITOR
    class NormalGizmo
    {
        public Vector3 NormalizedNormal;
        public Vector3 StartPoint;

        public NormalGizmo(Vector3 normalizedNormal, Vector3 startPoint)
        {
            NormalizedNormal = normalizedNormal;
            StartPoint = startPoint;
        }
    }
#endif
}
