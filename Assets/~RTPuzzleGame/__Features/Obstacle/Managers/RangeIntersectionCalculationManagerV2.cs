using CoreGame;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Profiling;

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

        private NativeArray<RoundedFrustumIntersectionJobData> RoundedFrustumIntersectionJobData;
        private NativeArray<RangeIntersectionResult> RoundedFrustumIntersectionJobResult;

        private NativeArray<SphereIntersectionJobData> SphereIntersectionJobData;
        private NativeArray<RangeIntersectionResult> SphereIntersectionJobResult;

        private NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;

        #region Job State   
        private bool JobEnded;
        private JobHandle SphereIntersectionJobHandle;
        private JobHandle RoudedFrustumIntersectionJobHandle;
        #endregion

        private RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection;

        private Dictionary<RangeIntersectionCalculatorV2, RangeIntersectionState> RangeIntersectionStates = new Dictionary<RangeIntersectionCalculatorV2, RangeIntersectionState>();

        //RangeIntersectionCalculatorV2 -> intersection value
        private Dictionary<int, bool> RangeIntersectionResults = new Dictionary<int, bool>();

        private Dictionary<int, bool> GetRangeIntersectionResult()
        {
            if (!this.JobEnded)
            {
                this.RoudedFrustumIntersectionJobHandle.Complete();
                this.SphereIntersectionJobHandle.Complete();
                while (!this.RoudedFrustumIntersectionJobHandle.IsCompleted){}
                while (!this.SphereIntersectionJobHandle.IsCompleted) { }
                this.OnJobEnded();
            }
            return this.RangeIntersectionResults;
        }

        public bool GetRangeIntersectionResult(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2)
        {
            return this.GetRangeIntersectionResult()[RangeIntersectionCalculatorV2.RangeIntersectionCalculatorV2UniqueID];
        }

        public void Tick(float d)
        {
            Profiler.BeginSample("RangeIntersectionCalculationManagerV2");
            this.ManualCalculation(this.RangeIntersectionCalculatorV2Manager.AllRangeIntersectionCalculatorV2, forceCalculation: false);
            Profiler.EndSample();
        }

        public void ManualCalculation(List<RangeIntersectionCalculatorV2> InvolvedRangeIntersectionCalculatorV2, bool forceCalculation)
        {
            var AllRangeIntersectionCalculatorV2Count = InvolvedRangeIntersectionCalculatorV2.Count;
            if (AllRangeIntersectionCalculatorV2Count > 0)
            {

                #region Counting
                int totalRoundedFrustumTypeIntersection = 0;
                int totalSphereTypeIntersection = 0;

                int totalObstacleFrustumPointsCounter = 0;
                foreach (var rangeIntersectionCalculatorV2 in InvolvedRangeIntersectionCalculatorV2)
                {
                    switch (rangeIntersectionCalculatorV2.GetAssociatedRangeObjectType())
                    {
                        case RangeType.ROUNDED_FRUSTUM:
                            totalRoundedFrustumTypeIntersection += 1;
                            break;
                        case RangeType.SPHERE:
                            totalSphereTypeIntersection += 1;
                            break;
                    }

                    var associatedObstacleListener = rangeIntersectionCalculatorV2.GetAssociatedObstacleListener();
                    foreach (var calculatedObstacleFrustum in ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener).Values)
                    {
                        totalObstacleFrustumPointsCounter += calculatedObstacleFrustum.Count;
                    }
                }
                #endregion

                this.RoundedFrustumIntersectionJobData = new NativeArray<RoundedFrustumIntersectionJobData>(totalRoundedFrustumTypeIntersection, Allocator.TempJob);
                this.RoundedFrustumIntersectionJobResult = new NativeArray<RangeIntersectionResult>(totalRoundedFrustumTypeIntersection, Allocator.TempJob);

                this.SphereIntersectionJobData = new NativeArray<SphereIntersectionJobData>(totalSphereTypeIntersection, Allocator.TempJob);
                this.SphereIntersectionJobResult = new NativeArray<RangeIntersectionResult>(totalSphereTypeIntersection, Allocator.TempJob);

                this.IsOccludedByObstacleJobData = new NativeArray<IsOccludedByObstacleJobData>(AllRangeIntersectionCalculatorV2Count, Allocator.TempJob);

                this.RangeObstacleOcclusionIntersection.Prepare(totalObstacleFrustumPointsCounter, this.RangeIntersectionCalculatorV2Manager);

                int currentRoundedFrustumIntersectionJobDataCounter = 0;
                int currentSphereIntersectionJobdataCounter = 0;

                int currentRangeIntersectionCalculatorCounter = 0;
                foreach (var RangeIntersectionCalculatorV2 in InvolvedRangeIntersectionCalculatorV2)
                {

                    if (this.RangeObstacleOcclusionIntersection.ForRangeInteresectionCalculator(RangeIntersectionCalculatorV2, this.ObstacleOcclusionCalculationManagerV2, out IsOccludedByObstacleJobData IsOccludedByObstacleJobData))
                    {
                        this.IsOccludedByObstacleJobData[currentRangeIntersectionCalculatorCounter] = IsOccludedByObstacleJobData;
                        currentRangeIntersectionCalculatorCounter += 1;
                    }

                    switch (RangeIntersectionCalculatorV2.GetAssociatedRangeObjectType())
                    {
                        case RangeType.ROUNDED_FRUSTUM:
                            var RoundedFrustumRangeObject = (RoundedFrustumRangeObjectV2)RangeIntersectionCalculatorV2.GetAssociatedRangeObject();
                            var RoundedFrustumIntersectionJobData = new RoundedFrustumIntersectionJobData
                            {
                                RangeIntersectionCalculatorV2UniqueID = RangeIntersectionCalculatorV2.RangeIntersectionCalculatorV2UniqueID,
                                FrustumRadius = RoundedFrustumRangeObject.GetFrustum().GetFrustumFaceRadius(),
                                RangeTransform = RoundedFrustumRangeObject.GetTransform(),
                                IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                                RoundedFrustumPositions = RoundedFrustumRangeObject.GetFrustumWorldPositions(),
                                ComparedCollider = RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition(),
                                ObstacleCalculationDataIndex = RoundedFrustumRangeObject.GetObstacleListener() == null ? -1 : (currentRangeIntersectionCalculatorCounter - 1)
                            };
                            this.RoundedFrustumIntersectionJobData[currentRoundedFrustumIntersectionJobDataCounter] = RoundedFrustumIntersectionJobData;
                            currentRoundedFrustumIntersectionJobDataCounter += 1;
                            break;
                        case RangeType.SPHERE:
                            var SphereRangeObject = (SphereRangeObjectV2)RangeIntersectionCalculatorV2.GetAssociatedRangeObject();
                            var SphereIntersectionJobData = new SphereIntersectionJobData
                            {
                                RangeIntersectionCalculatorV2UniqueID = RangeIntersectionCalculatorV2.RangeIntersectionCalculatorV2UniqueID,
                                RangeTransform = SphereRangeObject.GetTransform(),
                                IsOccludedByObstacleJobData = IsOccludedByObstacleJobData,
                                ComparedCollider = RangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.GetLogicColliderBoxDefinition(),
                                ObstacleCalculationDataIndex = SphereRangeObject.GetObstacleListener() == null ? -1 : (currentRangeIntersectionCalculatorCounter - 1),
                                SphereRadius = SphereRangeObject.SphereBoundingCollider.radius
                            };
                            this.SphereIntersectionJobData[currentSphereIntersectionJobdataCounter] = SphereIntersectionJobData;
                            currentSphereIntersectionJobdataCounter += 1;
                            break;
                    }

                }

                var roudedFrustumIntersectionJobHandle = new RoundedFrustumIntersectionJob
                {
                    RoundedFrustumIntersectionJobData = this.RoundedFrustumIntersectionJobData,
                    IsOccludedByObstacleJobData = this.IsOccludedByObstacleJobData,
                    AssociatedObstacleFrustumPointsPositions = this.RangeObstacleOcclusionIntersection.AssociatedObstacleFrustumPointsPositions,
                    IntersectionResult = this.RoundedFrustumIntersectionJobResult
                }.Schedule(totalRoundedFrustumTypeIntersection, 5);

                var sphereIntersectionJobHandle = new SphereIntersectionJob
                {
                    SphereIntersectionJobDatas = this.SphereIntersectionJobData,
                    IsOccludedByObstacleJobData = this.IsOccludedByObstacleJobData,
                    AssociatedObstacleFrustumPointsPositions = this.RangeObstacleOcclusionIntersection.AssociatedObstacleFrustumPointsPositions,
                    IntersectionResult = this.SphereIntersectionJobResult
                }.Schedule(totalSphereTypeIntersection, 5);

                if (!forceCalculation)
                {
                    this.JobEnded = false;
                    this.RoudedFrustumIntersectionJobHandle = roudedFrustumIntersectionJobHandle;
                    this.SphereIntersectionJobHandle = sphereIntersectionJobHandle;
                }
                else
                {
                    roudedFrustumIntersectionJobHandle.Complete();
                    sphereIntersectionJobHandle.Complete();
                    while (!roudedFrustumIntersectionJobHandle.IsCompleted) { }
                    while (!sphereIntersectionJobHandle.IsCompleted) { }
                    this.OnJobEnded();
                }


            }
        }

        public void LateTick()
        {
            //We trigger the end of calculations
            GetRangeIntersectionResult();
        }

        private void OnJobEnded()
        {
            foreach (var IntersectionJobResult in this.RoundedFrustumIntersectionJobResult)
            {
                this.RangeIntersectionResults[IntersectionJobResult.RangeIntersectionCalculatorV2UniqueID] = IntersectionJobResult.IsInsideRange;
            }

            foreach (var sphereJobResult in this.SphereIntersectionJobResult)
            {
                this.RangeIntersectionResults[sphereJobResult.RangeIntersectionCalculatorV2UniqueID] = sphereJobResult.IsInsideRange;
            }

            if (this.RoundedFrustumIntersectionJobData.IsCreated) { this.RoundedFrustumIntersectionJobData.Dispose(); }
            if (this.RoundedFrustumIntersectionJobResult.IsCreated) { this.RoundedFrustumIntersectionJobResult.Dispose(); }
            if (this.SphereIntersectionJobData.IsCreated) { this.SphereIntersectionJobData.Dispose(); }
            if (this.SphereIntersectionJobResult.IsCreated) { this.SphereIntersectionJobResult.Dispose(); }
            
            if (this.IsOccludedByObstacleJobData.IsCreated) { this.IsOccludedByObstacleJobData.Dispose(); }
            this.RangeObstacleOcclusionIntersection.Dispose();
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }

    public struct RangeObstacleOcclusionIntersection
    {
        public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;

        private int currentObstacleFrustumPointsCounter;

        public void Prepare(int totalObstacleFrustumPointsCounter, RangeIntersectionCalculatorV2Manager RangeIntersectionCalculatorV2Manager)
        {
            int AllRangeIntersectionCalculatorV2Count = RangeIntersectionCalculatorV2Manager.AllRangeIntersectionCalculatorV2.Count;
            this.AssociatedObstacleFrustumPointsPositions = new NativeArray<FrustumPointsPositions>(totalObstacleFrustumPointsCounter, Allocator.TempJob);
            this.currentObstacleFrustumPointsCounter = 0;
        }

        public bool ForRangeInteresectionCalculator(RangeIntersectionCalculatorV2 rangeIntersectionCalculatorV2, ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2,
            out IsOccludedByObstacleJobData IsOccludedByObstacleJobData)
        {
            int ObstacleFrustumPointsPositionsBeginIndex = currentObstacleFrustumPointsCounter;
            var associatedObstacleListener = rangeIntersectionCalculatorV2.GetAssociatedObstacleListener();

            if (associatedObstacleListener != null)
            {
                foreach (var calculatedObstacleFrustumList in ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener).Values)
                {
                    foreach (var calculatedObstacleFrustum in calculatedObstacleFrustumList)
                    {
                        this.AssociatedObstacleFrustumPointsPositions[currentObstacleFrustumPointsCounter] = calculatedObstacleFrustum;
                        currentObstacleFrustumPointsCounter += 1;
                    }
                }
                IsOccludedByObstacleJobData = new IsOccludedByObstacleJobData
                {
                    TestedBoxCollider = new BoxDefinition(rangeIntersectionCalculatorV2.TrackedInteractiveObject.InteractiveGameObject.LogicCollider),
                    ObstacleFrustumPointsPositionsBeginIndex = ObstacleFrustumPointsPositionsBeginIndex,
                    ObstacleFrustumPointsPositionsEndIndex = currentObstacleFrustumPointsCounter
                };
                return true;
            }

            IsOccludedByObstacleJobData = default;
            return false;
        }

        public void Dispose()
        {
            if (this.AssociatedObstacleFrustumPointsPositions.IsCreated) { this.AssociatedObstacleFrustumPointsPositions.Dispose(); }
        }

    }

    public struct IsOccludedByObstacleJobData
    {
        public BoxDefinition TestedBoxCollider;
        public int ObstacleFrustumPointsPositionsBeginIndex;
        public int ObstacleFrustumPointsPositionsEndIndex;

        public bool IsOccluded(NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions)
        {
            Intersection.ExtractBoxColliderWorldPointsV2(this.TestedBoxCollider,
               out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

            return
                  IsPointFullyOccludedByObstacle(BC1, AssociatedObstacleFrustumPointsPositions) &&
                  IsPointFullyOccludedByObstacle(BC2, AssociatedObstacleFrustumPointsPositions) &&
                  IsPointFullyOccludedByObstacle(BC3, AssociatedObstacleFrustumPointsPositions) &&
                  IsPointFullyOccludedByObstacle(BC4, AssociatedObstacleFrustumPointsPositions) &&
                  IsPointFullyOccludedByObstacle(BC5, AssociatedObstacleFrustumPointsPositions) &&
                  IsPointFullyOccludedByObstacle(BC6, AssociatedObstacleFrustumPointsPositions) &&
                  IsPointFullyOccludedByObstacle(BC7, AssociatedObstacleFrustumPointsPositions) &&
                  IsPointFullyOccludedByObstacle(BC8, AssociatedObstacleFrustumPointsPositions);
        }

        private bool IsPointFullyOccludedByObstacle(Vector3 PointWorldPosition, NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions)
        {
            bool isOccluded = true;

            for (var FrustumPointsPositionsIndex = this.ObstacleFrustumPointsPositionsBeginIndex; FrustumPointsPositionsIndex < this.ObstacleFrustumPointsPositionsEndIndex; FrustumPointsPositionsIndex++)
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

    [BurstCompile(CompileSynchronously = true)]
    public struct SphereIntersectionJob : IJobParallelFor
    {
        public NativeArray<SphereIntersectionJobData> SphereIntersectionJobDatas;
        public NativeArray<RangeIntersectionResult> IntersectionResult;

        [ReadOnly]
        public NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
        [ReadOnly]
        public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;

        public void Execute(int SphereIntersectionJobDataIndex)
        {
            var SphereIntersectionJobData = this.SphereIntersectionJobDatas[SphereIntersectionJobDataIndex];
            bool isInsideRange = Intersection.BoxIntersectsOrEntirelyContainedInSphere(SphereIntersectionJobData.ComparedCollider, SphereIntersectionJobData.RangeTransform.WorldPosition, SphereIntersectionJobData.SphereRadius);
            if (SphereIntersectionJobData.ObstacleCalculationDataIndex != -1 && isInsideRange)
            {
                isInsideRange = isInsideRange && !this.IsOccludedByObstacleJobData[SphereIntersectionJobData.ObstacleCalculationDataIndex].IsOccluded(this.AssociatedObstacleFrustumPointsPositions);
            }
            this.IntersectionResult[SphereIntersectionJobDataIndex] =
              new RangeIntersectionResult
              {
                  RangeIntersectionCalculatorV2UniqueID = SphereIntersectionJobData.RangeIntersectionCalculatorV2UniqueID,
                  IsInsideRange = isInsideRange
              };
        }
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct RoundedFrustumIntersectionJob : IJobParallelFor
    {
        public NativeArray<RoundedFrustumIntersectionJobData> RoundedFrustumIntersectionJobData;
        public NativeArray<RangeIntersectionResult> IntersectionResult;

        [ReadOnly]
        public NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;
        [ReadOnly]
        public NativeArray<FrustumPointsPositions> AssociatedObstacleFrustumPointsPositions;

        public void Execute(int RoundedFrustumIntersectionDataIndex)
        {
            var RoundedFrustumIntersectionJobData = this.RoundedFrustumIntersectionJobData[RoundedFrustumIntersectionDataIndex];
            bool isInsideRange = IsInside(RoundedFrustumIntersectionJobData);
            if (RoundedFrustumIntersectionJobData.ObstacleCalculationDataIndex != -1 && isInsideRange)
            {
                isInsideRange = isInsideRange && !this.IsOccludedByObstacleJobData[RoundedFrustumIntersectionJobData.ObstacleCalculationDataIndex].IsOccluded(this.AssociatedObstacleFrustumPointsPositions);
            }
            this.IntersectionResult[RoundedFrustumIntersectionDataIndex] =
                new RangeIntersectionResult
                {
                    RangeIntersectionCalculatorV2UniqueID = RoundedFrustumIntersectionJobData.RangeIntersectionCalculatorV2UniqueID,
                    IsInsideRange = isInsideRange
                };
        }

        private bool IsInside(RoundedFrustumIntersectionJobData RoundedFrustumIntersectionJobData)
        {
            return Intersection.BoxIntersectsOrEntirelyContainedInSphere(RoundedFrustumIntersectionJobData.ComparedCollider, RoundedFrustumIntersectionJobData.RangeTransform.WorldPosition, RoundedFrustumIntersectionJobData.FrustumRadius)
                    && (Intersection.FrustumBoxIntersection(RoundedFrustumIntersectionJobData.RoundedFrustumPositions, RoundedFrustumIntersectionJobData.ComparedCollider) || Intersection.BoxEntirelyContainedInFrustum(RoundedFrustumIntersectionJobData.RoundedFrustumPositions, RoundedFrustumIntersectionJobData.ComparedCollider))
             ;
        }
    }

    public struct RangeIntersectionResult
    {
        public int RangeIntersectionCalculatorV2UniqueID;
        public bool IsInsideRange;
    }

    public struct RoundedFrustumIntersectionJobData
    {
        public int RangeIntersectionCalculatorV2UniqueID;
        public TransformStruct RangeTransform;
        public FrustumPointsPositions RoundedFrustumPositions;
        public float FrustumRadius;
        public BoxDefinition ComparedCollider;
        public IsOccludedByObstacleJobData IsOccludedByObstacleJobData;
        public int ObstacleCalculationDataIndex;
    }

    public struct SphereIntersectionJobData
    {
        public int RangeIntersectionCalculatorV2UniqueID;
        public TransformStruct RangeTransform;
        public float SphereRadius;
        public BoxDefinition ComparedCollider;
        public IsOccludedByObstacleJobData IsOccludedByObstacleJobData;
        public int ObstacleCalculationDataIndex;
    }

    public struct RangeIntersectionState
    {
        public RangeType RangeType;
    }
}
