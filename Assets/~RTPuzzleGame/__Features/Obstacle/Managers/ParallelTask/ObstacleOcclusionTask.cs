using CoreGame;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace RTPuzzle
{
    public struct ObstacleOcclusionTask
    {
        public NativeArray<ObstacleListenerCalculationData> ObstacleListenerCalculationDatas;
        public NativeArray<SquareObstacleCalculationData> SquareObstacleCalculationDatas;
        public NativeArray<FrustumV2Struct> SquareObstacleFrustums;

        public NativeArray<RequestedCalculationIndex> RequestedCalculationIndexes;
        public NativeArray<FrustumPointsPositionsResult> FrustumPointsPositionsResults;

        public void DoParallel()
        {
            var ObstacleOcclusionTask = this;
            int currentRequestedCalculationIndex = 0;
            Parallel.For(0, this.ObstacleListenerCalculationDatas.Length,
                (ObstacleListenerTransformChangeIndex) =>
                {
                    var ObstacleListenerCalculationData = ObstacleOcclusionTask.ObstacleListenerCalculationDatas[ObstacleListenerTransformChangeIndex];
                    var ObstacleListenerTransformChange = ObstacleListenerCalculationData.ObstacleListenerTransformChange;
                    bool hasObstacleListernerChanged = ObstacleOcclusionTaskMethods.HasTrasformChanged(ObstacleListenerTransformChange.LastFrameTransform, ObstacleListenerTransformChange.CurrentFrameTransform);

                    for (var SquareObstacleForCalculationIndex = ObstacleListenerCalculationData.SquareObstacleBeginIndex; SquareObstacleForCalculationIndex < ObstacleListenerCalculationData.SquareObstacleEndIndex; SquareObstacleForCalculationIndex++)
                    {
                        var requestedCalculationIndex = currentRequestedCalculationIndex;
                        currentRequestedCalculationIndex += 1;
                        ObstacleOcclusionTask.RequestedCalculationIndexes[requestedCalculationIndex] = new RequestedCalculationIndex
                        {
                            ObstacleListenerIndex = ObstacleListenerTransformChangeIndex,
                            SquareObstacleIndex = SquareObstacleForCalculationIndex,
                            FrustumPointsPositionsResultsBeginIndex = ObstacleOcclusionTask.SquareObstacleCalculationDatas[SquareObstacleForCalculationIndex].SquareObstacleFrustumBeginIndex,
                            FrustumPointsPositionsResultsEndIndex = ObstacleOcclusionTask.SquareObstacleCalculationDatas[SquareObstacleForCalculationIndex].SquareObstacleFrustumEndIndex,
                        };
                    }

                    //For now, we only do One directional detection
                    /*
                    Parallel.For(ObstacleListenerCalculationData.SquareObstacleBeginIndex, ObstacleListenerCalculationData.SquareObstacleEndIndex,
                        (SquareObstacleIndex) =>
                        {
                            var SquareObstacleCalculationData = ObstacleOcclusionTask.SquareObstacleCalculationDatas[SquareObstacleIndex];

                            if (!SquareObstacleCalculationData.IsStatic)
                            {
                                var SquareObstacleTransformChange = SquareObstacleCalculationData.SquareObstacleTransformChange;
                                bool hasSquareObstacleChanged = ObstacleOcclusionTaskMethods.HasTrasformChanged(SquareObstacleTransformChange.LastFrameTransform, SquareObstacleTransformChange.CurrentFrameTransform);
                            }

                        });
                    */
                });

            //Do Calculation
            Parallel.For(0, this.RequestedCalculationIndexes.Length,
                (RequestedCalculationIndex) =>
                {
                    var requestedCalculation = ObstacleOcclusionTask.RequestedCalculationIndexes[RequestedCalculationIndex];
                    var squareObstacleForCalculation = ObstacleOcclusionTask.SquareObstacleCalculationDatas[requestedCalculation.SquareObstacleIndex];

                    int currentFrustumPointsPositionsResultIndex = requestedCalculation.FrustumPointsPositionsResultsBeginIndex;

                    Parallel.For(squareObstacleForCalculation.SquareObstacleFrustumBeginIndex, squareObstacleForCalculation.SquareObstacleFrustumEndIndex,
                        (squareObstacleOcclusionFrustumIndex) =>
                        {
                            ObstacleOcclusionTask.SquareObstacleFrustums[squareObstacleOcclusionFrustumIndex].CalculateFrustumPointsWorldPosByProjection(out FrustumPointsPositions FrustumPointsPositions, out bool isFacing);
                            ObstacleOcclusionTask.FrustumPointsPositionsResults[currentFrustumPointsPositionsResultIndex] = new FrustumPointsPositionsResult
                            {
                                FrustumPointsPositions = FrustumPointsPositions,
                                IsIgnored = !isFacing
                            };
                            currentFrustumPointsPositionsResultIndex++;
                        });
                });
        }
    }

    public struct ObstacleListenerCalculationData
    {
        public ObstacleListenerTransformChange ObstacleListenerTransformChange;
        public int SquareObstacleBeginIndex;
        public int SquareObstacleEndIndex;
    }

    public struct ObstacleListenerTransformChange
    {
        public TransformStruct LastFrameTransform;
        public TransformStruct CurrentFrameTransform;
    }

    public struct SquareObstacleCalculationData
    {
        public bool IsStatic;
        public SquareObstacleTransformChange SquareObstacleTransformChange;
        public Vector3 ObstacleListenerProjectionWorldPos;
        public int SquareObstacleFrustumBeginIndex;
        public int SquareObstacleFrustumEndIndex;
    }

    public struct SquareObstacleTransformChange
    {
        public TransformStruct LastFrameTransform;
        public TransformStruct CurrentFrameTransform;
    }

    public struct RequestedCalculationIndex
    {
        public int ObstacleListenerIndex;
        public int SquareObstacleIndex;
        public int FrustumPointsPositionsResultsBeginIndex;
        public int FrustumPointsPositionsResultsEndIndex;
    }

    public struct FrustumPointsPositionsResult
    {
        public bool IsIgnored;
        public FrustumPointsPositions FrustumPointsPositions;
    }

    public static class ObstacleOcclusionTaskMethods
    {
        public static bool HasTrasformChanged(TransformStruct LastFrameTransform, TransformStruct CurrentFrameTransform)
        {
            return (LastFrameTransform.WorldPosition != CurrentFrameTransform.WorldPosition)
                || (LastFrameTransform.WorldRotation != CurrentFrameTransform.WorldRotation)
                || (LastFrameTransform.LossyScale != CurrentFrameTransform.LossyScale);
        }
    }
}
