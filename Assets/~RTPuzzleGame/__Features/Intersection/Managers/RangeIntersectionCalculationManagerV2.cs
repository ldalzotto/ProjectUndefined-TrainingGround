using CoreGame;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public class RangeIntersectionCalculationManagerV2 : GameSingleton<RangeIntersectionCalculationManagerV2>
    {
        public RangeIntersectionCalculationManagerV2()
        {
            this.RangeIntersectionmanagers = new IIntersectionManager[] {
                new SphereIntersectionManager(),
                new RoundedFrustumIntersectionManager()
            };
        }

        #region External Dependencies
        private ObstacleOcclusionCalculationManagerV2 ObstacleOcclusionCalculationManagerV2 = ObstacleOcclusionCalculationManagerV2.Get();
        private RangeIntersectionCalculatorV2Manager RangeIntersectionCalculatorV2Manager = RangeIntersectionCalculatorV2Manager.Get();
        #endregion

        private NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;

        #region Job State   
        private bool JobEnded;
        #endregion

        private List<RangeIntersectionCalculatorV2> RangeIntersectionCalculatorThatChangedThatFrame = new List<RangeIntersectionCalculatorV2>();

        private RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection;

        private IIntersectionManager[] RangeIntersectionmanagers;

        //RangeIntersectionCalculatorV2 -> intersection value
        private Dictionary<int, bool> RangeIntersectionResults = new Dictionary<int, bool>();

        private Dictionary<int, bool> GetRangeIntersectionResult()
        {
            if (!this.JobEnded)
            {
                this.JobEnded = true;
                foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                {
                    RangeIntersectionmanager.Complete();
                }
                foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                {
                    RangeIntersectionmanager.WaitForResults();
                }
                this.OnJobEnded();
            }
            return this.RangeIntersectionResults;
        }

        public bool GetRangeIntersectionResult(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2)
        {
            return this.GetRangeIntersectionResult()[RangeIntersectionCalculatorV2.RangeIntersectionCalculatorV2UniqueID];
        }
        public void TryGetRangeintersectionResult(RangeIntersectionCalculatorV2 RangeIntersectionCalculatorV2, out bool result)
        {
            this.GetRangeIntersectionResult().TryGetValue(RangeIntersectionCalculatorV2.RangeIntersectionCalculatorV2UniqueID, out result);
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

                int totalObstacleFrustumPointsCounter = 0;
                foreach (var rangeIntersectionCalculatorV2 in InvolvedRangeIntersectionCalculatorV2)
                {

                    if (forceCalculation || (!forceCalculation && rangeIntersectionCalculatorV2.TickChangedPositions()))
                    {
                        this.RangeIntersectionCalculatorThatChangedThatFrame.Add(rangeIntersectionCalculatorV2);
                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                        {
                            RangeIntersectionmanager.CountingForRangeIntersectionCalculator(rangeIntersectionCalculatorV2);
                        }

                        var associatedObstacleListener = rangeIntersectionCalculatorV2.GetAssociatedObstacleListener();
                        if (associatedObstacleListener != null) //The range can ignore obstacles
                        {
                            //Obstacle listener could have never triggered a calculation
                            ObstacleOcclusionCalculationManagerV2.TryGetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener, out Dictionary<int, List<FrustumPointsPositions>> calculatedFrustumPositions);
                            if (calculatedFrustumPositions != null)
                            {
                                foreach (var calculatedObstacleFrustum in calculatedFrustumPositions.Values)
                                {
                                    totalObstacleFrustumPointsCounter += calculatedObstacleFrustum.Count;
                                }
                            }
                            
                        }
                    }
                }
                #endregion

                if (this.RangeIntersectionCalculatorThatChangedThatFrame.Count > 0)
                {
                    foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                    {
                        RangeIntersectionmanager.CreateNativeArrays();
                    }

                    this.IsOccludedByObstacleJobData = new NativeArray<IsOccludedByObstacleJobData>(AllRangeIntersectionCalculatorV2Count, Allocator.TempJob);

                    this.RangeObstacleOcclusionIntersection.Prepare(totalObstacleFrustumPointsCounter, this.RangeIntersectionCalculatorV2Manager);

                    int currentObstacleIntersectionCalculatorCounter = 0;
                    foreach (var RangeIntersectionCalculatorV2 in this.RangeIntersectionCalculatorThatChangedThatFrame)
                    {

                        if (this.RangeObstacleOcclusionIntersection.ForRangeInteresectionCalculator(RangeIntersectionCalculatorV2, this.ObstacleOcclusionCalculationManagerV2, out IsOccludedByObstacleJobData IsOccludedByObstacleJobData))
                        {
                            this.IsOccludedByObstacleJobData[currentObstacleIntersectionCalculatorCounter] = IsOccludedByObstacleJobData;
                            currentObstacleIntersectionCalculatorCounter += 1;
                        }

                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                        {
                            RangeIntersectionmanager.CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculatorV2,
                                 IsOccludedByObstacleJobData, currentObstacleIntersectionCalculatorCounter);
                        }
                    }

                    foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                    {
                        RangeIntersectionmanager.BuildJobHandle(this.IsOccludedByObstacleJobData, this.RangeObstacleOcclusionIntersection);
                    }

                    if (!forceCalculation)
                    {
                        this.JobEnded = false;
                    }
                    else
                    {
                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                        {
                            RangeIntersectionmanager.Complete();
                        }
                        foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
                        {
                            RangeIntersectionmanager.WaitForResults();
                        }
                        this.OnJobEnded();
                    }
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
            foreach (var RangeIntersectionmanager in RangeIntersectionmanagers)
            {
                foreach (var IntersectionJobResult in RangeIntersectionmanager.GetIntersectionResults())
                {
                    this.RangeIntersectionResults[IntersectionJobResult.RangeIntersectionCalculatorV2UniqueID] = IntersectionJobResult.IsInsideRange;
                }
                RangeIntersectionmanager.Dispose();
                RangeIntersectionmanager.ClearState();
                this.RangeIntersectionCalculatorThatChangedThatFrame.Clear();
            }

            if (this.IsOccludedByObstacleJobData.IsCreated) { this.IsOccludedByObstacleJobData.Dispose(); }

            this.RangeObstacleOcclusionIntersection.Dispose();
        }
        
    }
}
