using CoreGame;
using UnityEngine;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public static class RangeIntersectionOperations
    {
        public static bool IsInside(FrustumRangeObjectV2 FrustumRangeObjectV2, BoxCollider InsideCollider)
        {
            //TODO
            return false;
           // var FrustumWorldPositions = FrustumRangeObjectV2.GetFrustum().FrustumPointsPositions;
           // return Intersection.FrustumBoxIntersection(FrustumWorldPositions, InsideCollider) || Intersection.BoxEntirelyContainedInFrustum(FrustumWorldPositions, InsideCollider);
        }

        public static bool Isinside(FrustumRangeObjectV2 FrustumRangeObjectV2, BoxCollider InsideCollider)
        {
            //TODO
            return true;
          //  var FrustumWorldPositions = FrustumRangeObjectV2.GetFrustum().FrustumPointsPositions;
         //  return Intersection.FrustumBoxIntersection(FrustumWorldPositions, InsideCollider) || Intersection.BoxEntirelyContainedInFrustum(FrustumWorldPositions, InsideCollider);
        }

        public static bool IsInside(SphereRangeObjectV2 SphereRangeObjectV2, Vector3 worldPointComparison)
        {
            return Vector3.Distance(SphereRangeObjectV2.GetTransform().WorldPosition, worldPointComparison) <= SphereRangeObjectV2.SphereBoundingCollider.radius;
        }

        public static bool IsInside(RoundedFrustumRangeObjectV2 RoundedFrustumRangeObjectV2, Vector3 worldPointComparison)
        {
            return Vector3.Distance(RoundedFrustumRangeObjectV2.GetTransform().WorldPosition, worldPointComparison) <= RoundedFrustumRangeObjectV2.GetFrustum().GetFrustumFaceRadius()
                && Intersection.PointInsideFrustum(RoundedFrustumRangeObjectV2.GetFrustum(), worldPointComparison);
        }

        public static bool IsInsideAndNotOccluded(RangeObjectV2 RangeObject, Vector3 WorldPos, bool forceObstacleOcclusionIfNecessary)
        {
            Profiler.BeginSample("ObstacleIsInsideAndNotOccludedVector");
            bool isInside = false;
            if (RangeObject.GetType() == typeof(SphereRangeObjectV2))
            {
                isInside = IsInsideAndNotOccluded((SphereRangeObjectV2)RangeObject, WorldPos, forceObstacleOcclusionIfNecessary);
            }
            else if (RangeObject.GetType() == typeof(FrustumRangeObjectV2))
            {
                isInside = IsInsideAndNotOccluded((FrustumRangeObjectV2)RangeObject, WorldPos, forceObstacleOcclusionIfNecessary);
            }
            else if (RangeObject.GetType() == typeof(RoundedFrustumRangeObjectV2))
            {
                isInside = IsInsideAndNotOccluded((RoundedFrustumRangeObjectV2)RangeObject, WorldPos, forceObstacleOcclusionIfNecessary);
            }
            Profiler.EndSample();
            return isInside;
        }
        
        public static bool IsInsideAndNotOccluded(RoundedFrustumRangeObjectV2 RoundedFrustumRangeObjectV2, Vector3 WorldPos, bool forceObstacleOcclusionIfNecessary)
        {
            bool isInsideRange = RangeIntersectionOperations.IsInside(RoundedFrustumRangeObjectV2, WorldPos);
            return IsInsideAndNotOccluded(RoundedFrustumRangeObjectV2.GetObstacleListener(), isInsideRange, WorldPos, forceObstacleOcclusionIfNecessary);
        }

        private static bool IsInsideAndNotOccluded(SphereRangeObjectV2 SphereRangeObjectV2, Vector3 WorldPos, bool forceObstacleOcclusionIfNecessary)
        {
            bool isInsideRange = RangeIntersectionOperations.IsInside(SphereRangeObjectV2, WorldPos);
            return IsInsideAndNotOccluded(SphereRangeObjectV2.GetObstacleListener(), isInsideRange, WorldPos, forceObstacleOcclusionIfNecessary);
        }

        private static bool IsInsideAndNotOccluded(FrustumRangeObjectV2 FrustumRangeObjectV2, Vector3 WorldPos, bool forceObstacleOcclusionIfNecessary)
        {
            return false;
           // bool isInsideRange = RangeIntersectionOperations.IsInside(FrustumRangeObjectV2, WorldPos);
           // return IsInsideAndNotOccluded(FrustumRangeObjectV2.GetObstacleListener(), isInsideRange, WorldPos, forceObstacleOcclusionIfNecessary);
        }

        private static bool IsInsideAndNotOccluded(ObstacleListener ObstacleListener, bool isInside, Vector3 WorldPos, bool forceObstacleOcclusionIfNecessary)
        {
            return false;
            /*
            bool isInsideRange = isInside;
            if (ObstacleListener != null && isInsideRange)
            {
                isInsideRange = isInsideRange && !RangeIntersectionOperations.IsOccluded(ObstacleListener, WorldPos, forceObstacleOcclusionIfNecessary);
            }
            return isInsideRange;
            */
        }
    }

}
