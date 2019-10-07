using CoreGame;
using System.Collections.Generic;
using UnityEngine;

public static class RangeIntersectionSinglePointBlittable
{

    public static bool IsBoxFullyOccludedByObstacle(BoxDefinition BoxDefinition, List<FrustumPointsPositions> OcclusionFrustumsPositions)
    {
        Intersection.ExtractBoxColliderWorldPointsV2(BoxDefinition.LocalCenter, BoxDefinition.LocalSize, BoxDefinition.LocalToWorld, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

        return IsPointFullyOccludedByObstacle(BC1, OcclusionFrustumsPositions) &&
               IsPointFullyOccludedByObstacle(BC2, OcclusionFrustumsPositions) &&
               IsPointFullyOccludedByObstacle(BC3, OcclusionFrustumsPositions) &&
               IsPointFullyOccludedByObstacle(BC4, OcclusionFrustumsPositions) &&
               IsPointFullyOccludedByObstacle(BC5, OcclusionFrustumsPositions) &&
               IsPointFullyOccludedByObstacle(BC6, OcclusionFrustumsPositions) &&
               IsPointFullyOccludedByObstacle(BC7, OcclusionFrustumsPositions) &&
               IsPointFullyOccludedByObstacle(BC8, OcclusionFrustumsPositions);
    }

    public static bool IsPointFullyOccludedByObstacle(Vector3 PointWorldPosition, List<FrustumPointsPositions> OcclusionFrustumsPositions)
    {
        bool isOccluded = true;
        foreach (var occlusionFrustumPositions in OcclusionFrustumsPositions)
        {
            isOccluded = isOccluded && Intersection.PointInsideFrustum(occlusionFrustumPositions, PointWorldPosition);
            if (!isOccluded) { break; }
        }

        return isOccluded;
    }

}


public struct BoxDefinition
{
    public Vector3 LocalCenter;
    public Vector3 LocalSize;
    public Matrix4x4 LocalToWorld;
}