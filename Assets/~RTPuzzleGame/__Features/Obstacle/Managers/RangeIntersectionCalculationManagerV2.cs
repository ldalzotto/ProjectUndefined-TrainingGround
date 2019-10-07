using CoreGame;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeIntersectionCalculationManagerV2
    {
        private static RangeIntersectionCalculationManagerV2 Instance;
        public static RangeIntersectionCalculationManagerV2 Get()
        {
            if (Instance == null)
            {
                Instance = new RangeIntersectionCalculationManagerV2();
            }
            return Instance;
        }

        #region External Dependencies
        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
        private RangeIntersectionCalculatorV2Manager RangeIntersectionCalculatorV2Manager = RangeIntersectionCalculatorV2Manager.Get();
        #endregion

        #region Job State
        private JobHandle JobHandle;
        #endregion

        private NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
        private NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;
        private NativeArray<bool> IsOccludedByObstacleJobResult;

        public void Tick(float d)
        {
            int AllRangeIntersectionCalculatorV2Count = RangeIntersectionCalculatorV2Manager.AllRangeIntersectionCalculatorV2.Count;
            if (AllRangeIntersectionCalculatorV2Count > 0)
            {
                this.IsOccludedByObstacleJobData = new NativeArray<IsOccludedByObstacleJobData>(AllRangeIntersectionCalculatorV2Count, Allocator.TempJob);
                this.IsOccludedByObstacleJobResult = new NativeArray<bool>(AllRangeIntersectionCalculatorV2Count, Allocator.TempJob);

                int totalObstacleFrustumPointsCounter = 0;
                foreach (var rangeIntersectionCalculatorV2 in RangeIntersectionCalculatorV2Manager.AllRangeIntersectionCalculatorV2)
                {
                    var associatedObstacleListener = rangeIntersectionCalculatorV2.GetAssociatedObstacleListener();
                    foreach (var calculatedObstacleFrustum in this.ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener).Values)
                    {
                        totalObstacleFrustumPointsCounter += calculatedObstacleFrustum.Count;
                    }
                }

                this.AssociatedObstacleFrustumPointsPositions = new NativeArray<FrustumPointsPositions>(totalObstacleFrustumPointsCounter, Allocator.TempJob);

                int currentIsOccludedByObstacleJobDataCounter = 0;
                int currentObstacleFrustumPointsCounter = 0;

                foreach (var rangeIntersectionCalculatorV2 in RangeIntersectionCalculatorV2Manager.AllRangeIntersectionCalculatorV2)
                {
                    int ObstacleFrustumPointsPositionsBeginIndex = currentObstacleFrustumPointsCounter;
                    var associatedObstacleListener = rangeIntersectionCalculatorV2.GetAssociatedObstacleListener();

                    foreach (var calculatedObstacleFrustumList in this.ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener).Values)
                    {
                        foreach (var calculatedObstacleFrustum in calculatedObstacleFrustumList)
                        {
                            this.AssociatedObstacleFrustumPointsPositions[currentObstacleFrustumPointsCounter] = calculatedObstacleFrustum;
                            currentObstacleFrustumPointsCounter += 1;
                        }
                    }

                    this.IsOccludedByObstacleJobData[currentIsOccludedByObstacleJobDataCounter] = new IsOccludedByObstacleJobData
                    {
                        TestedBoxCollider = new BoxDefinition
                        {
                            LocalCenter = rangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.LogicCollider.center,
                            LocalSize = rangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.LogicCollider.size,
                            LocalToWorld = rangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.GetLocalToWorld()
                        },
                        ObstacleFrustumPointsPositionsBeginIndex = ObstacleFrustumPointsPositionsBeginIndex,
                        ObstacleFrustumPointsPositionsEndIndex = currentObstacleFrustumPointsCounter
                    };
                    currentIsOccludedByObstacleJobDataCounter += 1;
                }

                this.JobHandle = new ObstacleOcclusionJob
                {
                    IsOccludedByObstacleJobData = this.IsOccludedByObstacleJobData,
                    AssociatedObstacleFrustumPointsPositions = this.AssociatedObstacleFrustumPointsPositions,
                    IsOccludedByObstacleJobResult = this.IsOccludedByObstacleJobResult
                }.Schedule(AllRangeIntersectionCalculatorV2Count, 10);

                this.JobHandle.Complete();

              //  Debug.Log(this.IsOccludedByObstacleJobResult[0]);

                this.IsOccludedByObstacleJobData.Dispose();
                this.AssociatedObstacleFrustumPointsPositions.Dispose();
                this.IsOccludedByObstacleJobResult.Dispose();
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }

    [BurstCompile]
    public struct ObstacleOcclusionJob : IJobParallelFor
    {
        public NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
        public NativeArray<bool> IsOccludedByObstacleJobResult;
        [ReadOnly]
        public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;

        public void Execute(int IsOccludedByObstacleJobDataIndex)
        {
            var IsOccludedByObstacleJobData = this.IsOccludedByObstacleJobData[IsOccludedByObstacleJobDataIndex];

            Intersection.ExtractBoxColliderWorldPointsV2(IsOccludedByObstacleJobData.TestedBoxCollider.LocalCenter, IsOccludedByObstacleJobData.TestedBoxCollider.LocalSize, IsOccludedByObstacleJobData.TestedBoxCollider.LocalToWorld,
                out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

            bool isOccluded =
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC1) &&
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC2) &&
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC3) &&
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC4) &&
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC5) &&
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC6) &&
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC7) &&
                IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData, BC8);

            this.IsOccludedByObstacleJobResult[IsOccludedByObstacleJobDataIndex] = isOccluded;
        }

        private bool IsPointFullyOccludedByObstacle(IsOccludedByObstacleJobData IsOccludedByObstacleJobData, Vector3 PointWorldPosition)
        {
            bool isOccluded = true;

            for (var FrustumPointsPositionsIndex = IsOccludedByObstacleJobData.ObstacleFrustumPointsPositionsBeginIndex; FrustumPointsPositionsIndex < IsOccludedByObstacleJobData.ObstacleFrustumPointsPositionsEndIndex; FrustumPointsPositionsIndex++)
            {
                isOccluded = isOccluded && Intersection.PointInsideFrustum(AssociatedObstacleFrustumPointsPositions[FrustumPointsPositionsIndex], PointWorldPosition);
                if (!isOccluded)
                {
                    break;
                }
            }

            return isOccluded;
        }
    }


    public struct IsOccludedByObstacleJobData
    {
        public BoxDefinition TestedBoxCollider;
        public int ObstacleFrustumPointsPositionsBeginIndex;
        public int ObstacleFrustumPointsPositionsEndIndex;
    }
}
