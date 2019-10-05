
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
        public SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData { get; private set; }
        public List<FrustumV2> FaceFrustums { get; private set; }

        #region Internal Dependencies
        private ObstacleInteractiveObject AssociatedInteractiveObject;
        #endregion

        #region Obstacle Calculation Data
        public TransformStruct LastFrameTransform;
        #endregion

        #region Data Retrieval
        public TransformStruct GetTransform()
        {
            return new TransformStruct
            {
                WorldPosition = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldPosition +
                          this.AssociatedInteractiveObject.ObstacleCollider.center,
                WorldRotation = this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().WorldRotation,
                LossyScale = (this.AssociatedInteractiveObject.ObstacleCollider != null ? this.AssociatedInteractiveObject.ObstacleCollider.size : Vector3.one).Mul(this.AssociatedInteractiveObject.InteractiveGameObject.GetTransform().LossyScale)
            };
        }
        #endregion

        public SquareObstacleSystem(ObstacleInteractiveObject AssociatedInteractiveObject, SquareObstacleSystemInitializationData SquareObstacleSystemInitializationData)
        {
            this.AssociatedInteractiveObject = AssociatedInteractiveObject;
            this.SquareObstacleSystemInitializationData = SquareObstacleSystemInitializationData;

            this.FaceFrustums = new List<FrustumV2>();

            //Create frustum for all sides -> occlusions are only calculated for facing frustums.
            this.CreateAndAddFrustum(Quaternion.Euler(0, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 180, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, 90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(0, -90, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(90, 0, 0), 1);
            this.CreateAndAddFrustum(Quaternion.Euler(-90, 0, 0), 1);
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

}
