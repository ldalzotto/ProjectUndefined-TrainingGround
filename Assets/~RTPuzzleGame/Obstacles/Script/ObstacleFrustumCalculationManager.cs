using CoreGame;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public class ObstacleFrustumCalculationManager : MonoBehaviour
    {
        #region External Dependencies
        private SquareObstaclesManager SquareObstaclesManager;
        #endregion

        private Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>> calculationResults;
        public Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>> CalculationResults { get => calculationResults; }

        private ObstacleFrustumCalculationThread ObstacleFrustumCalculationThreadObject;

        public void Init()
        {
            this.ObstacleFrustumCalculationThreadObject = new ObstacleFrustumCalculationThread();

            this.SquareObstaclesManager = GameObject.FindObjectOfType<SquareObstaclesManager>();

            this.calculationResults = new Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>>();
        }

        private void OnDestroy()
        {
            this.ObstacleFrustumCalculationThreadObject.OnDestroy();
        }

        #region Data Retrieval
        public SquareObstacleFrustumCalculationResult GetResult(ObstacleListener ObstacleListener, SquareObstacle SquareObstacle)
        {
            return this.calculationResults[ObstacleListener][SquareObstacle];
        }
        #endregion

        #region External Events
        public void OnObstacleListenerCreation(ObstacleListener obstacleListener)
        {
            this.calculationResults[obstacleListener] = new Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>();
        }
        public void OnObstacleAddedToListener(ObstacleListener obstacleListener, SquareObstacle squareObstacle)
        {
            var calculation = new SquareObstacleFrustumCalculationResult(obstacleListener, squareObstacle);
            calculation.AskCalculation();
            this.calculationResults[obstacleListener][squareObstacle] = calculation;
        }
        public void OnObstacleRemovedToListener(ObstacleListener obstacleListener, SquareObstacle squareObstacle)
        {
            this.calculationResults[obstacleListener].Remove(squareObstacle);
        }
        #endregion

        #region Logical Conditions
        public bool IsPointOccludedByObstacles(ObstacleListener ObstacleListener, Vector3 worldPositionPoint)
        {
            if (Vector3.Distance(ObstacleListener.transform.position, worldPositionPoint) <= ObstacleListener.Radius)
            {
                //If calculation is waiting, we trigger sync calculation
                if (this.calculationResults[ObstacleListener].Values.Count == 0 || this.calculationResults[ObstacleListener].Values.ToList().Select(result => result).Where(result => result.CalculationAsked()).Count() > 0)
                {
                    //Debug.Log(MyLog.Format("Forced obstacle listener calculation : " + ObstacleListener.name));
                    this.UpdateSquareObstaclesOfListener(ObstacleListener, async: false);
                }

                foreach (var obstacleResultEntry in this.calculationResults[ObstacleListener].Values)
                {
                    bool pointContainedInOcclusionFrustum = false;
                    foreach (var calculatedFrustumPosition in obstacleResultEntry.CalculatedFrustumPositions)
                    {
                        pointContainedInOcclusionFrustum = pointContainedInOcclusionFrustum || Intersection.PointInsideFrustum(calculatedFrustumPosition, worldPositionPoint);
                    }

                    if (pointContainedInOcclusionFrustum)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        public void Tick(float d)
        {
            Profiler.BeginSample("ObstacleFrustumCalculationManagerTick");
            #region Change Detection
            var calculationResultsKeys = this.calculationResults.Keys.ToList();
            foreach (var obstacleListener in calculationResultsKeys)
            {
                if (obstacleListener.HasPositionChanged())
                {
                    this.UpdateSquareObstaclesOfListener(obstacleListener);
                }
            }

            foreach (var changedObstacles in this.SquareObstaclesManager.LastFrameChangedObstacles)
            {
                foreach (var obstacleListener in calculationResultsKeys)
                {
                    if (this.calculationResults[obstacleListener].ContainsKey(changedObstacles))
                    {
                        this.calculationResults[obstacleListener][changedObstacles].AskCalculation();
                    }
                }
            }

            #endregion

            List<SquareObstacleFrustumCalculationResult> batchedSquareObstacleFrustumCalculationResult = null;
            foreach (var obstacleListener in this.calculationResults.Keys)
            {
                foreach (var frustumCalculationEntry in this.calculationResults[obstacleListener])
                {
                    if (frustumCalculationEntry.Value.CalculationAsked())
                    {
                        if (batchedSquareObstacleFrustumCalculationResult == null)
                        {
                            batchedSquareObstacleFrustumCalculationResult = new List<SquareObstacleFrustumCalculationResult>();
                        }
                        batchedSquareObstacleFrustumCalculationResult.Add(frustumCalculationEntry.Value);
                        if (batchedSquareObstacleFrustumCalculationResult.Count > ObstacleFrustumCalculationThread.MaxCalculationBatch)
                        {
                            this.ObstacleFrustumCalculationThreadObject.CalculationRequestedBatched(batchedSquareObstacleFrustumCalculationResult.ToArray());
                            batchedSquareObstacleFrustumCalculationResult.Clear();
                        }

                    }
                }
            }

            if (batchedSquareObstacleFrustumCalculationResult != null)
            {
                this.ObstacleFrustumCalculationThreadObject.CalculationRequestedBatched(batchedSquareObstacleFrustumCalculationResult.ToArray());
                batchedSquareObstacleFrustumCalculationResult.Clear();
            }

            Profiler.EndSample();

        }

        private void UpdateSquareObstaclesOfListener(ObstacleListener obstacleListener, bool async = true)
        {
            foreach (var nearObstacle in obstacleListener.NearSquereObstacles)
            {
                if (!this.calculationResults[obstacleListener].ContainsKey(nearObstacle))
                {
                    this.calculationResults[obstacleListener][nearObstacle] = new SquareObstacleFrustumCalculationResult(obstacleListener, nearObstacle);
                }
                this.calculationResults[obstacleListener][nearObstacle].AskCalculation();
                if (!async)
                {
                    this.calculationResults[obstacleListener][nearObstacle].DoCalculationFromDedicateThread();
                }
            }
        }

    }

    public class SquareObstacleFrustumCalculationResult
    {
        private List<FrustumPointsWorldPositions> calculatedFrustumPositions;
        private bool calculationAsked;

        #region References
        private ObstacleListener obstacleListenerRef;
        private SquareObstacle squareObstacleRef;
        #endregion

        #region Calculation Input
        private Vector3 worldPositionStartAngleDefinition;
        private Vector3 obstaclePosition;
        private Quaternion obstacleRotation;
        private Vector3 obstacleLossyScale;
        #endregion

        #region Logical Conditions
        public bool CalculationAsked()
        {
            return this.calculationAsked;
        }
        #endregion

        public SquareObstacleFrustumCalculationResult(ObstacleListener obstacleListenerRef, SquareObstacle squareObstacleRef)
        {
            this.obstacleListenerRef = obstacleListenerRef;
            this.squareObstacleRef = squareObstacleRef;
            this.calculatedFrustumPositions = new List<FrustumPointsWorldPositions>();
        }

        public List<FrustumPointsWorldPositions> CalculatedFrustumPositions { get => calculatedFrustumPositions; }
        public Vector3 ObstaclePosition { get => obstaclePosition; }
        public Quaternion ObstacleRotation { get => obstacleRotation; }
        public Vector3 ObstacleLossyScale { get => obstacleLossyScale; }
        public Vector3 WorldPositionStartAngleDefinition { get => worldPositionStartAngleDefinition; }

        public void SetResult(List<FrustumPointsWorldPositions> calculatedFrustumBufferData)
        {
            this.calculatedFrustumPositions = calculatedFrustumBufferData;
            this.calculationAsked = false;
        }

        public void AskCalculation()
        {
            this.calculationAsked = true;
            this.worldPositionStartAngleDefinition = this.obstacleListenerRef.transform.position;
            this.obstaclePosition = this.squareObstacleRef.transform.position;
            this.obstacleRotation = this.squareObstacleRef.transform.rotation;
            this.obstacleLossyScale = this.squareObstacleRef.transform.lossyScale;
        }

        public void DoCalculationFromDedicateThread()
        {
            this.SetResult(this.squareObstacleRef.ComputeOcclusionFrustums_FromDedicatedThread(this));
        }

        public override bool Equals(object obj)
        {
            var result = obj as SquareObstacleFrustumCalculationResult;
            return result != null &&
                   EqualityComparer<ObstacleListener>.Default.Equals(obstacleListenerRef, result.obstacleListenerRef) &&
                   EqualityComparer<SquareObstacle>.Default.Equals(squareObstacleRef, result.squareObstacleRef);
        }

        public override int GetHashCode()
        {
            var hashCode = -92224167;
            hashCode = hashCode * -1521134295 + EqualityComparer<ObstacleListener>.Default.GetHashCode(obstacleListenerRef);
            hashCode = hashCode * -1521134295 + EqualityComparer<SquareObstacle>.Default.GetHashCode(squareObstacleRef);
            return hashCode;
        }
    }

    #region Threading
    public class ObstacleFrustumCalculationThread
    {

        public const int MaxCalculationBatch = 50;

        public ObstacleFrustumCalculationThread()
        {

        }

        public void OnDestroy()
        {
        }

        public void CalculationRequestedBatched(SquareObstacleFrustumCalculationResult[] SquareObstacleFrustumCalculationResults)
        {
            Task.Run(() => this.DoCalculation(SquareObstacleFrustumCalculationResults));
        }

        private void DoCalculation(SquareObstacleFrustumCalculationResult[] calculations)
        {
            foreach (var calculation in calculations)
            {
                this.DoCalculation(calculation);
            }
        }

        private void DoCalculation(SquareObstacleFrustumCalculationResult calculation)
        {
            calculation.DoCalculationFromDedicateThread();
        }
    }
    #endregion
}
