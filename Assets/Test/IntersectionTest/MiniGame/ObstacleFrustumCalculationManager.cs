using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Profiling;

public class ObstacleFrustumCalculationManager : MonoBehaviour
{
    #region External Dependencies
    private SquareObstaclesManager SquareObstaclesManager;
    #endregion

    private Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>> calculationResults;
    public Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>> CalculationResults { get => calculationResults; }

    private ObstacleFrustumCalculationThread ObstacleFrustumCalculationThreadObject;
    private Thread ObstacleFrustumCalculationThread;

    public void Init()
    {
        this.ObstacleFrustumCalculationThreadObject = new ObstacleFrustumCalculationThread();
        this.ObstacleFrustumCalculationThread = new Thread(new ThreadStart(this.ObstacleFrustumCalculationThreadObject.Main));
        this.ObstacleFrustumCalculationThread.IsBackground = true;
        this.ObstacleFrustumCalculationThread.Name = "ObstacleFrustumCalculationThread";
        this.ObstacleFrustumCalculationThread.Start();

        this.SquareObstaclesManager = GameObject.FindObjectOfType<SquareObstaclesManager>();

        this.calculationResults = new Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>>();
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

    public void Tick(float d)
    {
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

        foreach (var obstacleListener in this.calculationResults.Keys)
        {
            foreach (var frustumCalculationEntry in this.calculationResults[obstacleListener])
            {
                if (frustumCalculationEntry.Value.CalculationAsked())
                {
                    this.ObstacleFrustumCalculationThreadObject.CalculationRequested(frustumCalculationEntry.Value);
                  //  frustumCalculationEntry.Value.SetResult(frustumCalculationEntry.Key.ComputeOcclusionFrustums(obstacleListener.transform.position));
                }
            }
        }

    }

    private void UpdateSquareObstaclesOfListener(ObstacleListener obstacleListener)
    {
        foreach (var nearObstacle in obstacleListener.NearSquereObstacles)
        {
            if (!this.calculationResults[obstacleListener].ContainsKey(nearObstacle))
            {
                this.calculationResults[obstacleListener][nearObstacle] = new SquareObstacleFrustumCalculationResult(obstacleListener, nearObstacle);
            }
            this.calculationResults[obstacleListener][nearObstacle].AskCalculation();
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
    public Quaternion ObstacleRotation { get => obstacleRotation;  }
    public Vector3 ObstacleLossyScale { get => obstacleLossyScale;  }
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
    private CustomSampler sampler;

    public ObstacleFrustumCalculationThread()
    {
        this.sampler = CustomSampler.Create("ObstacleFrustumCalculationThread");
    }

    public void CalculationRequested(SquareObstacleFrustumCalculationResult SquareObstacleFrustumCalculationResult)
    {
        lock (this.frustumCalculations)
        {
            this.frustumCalculations[SquareObstacleFrustumCalculationResult.GetHashCode()] = SquareObstacleFrustumCalculationResult;
        }
    }

    private Dictionary<int, SquareObstacleFrustumCalculationResult> frustumCalculations = new Dictionary<int, SquareObstacleFrustumCalculationResult>();

    public void Main()
    {
        Profiler.BeginThreadProfiling("My threads", "ObstacleFrustumCalculationThread");
        while (true)
        {
            while (this.frustumCalculations.Count > 0)
            {
                this.sampler.Begin();
                DoCalculation();
                this.sampler.End();
            }
        }
    }

    private void DoCalculation()
    {
        SquareObstacleFrustumCalculationResult calculation = null;
        lock (this.frustumCalculations)
        {
            var e = this.frustumCalculations.Keys.GetEnumerator();
            e.MoveNext();
            var key = e.Current;
            calculation = this.frustumCalculations[key];
            this.frustumCalculations.Remove(key);
        }

        if (calculation != null)
        {
            calculation.DoCalculationFromDedicateThread();
        }
    }
}
#endregion