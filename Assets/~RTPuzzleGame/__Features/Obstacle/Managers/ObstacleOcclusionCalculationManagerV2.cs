using CoreGame;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public class ObstacleOcclusionCalculationManagerV2
    {
        private static ObstacleOcclusionCalculationManagerV2 Instance;

        public static ObstacleOcclusionCalculationManagerV2 Get()
        {
            if (Instance == null) { Instance = new ObstacleOcclusionCalculationManagerV2(); }
            return Instance;
        }

        #region External Dependencies
        private ObstaclesListenerManager ObstaclesListenerManager;
        private ObstacleFrustumCalculationManager ObstacleFrustumCalculationManager;
        #endregion

        public ObstacleOcclusionCalculationManagerV2()
        {
            this.ObstaclesListenerManager = PuzzleGameSingletonInstances.ObstaclesListenerManager;
            this.ObstacleFrustumCalculationManager = PuzzleGameSingletonInstances.ObstacleFrustumCalculationManager;
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("ObstacleOcclusionCalculationManagerV2");

            var allObstaclesListeners = this.ObstaclesListenerManager.GetAllObstacleListeners();

            int maxCalculationNumberCount = 0;
            var obstacleListenerCount = allObstaclesListeners.Count;

            if (obstacleListenerCount > 0)
            {
                NativeArray<ObstacleListenerCalculationData> ObstacleListenerCalculationData = new NativeArray<ObstacleListenerCalculationData>(obstacleListenerCount, Allocator.Temp);

                int SquareObstacleCount = 0;
                int SquareObstacleFrustumCount = 0;

                for (var obstacleListenerIndex = 0; obstacleListenerIndex < obstacleListenerCount; obstacleListenerIndex++)
                {
                    int beginSquareObstacleIndex = SquareObstacleCount;
                    int endSquareObstacleIndex = SquareObstacleCount + allObstaclesListeners[obstacleListenerIndex].NearSquareObstacles.Count;

                    var NearestObstacles = allObstaclesListeners[obstacleListenerIndex].NearSquareObstacles;
                    SquareObstacleCount += NearestObstacles.Count;

                    ObstacleListenerCalculationData[obstacleListenerIndex] = new ObstacleListenerCalculationData
                    {
                        ObstacleListenerTransformChange = new ObstacleListenerTransformChange
                        {
                            LastFrameTransform = allObstaclesListeners[obstacleListenerIndex].LastFrameTransform,
                            CurrentFrameTransform = allObstaclesListeners[obstacleListenerIndex].AssociatedRangeObject.GetTransform()
                        },
                        SquareObstacleBeginIndex = beginSquareObstacleIndex,
                        SquareObstacleEndIndex = endSquareObstacleIndex
                    };

                    //Frustums
                    for (var squareObstacleIndex = 0; squareObstacleIndex < NearestObstacles.Count; squareObstacleIndex++)
                    {
                        SquareObstacleFrustumCount = SquareObstacleFrustumCount + NearestObstacles[squareObstacleIndex].FaceFrustums.Count;
                    }
                }

                NativeArray<SquareObstacleCalculationData> SquareObstacleCalculationDatas = new NativeArray<SquareObstacleCalculationData>(SquareObstacleCount, Allocator.Temp);
                NativeArray<FrustumV2Struct> SquareObstacleFrustums = new NativeArray<FrustumV2Struct>(SquareObstacleFrustumCount, Allocator.Temp);
                NativeArray<FrustumPointsPositionsResult> FrustumPointsPositionsResults = new NativeArray<FrustumPointsPositionsResult>(SquareObstacleFrustumCount, Allocator.Temp);

                int currentSquareObstacleIndex = 0;
                int currentSquareObstacleFrustumIndex = 0;

                for (var obstacleListenerIndex = 0; obstacleListenerIndex < obstacleListenerCount; obstacleListenerIndex++)
                {
                    var NearestObstacles = allObstaclesListeners[obstacleListenerIndex].NearSquareObstacles;
                    for (var squareObstacleIndex = 0; squareObstacleIndex < NearestObstacles.Count; squareObstacleIndex++)
                    {
                        var CurrentObstacle = NearestObstacles[squareObstacleIndex];
                        var ObstacleListenerProjectionWorldPos = allObstaclesListeners[obstacleListenerIndex].AssociatedRangeObject.GetTransform().WorldPosition;
                        SquareObstacleCalculationDatas[currentSquareObstacleIndex] = new SquareObstacleCalculationData
                        {
                            IsStatic = CurrentObstacle.SquareObstacleSystemInitializationData.IsStatic,
                            SquareObstacleTransformChange = new SquareObstacleTransformChange
                            {
                                LastFrameTransform = CurrentObstacle.LastFrameTransform,
                                CurrentFrameTransform = CurrentObstacle.GetTransform()
                            },
                            ObstacleListenerProjectionWorldPos = ObstacleListenerProjectionWorldPos,
                            SquareObstacleFrustumBeginIndex = currentSquareObstacleFrustumIndex,
                            SquareObstacleFrustumEndIndex = currentSquareObstacleFrustumIndex + CurrentObstacle.FaceFrustums.Count
                        };
                        maxCalculationNumberCount += 1;
                        currentSquareObstacleIndex += 1;


                        for (var squareObstacleFrustumIndex = 0; squareObstacleFrustumIndex < CurrentObstacle.FaceFrustums.Count; squareObstacleFrustumIndex++)
                        {
                            SquareObstacleFrustums[currentSquareObstacleFrustumIndex] = FrustumV2Struct.FromFrustumV2PointProjection(CurrentObstacle.GetTransform(),
                                ObstacleListenerProjectionWorldPos, CurrentObstacle.FaceFrustums[squareObstacleFrustumIndex]);
                            currentSquareObstacleFrustumIndex += 1;
                        }
                    }
                }

                NativeArray<RequestedCalculationIndex> RequestedCalculationIndexes = new NativeArray<RequestedCalculationIndex>(maxCalculationNumberCount, Allocator.Temp);

                var ObstacleOcclusionTask = new ObstacleOcclusionTask
                {
                    ObstacleListenerCalculationDatas = ObstacleListenerCalculationData,
                    SquareObstacleCalculationDatas = SquareObstacleCalculationDatas,
                    RequestedCalculationIndexes = RequestedCalculationIndexes,
                    SquareObstacleFrustums = SquareObstacleFrustums,
                    FrustumPointsPositionsResults = FrustumPointsPositionsResults
                };

                ObstacleOcclusionTask.DoParallel();

                //Update Systems Data
                currentSquareObstacleIndex = 0;
                for (var obstacleListenerIndex = 0; obstacleListenerIndex < obstacleListenerCount; obstacleListenerIndex++)
                {
                    allObstaclesListeners[obstacleListenerIndex].LastFrameTransform = ObstacleListenerCalculationData[obstacleListenerIndex].ObstacleListenerTransformChange.CurrentFrameTransform;
                    var NearestObstacles = allObstaclesListeners[obstacleListenerIndex].NearSquareObstacles;
                    for (var squareObstacleIndex = 0; squareObstacleIndex < NearestObstacles.Count; squareObstacleIndex++)
                    {
                        NearestObstacles[squareObstacleIndex].LastFrameTransform = SquareObstacleCalculationDatas[currentSquareObstacleIndex].SquareObstacleTransformChange.CurrentFrameTransform;
                        currentSquareObstacleIndex++;
                    }
                }

                for (var RequestedCalculationIndex = 0; RequestedCalculationIndex < RequestedCalculationIndexes.Length; RequestedCalculationIndex++)
                {
                    var obstacleListener = allObstaclesListeners[RequestedCalculationIndexes[RequestedCalculationIndex].ObstacleListenerIndex];
                    var squareObstacleLocalIndex = RequestedCalculationIndexes[RequestedCalculationIndex].SquareObstacleIndex - ObstacleListenerCalculationData[RequestedCalculationIndexes[RequestedCalculationIndex].ObstacleListenerIndex].SquareObstacleBeginIndex;

                    List<FrustumPointsPositions> AggregatedFrustumResults = new List<FrustumPointsPositions>();

                    for (var resultFrustumPositionIndex = RequestedCalculationIndexes[RequestedCalculationIndex].FrustumPointsPositionsResultsBeginIndex;
                        resultFrustumPositionIndex < RequestedCalculationIndexes[RequestedCalculationIndex].FrustumPointsPositionsResultsEndIndex; resultFrustumPositionIndex++)
                    {
                        if (!FrustumPointsPositionsResults[resultFrustumPositionIndex].IsIgnored)
                        {
                            AggregatedFrustumResults.Add(FrustumPointsPositionsResults[resultFrustumPositionIndex].FrustumPointsPositions);
                        }
                    }

                    this.ObstacleFrustumCalculationManager.CalculationResults[obstacleListener][obstacleListener.NearSquareObstacles[squareObstacleLocalIndex]]
                         = new SquareObstacleFrustumCalculationResult(AggregatedFrustumResults);
                }

                ObstacleListenerCalculationData.Dispose();
                SquareObstacleCalculationDatas.Dispose();
                RequestedCalculationIndexes.Dispose();
                SquareObstacleFrustums.Dispose();
                FrustumPointsPositionsResults.Dispose();

                Profiler.EndSample();
            }
        }

        public void OnDestroy()
        {
            Instance = null;
        }

    }
}

