using System.Collections.Generic;
using CoreGame;
using Obstacle;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public class ObstacleOcclusionCalculationManagerV2 : GameSingleton<ObstacleOcclusionCalculationManagerV2>
    {
        //ObstacleListener -> SquareObstacleSystem -> FrustumPositions
        private Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>> CalculatedOcclusionFrustums = new Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>>();
        private Dictionary<ObstacleInteractiveObject, TransformStruct> ObstacleLastFramePositions = new Dictionary<ObstacleInteractiveObject, TransformStruct>();

        private Dictionary<ObstacleListenerSystem, TransformStruct> ObstacleListenerLastFramePositions = new Dictionary<ObstacleListenerSystem, TransformStruct>();

        private List<ObstacleListenerSystem> obstacleListenersThatHasChangedThisFrame = new List<ObstacleListenerSystem>();
        private Dictionary<ObstacleListenerSystem, List<ObstacleInteractiveObject>> singleObstacleThatHasChangedThisFrame = new Dictionary<ObstacleListenerSystem, List<ObstacleInteractiveObject>>();


        private Dictionary<int, Dictionary<int, List<FrustumPointsPositions>>> GetCalculatedOcclusionFrustums()
        {
            if (!JobEnded)
            {
                this.JobHandle.Complete();
                while (!this.JobHandle.IsCompleted)
                {
                }

                this.JobEnded = true;
                this.OnJobEnded();
            }

            return this.CalculatedOcclusionFrustums;
        }

        public List<FrustumPointsPositions> GetCalculatedOcclusionFrustums(ObstacleListenerSystem ObstacleListener, ObstacleInteractiveObject obstacleInteractiveObject)
        {
            return this.GetCalculatedOcclusionFrustums()[ObstacleListener.ObstacleListenerUniqueID][obstacleInteractiveObject.ObstacleInteractiveObjectUniqueID];
        }

        public void TryGetCalculatedOcclusionFrustumsForObstacleListener(ObstacleListenerSystem ObstacleListener, out Dictionary<int, List<FrustumPointsPositions>> calculatedFrustumPositions)
        {
            this.GetCalculatedOcclusionFrustums().TryGetValue(ObstacleListener.ObstacleListenerUniqueID, out calculatedFrustumPositions);
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("ObstacleOcclusionCalculationManagerV2");
            this.ManualCalculation(this.ObstaclesListenerManager.GetAllObstacleListeners(), forceCalculation: false);
            Profiler.EndSample();
        }

        public void ManualCalculation(List<ObstacleListenerSystem> ConcernedObstacleListeners, bool forceCalculation)
        {
            int occlusionCalculationCounter = 0;
            int totalFrustumCounter = 0;

            if (!forceCalculation)
            {
                //Position change detection
                foreach (var obstacleListener in ConcernedObstacleListeners)
                {
                    this.ObstacleListenerLastFramePositions.TryGetValue(obstacleListener, out TransformStruct lastFramePosition);
                    bool hasChanged = !obstacleListener.AssociatedRangeObject.GetTransform().IsEqualTo(lastFramePosition);

                    //The obstacle listener has changed -> all associated near square obstacles are updated
                    if (hasChanged)
                    {
                        this.obstacleListenersThatHasChangedThisFrame.Add(obstacleListener);

                        foreach (var obstacleInteractiveObject in obstacleListener.NearSquareObstacles)
                        {
                            occlusionCalculationCounter += 1;
                            totalFrustumCounter += obstacleInteractiveObject.SquareObstacleOcclusionFrustumsDefinition.FaceFrustums.Count;
                            this.ClearAndCreateCalculatedFrustums(obstacleListener, obstacleInteractiveObject);
                        }
                    }

                    //The obstacle listener hasn't changed -> we compate near square obstacles positions for update
                    else
                    {
                        this.singleObstacleThatHasChangedThisFrame.TryGetValue(obstacleListener, out List<ObstacleInteractiveObject> obstacleInteractiveObjectsThatChanged);
                        if (obstacleInteractiveObjectsThatChanged == null)
                        {
                            this.singleObstacleThatHasChangedThisFrame.Add(obstacleListener, new List<ObstacleInteractiveObject>());
                            obstacleInteractiveObjectsThatChanged = this.singleObstacleThatHasChangedThisFrame[obstacleListener];
                        }


                        foreach (var squareObstacle in obstacleListener.NearSquareObstacles)
                        {
                            this.ObstacleLastFramePositions.TryGetValue(squareObstacle, out TransformStruct lastFrameSquareObstacleTrasform);
                            //Static obstacles are not detecting changes
                            if (!squareObstacle.SquareObstacleSystemInitializationData.IsStatic
                                && !squareObstacle.GetObstacleCenterTransform().IsEqualTo(lastFrameSquareObstacleTrasform))
                            {
                                //We add this single couple (listener <-> obstacle) to calculation
                                obstacleInteractiveObjectsThatChanged.Add(squareObstacle);
                                occlusionCalculationCounter += 1;
                                totalFrustumCounter += squareObstacle.SquareObstacleOcclusionFrustumsDefinition.FaceFrustums.Count;
                                this.ClearAndCreateCalculatedFrustums(obstacleListener, squareObstacle);
                            }
                        }
                    }

                    //Update Obstacle Listener Positions
                    this.ObstacleListenerLastFramePositions[obstacleListener] = obstacleListener.AssociatedRangeObject.GetTransform();
                }

                //Update Square Obstacle Positions
                foreach (var obstacleInteractiveObject in this._obstacleInteractiveObjectManager.AllObstacleInteractiveObjects)
                {
                    this.ObstacleLastFramePositions[obstacleInteractiveObject] = obstacleInteractiveObject.GetObstacleCenterTransform();
                }
            }
            else
            {
                foreach (var obstacleListener in ConcernedObstacleListeners)
                {
                    foreach (var squareObstacle in obstacleListener.NearSquareObstacles)
                    {
                        occlusionCalculationCounter += 1;
                        totalFrustumCounter += squareObstacle.SquareObstacleOcclusionFrustumsDefinition.FaceFrustums.Count;
                    }
                }
            }

            if (occlusionCalculationCounter > 0)
            {
                this.FrustumOcclusionCalculationDatas = new NativeArray<FrustumOcclusionCalculationData>(occlusionCalculationCounter, Allocator.TempJob);
                this.AssociatedFrustums = new NativeArray<FrustumV2Indexed>(totalFrustumCounter, Allocator.TempJob);
                this.Results = new NativeArray<FrustumPointsWithInitializedFlag>(totalFrustumCounter, Allocator.TempJob);

                int currentOcclusionCalculationCounter = 0;
                int currentFrustumCounter = 0;

                foreach (var obstacleListenerThatChanged in this.obstacleListenersThatHasChangedThisFrame)
                {
                    foreach (var nearSquareObstacle in obstacleListenerThatChanged.NearSquareObstacles)
                    {
                        AddToArrays(ref FrustumOcclusionCalculationDatas, AssociatedFrustums, ref currentOcclusionCalculationCounter, ref currentFrustumCounter, obstacleListenerThatChanged, nearSquareObstacle);
                    }
                }

                foreach (var singleObstacleSystemThatChanged in this.singleObstacleThatHasChangedThisFrame)
                {
                    if (singleObstacleSystemThatChanged.Value.Count > 0)
                    {
                        foreach (var nearSquareObstacle in singleObstacleSystemThatChanged.Value)
                        {
                            AddToArrays(ref FrustumOcclusionCalculationDatas, AssociatedFrustums, ref currentOcclusionCalculationCounter, ref currentFrustumCounter, singleObstacleSystemThatChanged.Key, nearSquareObstacle);
                        }
                    }
                }

                var jobHandle = new FrustumOcclusionCalculationJob()
                {
                    AssociatedFrustums = AssociatedFrustums,
                    FrustumOcclusionCalculationDatas = FrustumOcclusionCalculationDatas,
                    Results = Results
                }.Schedule(totalFrustumCounter, 36);

                if (!forceCalculation)
                {
                    this.JobEnded = false;
                    this.JobHandle = jobHandle;
                }
                else
                {
                    jobHandle.Complete();
                    while (!jobHandle.IsCompleted)
                    {
                    }

                    this.OnJobEnded();
                }
            }
            else
            {
                this.ClearFrameDependantData();
            }
        }

        private void OnJobEnded()
        {
            //Store results
            foreach (var result in Results)
            {
                if (result.Isinitialized)
                {
                    this.CalculatedOcclusionFrustums[result.FrustumCalculationDataID.ObstacleListenerUniqueID][result.FrustumCalculationDataID.SquareObstacleSystemUniqueID].Add(result.FrustumPointsPositions);
                }
            }

            ClearFrameDependantData();

            if (FrustumOcclusionCalculationDatas.IsCreated)
            {
                FrustumOcclusionCalculationDatas.Dispose();
            }

            if (AssociatedFrustums.IsCreated)
            {
                AssociatedFrustums.Dispose();
            }

            if (Results.IsCreated)
            {
                Results.Dispose();
            }
        }

        private void ClearFrameDependantData()
        {
            //Clear data that changed
            this.obstacleListenersThatHasChangedThisFrame.Clear();
            foreach (var singleObstacleSystemThatChanged in this.singleObstacleThatHasChangedThisFrame)
            {
                singleObstacleSystemThatChanged.Value.Clear();
            }
        }

        private static void AddToArrays(ref NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas, NativeArray<FrustumV2Indexed> AssociatedFrustums,
            ref int currentOcclusionCalculationCounter, ref int currentFrustumCounter, ObstacleListenerSystem obstacleListenerThatChanged, ObstacleInteractiveObject nearSquareObstacle)
        {
            foreach (var nearSquaureObstacleFrustum in nearSquareObstacle.SquareObstacleOcclusionFrustumsDefinition.FaceFrustums)
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
                    SquareObstacleSystemUniqueID = nearSquareObstacle.ObstacleInteractiveObjectUniqueID,
                },
                ObstacleListenerTransform = obstacleListenerThatChanged.AssociatedRangeObject.GetTransform(),
                SquareObstacleTransform = nearSquareObstacle.GetObstacleCenterTransform()
            };

            currentOcclusionCalculationCounter += 1;
        }

        public void LateTick()
        {
            //We trigger the end of calculations
            GetCalculatedOcclusionFrustums();
        }

        private void ClearAndCreateCalculatedFrustums(ObstacleListenerSystem obstacleListener, ObstacleInteractiveObject obstalceInteractiveObject)
        {
            this.CalculatedOcclusionFrustums.TryGetValue(obstacleListener.ObstacleListenerUniqueID, out Dictionary<int, List<FrustumPointsPositions>> obstalceFrustumPointsPositions);
            if (obstalceFrustumPointsPositions == null)
            {
                this.CalculatedOcclusionFrustums.Add(obstacleListener.ObstacleListenerUniqueID, new Dictionary<int, List<FrustumPointsPositions>>());
                obstalceFrustumPointsPositions = this.CalculatedOcclusionFrustums[obstacleListener.ObstacleListenerUniqueID];
            }

            obstalceFrustumPointsPositions.TryGetValue(obstalceInteractiveObject.ObstacleInteractiveObjectUniqueID, out List<FrustumPointsPositions> squareObstacleFrustumPositions);
            if (squareObstacleFrustumPositions == null)
            {
                obstalceFrustumPointsPositions.Add(obstalceInteractiveObject.ObstacleInteractiveObjectUniqueID, new List<FrustumPointsPositions>());
            }
            else
            {
                squareObstacleFrustumPositions.Clear();
            }
        }

        #region External Dependencies

        private ObstaclesListenerManager ObstaclesListenerManager = ObstaclesListenerManager.Get();
        private ObstacleInteractiveObjectManager _obstacleInteractiveObjectManager = ObstacleInteractiveObjectManager.Get();

        #endregion

        #region Native Arrays

        private NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas;
        private NativeArray<FrustumV2Indexed> AssociatedFrustums;
        private NativeArray<FrustumPointsWithInitializedFlag> Results;

        #endregion

        #region Job State   

        private bool JobEnded;
        private JobHandle JobHandle;

        #endregion
    }

    [BurstCompile]
    public struct FrustumOcclusionCalculationJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<FrustumOcclusionCalculationData> FrustumOcclusionCalculationDatas;

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

    #region Type Definition

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

    #endregion
}