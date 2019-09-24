
using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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

        #region Internal Dependencies
        private BoxCollider BoxCollider;
        #endregion

#if UNITY_EDITOR
        public bool DebugGizmo;
        public bool DebugIntersection;
        public Transform IntersectionPoint;
        private List<FrustumPointsPositions> FrustumToDisplay;
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

        #region Data Retrieval
        public Vector3 GetPosition()
        {
            return (this.BoxCollider != null ? this.transform.TransformPoint(this.BoxCollider.center) : this.transform.position);
        }
        public Quaternion GetRotation() { return this.transform.rotation; }
        public Vector3 GetLossyScale()
        {
            return (this.BoxCollider != null ? this.BoxCollider.size : Vector3.one).Mul(this.transform.lossyScale);
        }
        #endregion

        public void Init()
        {
            this.BoxCollider = GetComponent<BoxCollider>();

            this.FaceFrustums = new List<FrustumV2>();
            this.SquareObstacleChangeTracker = new SquareObstacleChangeTracker(this);

            //Create frustum for all sides -> occlusions are only calculated for facing frustums.
            this.CreateAndAddFrustum(Quaternion.Euler(0, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 180, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, -90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(90, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(-90, 0, 0), 1);
        }

        #region Logical Conditions

        public bool IsWorldPositionPointContainedInOcclusionFrustum(Vector3 comparisonWorldPoint)
        {
#warning PointInsideFrustum trigger frustum points recalculation
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

        public List<FrustumPointsPositions> ComputeOcclusionFrustums_FromDedicatedThread(SquareObstacleFrustumCalculationResult obstacleCalucation)
        {
            Profiler.BeginThreadProfiling("ObstacleOcclusions", "ObstacleOcclusions");
            Profiler.BeginSample("ObstacleOcclusions");

#if UNITY_EDITOR
            if (this.DebugGizmo)
            {
                if (this.FrustumToDisplay == null)
                {
                    FrustumToDisplay = new List<FrustumPointsPositions>();
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
            List<FrustumPointsPositions> frustumPointsWorldPositions = new List<FrustumPointsPositions>();


            foreach (var faceFrustum in this.FaceFrustums)
            {
                faceFrustum.SetCalculationDataForPointProjection(obstacleCalucation.WorldPositionStartAngleDefinition, obstacleCalucation.ObstaclePosition, obstacleCalucation.ObstacleRotation, obstacleCalucation.ObstacleLossyScale);

                var FrustumPointsPositions = faceFrustum.CalculateFrustumPointsWorldPosV2();

                Vector3 frontFaceNormal = Vector3.Cross(FrustumPointsPositions.FC2 - FrustumPointsPositions.FC1, FrustumPointsPositions.FC4 - FrustumPointsPositions.FC1).normalized;

                //We filter faceFrustums that are not facing the initial calculation point -> they are useless because always occluded by front faces
                if (Vector3.Dot(frontFaceNormal, FrustumPointsPositions.FC1 - obstacleCalucation.WorldPositionStartAngleDefinition) < 0)
                {
                    continue;
                }

                frustumPointsWorldPositions.Add(FrustumPointsPositions);
#if UNITY_EDITOR
                if (this.DebugGizmo)
                {
                    FrustumToDisplay.Add(FrustumPointsPositions);
                    this.FrustumFacesNormal[faceFrustum] = new NormalGizmo(frontFaceNormal, FrustumPointsPositions.FC1);
                }
#endif
            }

            Profiler.EndSample();
            Profiler.EndThreadProfiling();
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
            if (this.lastFramePosition != this.SquareObstacleRef.GetPosition() ||
                this.lastFrameRotation != this.SquareObstacleRef.GetRotation() ||
                this.lastFrameScale != this.SquareObstacleRef.GetLossyScale())
            {
                hasChanged = true;
            }
            this.lastFramePosition = this.SquareObstacleRef.GetPosition();
            this.lastFrameRotation = this.SquareObstacleRef.GetRotation();
            this.lastFrameScale = this.SquareObstacleRef.GetLossyScale();
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
