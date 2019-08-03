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
                ForceCalculationIfNecessary(ObstacleListener);

                foreach (var obstacleResultEntry in this.calculationResults[ObstacleListener].Values)
                {
                    foreach (var calculatedFrustumPosition in obstacleResultEntry.CalculatedFrustumPositions)
                    {
                        if (Intersection.PointInsideFrustum(calculatedFrustumPosition, worldPositionPoint))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //A boxcollider is considered occluded by obstacles if it's eight corner points are occluded by obstacle frustums
        public bool IsPointOccludedByObstacles(ObstacleListener ObstacleListener, BoxCollider boxCollider)
        {
            this.ForceCalculationIfNecessary(ObstacleListener);

            Intersection.ExtractBoxColliderWorldPoints(boxCollider, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);
            var boxPointsOcclusionStatus = new List<BoxPointOccludedStatus>() {
                new BoxPointOccludedStatus(BC1),new BoxPointOccludedStatus(BC2),new BoxPointOccludedStatus(BC3),new BoxPointOccludedStatus(BC4),new BoxPointOccludedStatus(BC5),new BoxPointOccludedStatus(BC6)
                ,new BoxPointOccludedStatus(BC7),new BoxPointOccludedStatus(BC8)
            };

            foreach (var obstacleResultEntry in this.calculationResults[ObstacleListener].Values)
            {
                foreach (var calculatedFrustumPosition in obstacleResultEntry.CalculatedFrustumPositions)
                {
                    foreach (var boxPointOcclusionStatus in boxPointsOcclusionStatus)
                    {
                        if (!boxPointOcclusionStatus.IsOccluded && Intersection.PointInsideFrustum(calculatedFrustumPosition, boxPointOcclusionStatus.WorldPos))
                        {
                            boxPointOcclusionStatus.IsOccluded = true;
                        }
                    }
                }
            }

            foreach (var boxPointOcclusionStatus in boxPointsOcclusionStatus)
            {
                //At least one point is not occluded -> box is visible
                if (!boxPointOcclusionStatus.IsOccluded)
                {
                    return false;
                }
            }

            return true;
        }

        class BoxPointOccludedStatus
        {
            private Vector3 worldPos;
            private bool isOccluded;

            public BoxPointOccludedStatus(Vector3 worldPos)
            {
                this.worldPos = worldPos;
                this.isOccluded = false;
            }

            public Vector3 WorldPos { get => worldPos; }
            public bool IsOccluded { get => isOccluded; set => isOccluded = value; }
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

            foreach (var changedObstacles in this.SquareObstaclesManager.ConsumeLastFrameChangedObstacles())
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
            foreach (var nearObstacle in obstacleListener.NearSquareObstacles)
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

        private void ForceCalculationIfNecessary(ObstacleListener ObstacleListener)
        {
            //If calculation is waiting, we trigger sync calculation
            if (this.calculationResults[ObstacleListener].Values.Count == 0 || this.calculationResults[ObstacleListener].Values.ToList().Select(result => result).Where(result => result.CalculationAsked()).Count() > 0)
            {
                //Debug.Log(MyLog.Format("Forced obstacle listener calculation : " + ObstacleListener.name));
                this.UpdateSquareObstaclesOfListener(ObstacleListener, async: false);
            }
        }
    }

    public class SquareObstacleFrustumCalculationResult
    {
        private List<FrustumPointsPositions> calculatedFrustumPositions;
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
            this.calculatedFrustumPositions = new List<FrustumPointsPositions>();
        }

        public List<FrustumPointsPositions> CalculatedFrustumPositions { get => calculatedFrustumPositions; }
        public Vector3 ObstaclePosition { get => obstaclePosition; }
        public Quaternion ObstacleRotation { get => obstacleRotation; }
        public Vector3 ObstacleLossyScale { get => obstacleLossyScale; }
        public Vector3 WorldPositionStartAngleDefinition { get => worldPositionStartAngleDefinition; }

        public void SetResult(List<FrustumPointsPositions> calculatedFrustumBufferData)
        {
            this.calculatedFrustumPositions = calculatedFrustumBufferData;
            this.calculationAsked = false;
        }

        public void AskCalculation()
        {
            // Debug.Log(MyLog.Format("ObstacleFrustumCalculation asked. Listener : " + this.obstacleListenerRef.name + " obstacle : " + this.squareObstacleRef.name));
            this.calculationAsked = true;
            this.worldPositionStartAngleDefinition = this.obstacleListenerRef.transform.position;
            this.obstaclePosition = this.squareObstacleRef.GetPosition();
            this.obstacleRotation = this.squareObstacleRef.GetRotation();
            this.obstacleLossyScale = this.squareObstacleRef.GetLossyScale();
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
