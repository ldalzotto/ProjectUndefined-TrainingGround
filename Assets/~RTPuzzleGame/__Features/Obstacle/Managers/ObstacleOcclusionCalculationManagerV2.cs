using CoreGame;
using System.Collections.Generic;
//using CoreGame;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
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
        private ObstaclesListenerManager ObstaclesListenerManager = ObstaclesListenerManager.Get();
        private SquareObstacleSystemManager SquareObstacleSystemManager = SquareObstacleSystemManager.Get();
        #endregion

        private Dictionary<ObstacleListener, TransformStruct> ObstacleListenerLastFramePositions = new Dictionary<ObstacleListener, TransformStruct>();

        private List<ObstacleListener> obstacleListenersThatHasChangedThisFrame = new List<ObstacleListener>();

        //ObstacleListener -> SquareObstacleSystem -> FrustumPositions
        private Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>> CalculatedFrustums = new Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>>();

        public void Tick(float d)
        {
            Profiler.BeginSample("ObstacleOcclusionCalculationManagerV2");


            var allObstaclesListeners = this.ObstaclesListenerManager.GetAllObstacleListeners();

            int nearObstaclesCounter = 0;
            int totalFrustumCounter = 0;

            //Position change detection
            foreach (var obstacleListener in allObstaclesListeners)
            {
                this.ObstacleListenerLastFramePositions.TryGetValue(obstacleListener, out TransformStruct lastFramePosition);
                bool hasChanged = !obstacleListener.AssociatedRangeObject.GetTransform().IsEqualTo(lastFramePosition);
                if (hasChanged)
                {
                    this.obstacleListenersThatHasChangedThisFrame.Add(obstacleListener);
                    nearObstaclesCounter += obstacleListener.NearSquareObstacles.Count;


                    this.CalculatedFrustums.TryGetValue(obstacleListener.ObstacleListenerUniqueID, out Dictionary<int, List<FrustumPointsPositions>> obstalceFrustumPointsPositions);
                    if (obstalceFrustumPointsPositions == null)
                    {
                        this.CalculatedFrustums.Add(obstacleListener.ObstacleListenerUniqueID, new Dictionary<int, List<FrustumPointsPositions>>());
                        obstalceFrustumPointsPositions = this.CalculatedFrustums[obstacleListener.ObstacleListenerUniqueID];
                    }

                    foreach (var squareObstacle in obstacleListener.NearSquareObstacles)
                    {
                        totalFrustumCounter += squareObstacle.FaceFrustums.Count;

                        obstalceFrustumPointsPositions.TryGetValue(squareObstacle.SquareObstacleSystemUniqueID, out List<FrustumPointsPositions> squareObstacleFrustumPositions);
                        if (squareObstacleFrustumPositions == null)
                        {
                            obstalceFrustumPointsPositions.Add(squareObstacle.SquareObstacleSystemUniqueID, new List<FrustumPointsPositions>());
                            squareObstacleFrustumPositions = obstalceFrustumPointsPositions[squareObstacle.SquareObstacleSystemUniqueID];
                        }
                        else
                        {
                            squareObstacleFrustumPositions.Clear();
                        }
                    }
                }
                else
                {
                    this.obstacleListenersThatHasChangedThisFrame.Remove(obstacleListener);
                }
                this.ObstacleListenerLastFramePositions[obstacleListener] = obstacleListener.AssociatedRangeObject.GetTransform();
            }

            if (nearObstaclesCounter > 0)
            {

                NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas = new NativeArray<FrustumOcclusionCalculationData>(nearObstaclesCounter, Allocator.TempJob);
                NativeArray<FrustumV2Indexed> AssociatedFrustums = new NativeArray<FrustumV2Indexed>(totalFrustumCounter, Allocator.TempJob);
                NativeArray<FrustumPointsWithInitializedFlag> Results = new NativeArray<FrustumPointsWithInitializedFlag>(totalFrustumCounter, Allocator.TempJob);

                int currentNearObstacleCounter = 0;
                int currentFrustumCounter = 0;


                foreach (var obstacleListenerThatChanged in obstacleListenersThatHasChangedThisFrame)
                {
                    foreach (var nearSquareObstacle in obstacleListenerThatChanged.NearSquareObstacles)
                    {
                        int beginFrustumIndex = currentFrustumCounter;
                        foreach (var nearSquaureObstacleFrustum in nearSquareObstacle.FaceFrustums)
                        {
                            AssociatedFrustums[currentFrustumCounter] = new FrustumV2Indexed
                            {
                                FrustumV2 = nearSquaureObstacleFrustum,
                                CalculationDataIndex = currentNearObstacleCounter
                            };
                            currentFrustumCounter += 1;
                        }

                        FrustumOcclusionCalculationDatas[currentNearObstacleCounter] = new FrustumOcclusionCalculationData
                        {
                            FrustumCalculationDataID = new FrustumCalculationDataID
                            {
                                ObstacleListenerUniqueID = obstacleListenerThatChanged.ObstacleListenerUniqueID,
                                SquareObstacleSystemUniqueID = nearSquareObstacle.SquareObstacleSystemUniqueID,
                            },
                            ObstacleListenerTransform = obstacleListenerThatChanged.AssociatedRangeObject.GetTransform(),
                            SquareObstacleTransform = nearSquareObstacle.GetTransform()
                        };

                        currentNearObstacleCounter += 1;
                    }
                }

                new FrustumOcclusionCalculationJob()
                {
                    AssociatedFrustums = AssociatedFrustums,
                    FrustumOcclusionCalculationDatas = FrustumOcclusionCalculationDatas,
                    Results = Results
                }.Schedule(SquareObstacleSystemManager.TotalFrustumCounter, 36).Complete();

                //Store results
                foreach (var result in Results)
                {
                    if (result.Isinitialized)
                    {
                        this.CalculatedFrustums[result.FrustumCalculationDataID.ObstacleListenerUniqueID][result.FrustumCalculationDataID.SquareObstacleSystemUniqueID].Add(result.FrustumPointsPositions);
                    }
                }

                FrustumOcclusionCalculationDatas.Dispose();
                AssociatedFrustums.Dispose();
                Results.Dispose();

            }

            Profiler.EndSample();
        }

        public void OnDestroy()
        {
            Instance = null;
        }

    }

    [BurstCompile]
    public struct FrustumOcclusionCalculationJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas;

        public NativeArray<FrustumV2Indexed> AssociatedFrustums;
        public NativeArray<FrustumPointsWithInitializedFlag> Results;

        public void Execute(int frustumIndex)
        {
            var FrustumV2Indexed = this.AssociatedFrustums[frustumIndex];
            var FrustumOcclusionCalculationData = this.FrustumOcclusionCalculationDatas[FrustumV2Indexed.CalculationDataIndex];
            FrustumV2Indexed.FrustumV2.CalculateFrustumPointsWorldPosByProjection(out FrustumPointsPositions FrustumPointsPositions, out bool IsFacing, FrustumOcclusionCalculationData.SquareObstacleTransform, FrustumOcclusionCalculationData.ObstacleListenerTransform.WorldPosition);

            if (IsFacing)
            {
                this.Results[frustumIndex] = new FrustumPointsWithInitializedFlag
                {
                    Isinitialized = true,
                    FrustumCalculationDataID = FrustumOcclusionCalculationData.FrustumCalculationDataID,
                    FrustumPointsPositions = FrustumPointsPositions
                };
            }
        }
    }

    public struct FrustumV2Indexed
    {
        public FrustumV2 FrustumV2;
        public int CalculationDataIndex;
    }

    public struct FrustumCalculationDataID
    {
        public int ObstacleListenerUniqueID;
        public int SquareObstacleSystemUniqueID;

        public override bool Equals(object obj)
        {
            if (!(obj is FrustumCalculationDataID))
            {
                return false;
            }

            var iD = (FrustumCalculationDataID)obj;
            return ObstacleListenerUniqueID == iD.ObstacleListenerUniqueID &&
                   SquareObstacleSystemUniqueID == iD.SquareObstacleSystemUniqueID;
        }

        public override int GetHashCode()
        {
            var hashCode = 880951818;
            hashCode = hashCode * -1521134295 + ObstacleListenerUniqueID.GetHashCode();
            hashCode = hashCode * -1521134295 + SquareObstacleSystemUniqueID.GetHashCode();
            return hashCode;
        }
    }

    public struct FrustumOcclusionCalculationData
    {
        public FrustumCalculationDataID FrustumCalculationDataID;
        public TransformStruct ObstacleListenerTransform;
        public TransformStruct SquareObstacleTransform;
    }

    public struct FrustumPointsWithInitializedFlag
    {
        public bool Isinitialized;
        public FrustumCalculationDataID FrustumCalculationDataID;
        public FrustumPointsPositions FrustumPointsPositions;
    }


}

