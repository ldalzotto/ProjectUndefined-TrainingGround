using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObstacleFrustumCalculationManager : MonoBehaviour
{
    private Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>> calculationResults;

    public Dictionary<ObstacleListener, Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>> CalculationResults { get => calculationResults; }

    public void Init()
    {
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
        foreach (var obstacleListener in this.calculationResults.Keys.ToList())
        {
            if (obstacleListener.HasPositionChanged())
            {
                this.calculationResults[obstacleListener] = new Dictionary<SquareObstacle, SquareObstacleFrustumCalculationResult>();
                this.UpdateSquareObstaclesOfListener(obstacleListener);
            }
        }

        foreach (var obstacleListener in this.calculationResults.Keys)
        {
            foreach (var frustumCalculationEntry in this.calculationResults[obstacleListener])
            {
                if (frustumCalculationEntry.Value.CalculationAsked())
                {
                    frustumCalculationEntry.Value.SetResult(frustumCalculationEntry.Key.ComputeOcclusionFrustums(obstacleListener.transform.position));
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
    private ObstacleListener ObstacleListenerRef;
    private SquareObstacle SquareObstacleRef;
    #endregion

    #region Logical Conditions
    public bool CalculationAsked()
    {
        return this.calculationAsked;
    }
    #endregion

    public SquareObstacleFrustumCalculationResult(ObstacleListener obstacleListenerRef, SquareObstacle squareObstacleRef)
    {
        ObstacleListenerRef = obstacleListenerRef;
        SquareObstacleRef = squareObstacleRef;
    }

    public List<FrustumPointsWorldPositions> CalculatedFrustumPositions { get => calculatedFrustumPositions; }

    public void SetResult(List<FrustumPointsWorldPositions> calculatedFrustumBufferData)
    {
        this.calculatedFrustumPositions = calculatedFrustumBufferData;
        this.calculationAsked = false;
    }

    public void AskCalculation()
    {
        this.calculationAsked = true;
    }
}