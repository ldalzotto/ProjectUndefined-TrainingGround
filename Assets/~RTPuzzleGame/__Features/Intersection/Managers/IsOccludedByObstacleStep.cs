using CoreGame;
using RTPuzzle;
using Unity.Collections;
using UnityEngine;

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

        for (var FrustumPointsPositionsIndex = this.ObstacleFrustumPointsPositionsBeginIndex; FrustumPointsPositionsIndex < this.ObstacleFrustumPointsPositionsEndIndex; FrustumPointsPositionsIndex++)
        {
            if (Intersection.PointInsideFrustum(AssociatedObstacleFrustumPointsPositions[FrustumPointsPositionsIndex], PointWorldPosition))
            {
                return true;
            }
        }
        return false;
    }
}
