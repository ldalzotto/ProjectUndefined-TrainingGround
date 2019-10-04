
using CoreGame;
using InteractiveObjectTest;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    [System.Serializable]
    public class SquareObstacleSystemInitializationData
    {
        [Tooltip("Avoid tracking of value every frame. But obstacle frustum will never be updated")]
        public bool IsStatic = true;
    }

    public class SquareObstacleSystem
    {
        private SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData;
        private List<FrustumV2> FaceFrustums;

        #region Internal Managers
        private SquareObstacleChangeTracker SquareObstacleChangeTracker;
        #endregion

        #region Internal Dependencies
        private ObstacleInteractiveObject AssociatedInteractiveObject;
        #endregion

        #region Data Retrieval
        public Vector3 GetPosition()
        {
            return this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition +
                          this.AssociatedInteractiveObject.ObstacleCollider.center;
        }

        public Quaternion GetRotation() { return this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldRotation; }
        public Vector3 GetLossyScale()
        {
            return (this.AssociatedInteractiveObject.ObstacleCollider != null ? this.AssociatedInteractiveObject.ObstacleCollider.size : Vector3.one).Mul(this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().LossyScale);
        }
        #endregion

        public SquareObstacleSystem(ObstacleInteractiveObject AssociatedInteractiveObject, SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.SquareObstacleSystemInitializationData = SquareObstacleSystemInitializationData;

            this.FaceFrustums = new List<FrustumV2>();
            this.SquareObstacleChangeTracker = new SquareObstacleChangeTracker(this);

            //Create frustum for all sides -> occlusions are only calculated for facing frustums.
            this.CreateAndAddFrustum(Quaternion.Euler(0, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 180, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, -90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(90, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(-90, 0, 0), 1);

            PuzzleGameSingletonInstances.SquareObstaclesManager.AddSquareObstacleSystem(this);
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
            if (this.SquareObstacleSystemInitializationData.IsStatic)
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
            frustum.DeltaRotation = deltaRotation;
            frustum.F1.FaceOffsetFromCenter.z = F1FaceZOffset;
            this.FaceFrustums.Add(frustum);
        }

        public List<FrustumPointsPositions> ComputeOcclusionFrustums_FromDedicatedThread(SquareObstacleFrustumCalculationResult obstacleCalucation)
        {
            Profiler.BeginThreadProfiling("ObstacleOcclusions", "ObstacleOcclusions");
            Profiler.BeginSample("ObstacleOcclusions");

            List<FrustumPointsPositions> frustumPointsWorldPositions = new List<FrustumPointsPositions>();


            foreach (var faceFrustum in this.FaceFrustums)
            {
                faceFrustum.SetCalculationDataForPointProjection(obstacleCalucation.WorldPositionStartAngleDefinition, obstacleCalucation.ObstaclePosition, obstacleCalucation.ObstacleRotation, obstacleCalucation.ObstacleLossyScale);
                var FrustumPointsPositions = faceFrustum.FrustumPointsPositions;
                Vector3 frontFaceNormal = Vector3.Cross(FrustumPointsPositions.FC2 - FrustumPointsPositions.FC1, FrustumPointsPositions.FC4 - FrustumPointsPositions.FC1).normalized;

                //We filter faceFrustums that are not facing the initial calculation point -> they are useless because always occluded by front faces
                if (Vector3.Dot(frontFaceNormal, FrustumPointsPositions.FC1 - obstacleCalucation.WorldPositionStartAngleDefinition) < 0)
                {
                    continue;
                }

                frustumPointsWorldPositions.Add(FrustumPointsPositions);

            }

            Profiler.EndSample();
            Profiler.EndThreadProfiling();
            return frustumPointsWorldPositions;

        }

    }

    class SquareObstacleChangeTracker
    {
        private SquareObstacleSystem SquareObstacleRef;

        public SquareObstacleChangeTracker(SquareObstacleSystem squareObstacleRef)
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

}
