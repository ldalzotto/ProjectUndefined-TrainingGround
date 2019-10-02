using CoreGame;
using RTPuzzle;
using UnityEngine;
using UnityEngine.Profiling;

public static class RangeIntersectionOperations
{
    public static bool IsInside(SphereRangeObjectV2 SphereRangeObjectV2, BoxCollider InsideCollider)
    {
        return Intersection.BoxIntersectsOrEntirelyContainedInSphere(InsideCollider.center, InsideCollider.size, InsideCollider.transform.localToWorldMatrix,
          SphereRangeObjectV2.SphereBoundingCollider.transform.position,
          SphereRangeObjectV2.SphereBoundingCollider.radius);
    }

    public static bool IsInside(FrustumRangeObjectV2 FrustumRangeObjectV2, BoxCollider InsideCollider)
    {
        var FrustumWorldPositions = FrustumRangeObjectV2.GetFrustum().FrustumPointsPositions;
        return Intersection.FrustumBoxIntersection(FrustumWorldPositions, InsideCollider) || Intersection.BoxEntirelyContainedInFrustum(FrustumWorldPositions, InsideCollider);
    }

    public static bool IsInside(RoundedFrustumRangeObjectV2 RoundedFrustumRangeObjectV2, BoxCollider InsideCollder)
    {
        var FrustumWorldPositions = RoundedFrustumRangeObjectV2.GetFrustum().FrustumPointsPositions;
        return Intersection.BoxIntersectsOrEntirelyContainedInSphere(InsideCollder.center, InsideCollder.size, InsideCollder.transform.localToWorldMatrix, RoundedFrustumRangeObjectV2.RangeGameObjectV2.BoundingCollider.transform.position, RoundedFrustumRangeObjectV2.GetFrustum().GetFrustumFaceRadius())
            && (Intersection.FrustumBoxIntersection(FrustumWorldPositions, InsideCollder) || Intersection.BoxEntirelyContainedInFrustum(FrustumWorldPositions, InsideCollder));
    }

    public static bool IsOccluded(RangeObjectV2 RangeObjectV2, BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
    {
        return IsOccluded(RangeObjectV2.RangeGameObjectV2.ObstacleListener, boxCollider, forceObstacleOcclusionIfNecessary);
    }

    private static bool IsOccluded(ObstacleListener ObstacleListener, BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
    {
        return ObstacleListener != null && ObstacleListener.IsBoxOccludedByObstacles(boxCollider, forceObstacleOcclusionIfNecessary);
    }

    public static bool IsInsideAndNotOccluded(RangeObjectV2 RangeObject, BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
    {
        Profiler.BeginSample("ObstacleIsInsideAndNotOccluded");
        if (RangeObject.GetType() == typeof(SphereRangeObjectV2))
        {
            Profiler.EndSample();
            return IsInsideAndNotOccluded((SphereRangeObjectV2)RangeObject, boxCollider, forceObstacleOcclusionIfNecessary);
        }
        else
        {
            Profiler.EndSample();
            return false;
        }
    }

    private static bool IsInsideAndNotOccluded(SphereRangeObjectV2 SphereRangeObjectV2, BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
    {
        bool isInsideRange = RangeIntersectionOperations.IsInside(SphereRangeObjectV2, boxCollider);
        var obstacleListener = SphereRangeObjectV2.RangeGameObjectV2.ObstacleListener;
        if (obstacleListener != null && isInsideRange)
        {
            isInsideRange = isInsideRange && !RangeIntersectionOperations.IsOccluded(SphereRangeObjectV2, boxCollider, forceObstacleOcclusionIfNecessary);
        }
        return isInsideRange;
    }

    private static bool IsInsideAndNotOccluded(ObstacleListener ObstacleListener, bool isInside, BoxCollider boxCollider, bool forceObstacleOcclusionIfNecessary)
    {
        bool isInsideRange = isInside;
        if (ObstacleListener != null && isInsideRange)
        {
            isInsideRange = isInsideRange && !RangeIntersectionOperations.IsOccluded(ObstacleListener, boxCollider, forceObstacleOcclusionIfNecessary);
        }
        return isInsideRange;
    }
}
