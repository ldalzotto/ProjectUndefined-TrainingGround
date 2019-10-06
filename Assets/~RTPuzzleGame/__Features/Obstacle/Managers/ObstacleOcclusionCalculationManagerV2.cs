using CoreGame;
using System.Collections.Generic;
//using CoreGame;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
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
        private Dictionary<SquareObstacleSystem, TransformStruct> SquareObstacleLastFramePositions = new Dictionary<SquareObstacleSystem, TransformStruct>();

        private List<ObstacleListener> obstacleListenersThatHasChangedThisFrame = new List<ObstacleListener>();
        private Dictionary<ObstacleListener, List<SquareObstacleSystem>> singleObstacleSystemsThatHasChangedThisFrame = new Dictionary<ObstacleListener, List<SquareObstacleSystem>>();

        //ObstacleListener -> SquareObstacleSystem -> FrustumPositions
        private Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>> CalculatedFrustums = new Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>>();

        public void Tick(float d)
        {
            Profiler.BeginSample("ObstacleOcclusionCalculationManagerV2");


            var allObstaclesListeners = this.ObstaclesListenerManager.GetAllObstacleListeners();

            int occlusionCalculationCounter = 0;
            int totalFrustumCounter = 0;

            //Position change detection
            foreach (var obstacleListener in allObstaclesListeners)
            {
                this.ObstacleListenerLastFramePositions.TryGetValue(obstacleListener, out TransformStruct lastFramePosition);
                bool hasChanged = !obstacleListener.AssociatedRangeObject.GetTransform().IsEqualTo(lastFramePosition);

                //The obstacle listener has changed -> all associated near square obstacles are updated
                if (hasChanged)
                {
                    this.obstacleListenersThatHasChangedThisFrame.Add(obstacleListener);

                    foreach (var squareObstacle in obstacleListener.NearSquareObstacles)
                    {
                        occlusionCalculationCounter += 1;
                        totalFrustumCounter += squareObstacle.FaceFrustums.Count;
                        this.ClearAndCreateCalculatedFrustums(obstacleListener, squareObstacle);
                    }
                }

                //The obstacle listener hasn't changed -> we compate near square obstacles positions for update
                else
                {
                    this.singleObstacleSystemsThatHasChangedThisFrame.TryGetValue(obstacleListener, out List<SquareObstacleSystem> squareObstacleSystemsThatChanged);
                    if (squareObstacleSystemsThatChanged == null)
                    {
                        this.singleObstacleSystemsThatHasChangedThisFrame.Add(obstacleListener, new List<SquareObstacleSystem>());
                        squareObstacleSystemsThatChanged = this.singleObstacleSystemsThatHasChangedThisFrame[obstacleListener];
                    }


                    foreach (var squareObstacle in obstacleListener.NearSquareObstacles)
                    {
                        this.SquareObstacleLastFramePositions.TryGetValue(squareObstacle, out TransformStruct lastFrameSquareObstacleTrasform);
                        if (!squareObstacle.GetTransform().IsEqualTo(lastFrameSquareObstacleTrasform))
                        {
                            //We add this single couple (listener <-> obstacle) to calculation
                            squareObstacleSystemsThatChanged.Add(squareObstacle);
                            occlusionCalculationCounter += 1;
                            totalFrustumCounter += squareObstacle.FaceFrustums.Count;
                            this.ClearAndCreateCalculatedFrustums(obstacleListener, squareObstacle);
                        }
                    }
                }

                //Update Obstacle Listener Positions
                this.ObstacleListenerLastFramePositions[obstacleListener] = obstacleListener.AssociatedRangeObject.GetTransform();
            }

            //Update Square Obstacle Positions
            foreach (var squareObstacleSystem in this.SquareObstacleSystemManager.AllSquareObstacleSystems)
            {
                this.SquareObstacleLastFramePositions[squareObstacleSystem] = squareObstacleSystem.GetTransform();
            }

            if (occlusionCalculationCounter > 0)
            {

                NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas = new NativeArray<FrustumOcclusionCalculationData>(occlusionCalculationCounter, Allocator.TempJob);
                NativeArray<FrustumV2Indexed> AssociatedFrustums = new NativeArray<FrustumV2Indexed>(totalFrustumCounter, Allocator.TempJob);
                NativeArray<FrustumPointsWithInitializedFlag> Results = new NativeArray<FrustumPointsWithInitializedFlag>(totalFrustumCounter, Allocator.TempJob);

                int currentOcclusionCalculationCounter = 0;
                int currentFrustumCounter = 0;

                foreach (var obstacleListenerThatChanged in this.obstacleListenersThatHasChangedThisFrame)
                {
                    foreach (var nearSquareObstacle in obstacleListenerThatChanged.NearSquareObstacles)
                    {
                        AddToArrays(ref FrustumOcclusionCalculationDatas, AssociatedFrustums, ref currentOcclusionCalculationCounter, ref currentFrustumCounter, obstacleListenerThatChanged, nearSquareObstacle);
                    }
                }

                foreach (var singleObstacleSystemThatChanged in this.singleObstacleSystemsThatHasChangedThisFrame)
                {
                    if (singleObstacleSystemThatChanged.Value.Count > 0)
                    {
                        foreach (var nearSquareObstacle in singleObstacleSystemThatChanged.Value)
                        {
                            AddToArrays(ref FrustumOcclusionCalculationDatas, AssociatedFrustums, ref currentOcclusionCalculationCounter, ref currentFrustumCounter, singleObstacleSystemThatChanged.Key, nearSquareObstacle);
                        }
                    }
                }

                new FrustumOcclusionCalculationJob()
                {
                    AssociatedFrustums = AssociatedFrustums,
                    FrustumOcclusionCalculationDatas = FrustumOcclusionCalculationDatas,
                    Results = Results
                }.Schedule(totalFrustumCounter, 36).Complete();

                //Store results
                foreach (var result in Results)
                {
                    if (result.Isinitialized)
                    {
                        this.CalculatedFrustums[result.FrustumCalculationDataID.ObstacleListenerUniqueID][result.FrustumCalculationDataID.SquareObstacleSystemUniqueID].Add(result.FrustumPointsPositions);
                    }
                }

                //Clear data that changed
                this.obstacleListenersThatHasChangedThisFrame.Clear();
                foreach (var singleObstacleSystemThatChanged in this.singleObstacleSystemsThatHasChangedThisFrame)
                {
                    singleObstacleSystemThatChanged.Value.Clear();
                }

                FrustumOcclusionCalculationDatas.Dispose();
                AssociatedFrustums.Dispose();
                Results.Dispose();

            }

            Profiler.EndSample();
        }

        private static void AddToArrays(ref NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas, NativeArray<FrustumV2Indexed> AssociatedFrustums,
            ref int currentOcclusionCalculationCounter, ref int currentFrustumCounter, ObstacleListener obstacleListenerThatChanged, SquareObstacleSystem nearSquareObstacle)
        {
            Debug.Log(obstacleListenerThatChanged.ObstacleListenerUniqueID + " " + nearSquareObstacle.SquareObstacleSystemUniqueID);
            foreach (var nearSquaureObstacleFrustum in nearSquareObstacle.FaceFrustums)
            {
                AssociatedFrustums[currentFrustumCounter] = new FrustumV2Indexed
                {
                    FrustumV2 = nearSquaureObstacleFrustum,
                    CalculationDataIndex = currentOcclusionCalculationCounter
                };
                currentFrustumCounter += 1;
            }

            FrustumOcclusionCalculationDatas[currentOcclusionCalculationCounter] = new FrustumOcclusionCalculationData
            {
                FrustumCalculationDataID = new FrustumCalculationDataID
                {
                    ObstacleListenerUniqueID = obstacleListenerThatChanged.ObstacleListenerUniqueID,
                    SquareObstacleSystemUniqueID = nearSquareObstacle.SquareObstacleSystemUniqueID,
                },
                ObstacleListenerTransform = obstacleListenerThatChanged.AssociatedRangeObject.GetTransform(),
                SquareObstacleTransform = nearSquareObstacle.GetTransform()
            };

            currentOcclusionCalculationCounter += 1;
        }

        public void OnDestroy()
        {
            Instance = null;
        }

        private void ClearAndCreateCalculatedFrustums(ObstacleListener obstacleListener, SquareObstacleSystem SquareObstacleSystem)
        {
            this.CalculatedFrustums.TryGetValue(obstacleListener.ObstacleListenerUniqueID, out Dictionary<int, List<FrustumPointsPositions>> obstalceFrustumPointsPositions);
            if (obstalceFrustumPointsPositions == null)
            {
                this.CalculatedFrustums.Add(obstacleListener.ObstacleListenerUniqueID, new Dictionary<int, List<FrustumPointsPositions>>());
                obstalceFrustumPointsPositions = this.CalculatedFrustums[obstacleListener.ObstacleListenerUniqueID];
            }
            obstalceFrustumPointsPositions.TryGetValue(SquareObstacleSystem.SquareObstacleSystemUniqueID, out List<FrustumPointsPositions> squareObstacleFrustumPositions);
            if (squareObstacleFrustumPositions == null)
            {
                obstalceFrustumPointsPositions.Add(SquareObstacleSystem.SquareObstacleSystemUniqueID, new List<FrustumPointsPositions>());
            }
            else
            {
                squareObstacleFrustumPositions.Clear();
            }
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

