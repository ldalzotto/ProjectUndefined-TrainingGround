using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
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
        
        private NativeArray<IsOccludedByObstacleJobData> IsOccludedByObstacleJobData;

        #region Job State   
        private bool JobEnded;
        #endregion

        private RangeObstacleOcclusionIntersection RangeObstacleOcclusionIntersection;

        private SphereIntersectionManager SphereIntersectionManager;
        private RoundedFrustumIntersectionManager RoundedFrustumIntersectionManager;

        //RangeIntersectionCalculatorV2 -> intersection value
        private Dictionary<int, bool> RangeIntersectionResults = new Dictionary<int, bool>();

        private Dictionary<int, bool> GetRangeIntersectionResult()
        {
            if (!this.JobEnded)
            {
                this.JobEnded = true;
                this.RoundedFrustumIntersectionManager.Complete();
                this.SphereIntersectionManager.Complete();
                this.RoundedFrustumIntersectionManager.WaitForResults();
                this.SphereIntersectionManager.WaitForResults();
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

                int totalObstacleFrustumPointsCounter = 0;
                foreach (var rangeIntersectionCalculatorV2 in InvolvedRangeIntersectionCalculatorV2)
                {
                    this.RoundedFrustumIntersectionManager.CountingForRangeIntersectionCalculator(rangeIntersectionCalculatorV2);
                    this.SphereIntersectionManager.CountingForRangeIntersectionCalculator(rangeIntersectionCalculatorV2);

                    var associatedObstacleListener = rangeIntersectionCalculatorV2.GetAssociatedObstacleListener();
                    foreach (var calculatedObstacleFrustum in ObstacleOcclusionCalculationManagerV2.GetCalculatedOcclusionFrustumsForObstacleListener(associatedObstacleListener).Values)
                    {
                        totalObstacleFrustumPointsCounter += calculatedObstacleFrustum.Count;
                    }
                }
                #endregion

                this.RoundedFrustumIntersectionManager.CreateNativeArrays();
                this.SphereIntersectionManager.CreateNativeArrays();

                this.IsOccludedByObstacleJobData = new NativeArray<IsOccludedByObstacleJobData>(AllRangeIntersectionCalculatorV2Count, Allocator.TempJob);

                this.RangeObstacleOcclusionIntersection.Prepare(totalObstacleFrustumPointsCounter, this.RangeIntersectionCalculatorV2Manager);


                int currentObstacleIntersectionCalculatorCounter = 0;
                foreach (var RangeIntersectionCalculatorV2 in InvolvedRangeIntersectionCalculatorV2)
                {

                    if (this.RangeObstacleOcclusionIntersection.ForRangeInteresectionCalculator(RangeIntersectionCalculatorV2, this.ObstacleOcclusionCalculationManagerV2, out IsOccludedByObstacleJobData IsOccludedByObstacleJobData))
                    {
                        this.IsOccludedByObstacleJobData[currentObstacleIntersectionCalculatorCounter] = IsOccludedByObstacleJobData;
                        currentObstacleIntersectionCalculatorCounter += 1;
                    }

                    this.RoundedFrustumIntersectionManager.CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculatorV2,
                             IsOccludedByObstacleJobData, currentObstacleIntersectionCalculatorCounter);
                    this.SphereIntersectionManager.CalculationDataSetupForRangeIntersectionCalculator(RangeIntersectionCalculatorV2,
                             IsOccludedByObstacleJobData, currentObstacleIntersectionCalculatorCounter);
                }

                this.RoundedFrustumIntersectionManager.BuildJobHandle(this.IsOccludedByObstacleJobData, this.RangeObstacleOcclusionIntersection);
                this.SphereIntersectionManager.BuildJobHandle(this.IsOccludedByObstacleJobData, this.RangeObstacleOcclusionIntersection);
               
                if (!forceCalculation)
                {
                    this.JobEnded = false;
                }
                else
                {
                    this.RoundedFrustumIntersectionManager.Complete();
                    this.SphereIntersectionManager.Complete();
                    this.RoundedFrustumIntersectionManager.WaitForResults();
                    this.SphereIntersectionManager.WaitForResults();
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
            foreach (var IntersectionJobResult in this.RoundedFrustumIntersectionManager.RoundedFrustumIntersectionJobResult)
            {
                this.RangeIntersectionResults[IntersectionJobResult.RangeIntersectionCalculatorV2UniqueID] = IntersectionJobResult.IsInsideRange;
            }

            foreach (var sphereJobResult in this.SphereIntersectionManager.SphereIntersectionJobResult)
            {
                this.RangeIntersectionResults[sphereJobResult.RangeIntersectionCalculatorV2UniqueID] = sphereJobResult.IsInsideRange;
            }

            this.RoundedFrustumIntersectionManager.Dispose();
            this.RoundedFrustumIntersectionManager.ClearState();

            this.SphereIntersectionManager.Dispose();
            this.SphereIntersectionManager.ClearState();

            if (this.IsOccludedByObstacleJobData.IsCreated) { this.IsOccludedByObstacleJobData.Dispose(); }

            this.RangeObstacleOcclusionIntersection.Dispose();
        }

        public void OnDestroy()
        {
            Instance = null;
        }
    }
}
