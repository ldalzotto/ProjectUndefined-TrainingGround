using UnityEngine;

namespace CoreGame
{

    public class Intersection
    {

        #region BOX->SPHERE
        public static bool BoxIntersectsOrEntirelyContainedInSphere(BoxCollider boxCollider, Vector3 SphereWorldPosition, float SphereRadius)
        {
            return BoxEntirelyContainedInSphere(boxCollider, SphereWorldPosition, SphereRadius) || BoxIntersectsSphere(boxCollider, SphereWorldPosition, SphereRadius);
        }

        public static bool BoxIntersectsSphere(BoxCollider boxCollider, Vector3 SphereWorldPosition, float SphereRadius)
        {
            // -> Optimisations can be made in order to not trigger the full calcuation if objects are too far.
            // -> Also, order of faces can be sorted by distance check.
            ExtractBoxColliderWorldPoints(boxCollider, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

            //Face intersection
            return FaceIntersectSphere(BC6, BC8, BC5, BC7, SphereWorldPosition, SphereRadius)
                || FaceIntersectSphere(BC2, BC4, BC1, BC3, SphereWorldPosition, SphereRadius)
                || FaceIntersectSphere(BC6, BC8, BC2, BC4, SphereWorldPosition, SphereRadius)
                || FaceIntersectSphere(BC5, BC7, BC1, BC3, SphereWorldPosition, SphereRadius)
                || FaceIntersectSphere(BC6, BC2, BC5, BC1, SphereWorldPosition, SphereRadius)
                || FaceIntersectSphere(BC8, BC4, BC7, BC3, SphereWorldPosition, SphereRadius);
        }
        public static bool BoxEntirelyContainedInSphere(BoxCollider boxCollider, Vector3 SphereWorldPosition, float SphereRadius)
        {
            // -> Optimisations can be made in order to not trigger the full calcuation if objects are too far.
            // -> Also, order of faces can be sorted by distance check.
            ExtractBoxColliderWorldPoints(boxCollider, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);
            //Face contains
            return FaceEntirelyContainedInSphere(BC6, BC8, BC5, BC7, SphereWorldPosition, SphereRadius)
                || FaceEntirelyContainedInSphere(BC2, BC4, BC1, BC3, SphereWorldPosition, SphereRadius)
                || FaceEntirelyContainedInSphere(BC6, BC8, BC2, BC4, SphereWorldPosition, SphereRadius)
                || FaceEntirelyContainedInSphere(BC5, BC7, BC1, BC3, SphereWorldPosition, SphereRadius)
                || FaceEntirelyContainedInSphere(BC6, BC2, BC5, BC1, SphereWorldPosition, SphereRadius)
                || FaceEntirelyContainedInSphere(BC8, BC4, BC7, BC3, SphereWorldPosition, SphereRadius);
        }
        #endregion

        #region FRUSTUM<->POINT

        public static bool PointInsideFrustum(FrustumV2 frustum, Vector3 worldPositionPoint)
        {
#warning PointInsideFrustum trigger frustum points recalculation
            frustum.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
            return PointInsideFrustumComputation(worldPositionPoint, C1, C2, C3, C4, C5, C6, C7, C8);
        }

        public static bool PointInsideFrustum(FrustumPointsPositions FrustumPointsWorldPositions, Vector3 worldPositionPoint)
        {
            return PointInsideFrustumComputation(worldPositionPoint, FrustumPointsWorldPositions.FC1, FrustumPointsWorldPositions.FC2, FrustumPointsWorldPositions.FC3
                , FrustumPointsWorldPositions.FC4, FrustumPointsWorldPositions.FC5, FrustumPointsWorldPositions.FC6, FrustumPointsWorldPositions.FC7, FrustumPointsWorldPositions.FC8);
        }

        private static bool PointInsideFrustumComputation(Vector3 worldPositionPoint, Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4, Vector3 C5, Vector3 C6, Vector3 C7, Vector3 C8)
        {
            bool pointInsideFrustum = false;

            float crossSign = Mathf.Sign(Vector3.Dot(C5 - C1, Vector3.Cross(C2 - C1, C4 - C1)));
            Vector3 normal = Vector3.zero;
            normal = crossSign * Vector3.Cross(C2 - C1, C3 - C1);
            pointInsideFrustum = (Vector3.Dot(normal, worldPositionPoint - C1) >= 0) && (Vector3.Dot(normal, C5 - C1) > 0);

            if (pointInsideFrustum)
            {
                normal = crossSign * Vector3.Cross(C5 - C1, C2 - C1);
                pointInsideFrustum = (Vector3.Dot(normal, worldPositionPoint - C1) >= 0) && (Vector3.Dot(normal, C4 - C1) > 0);

                if (pointInsideFrustum)
                {
                    normal = crossSign * Vector3.Cross(C6 - C2, C3 - C2);
                    pointInsideFrustum = (Vector3.Dot(normal, worldPositionPoint - C2) >= 0) && (Vector3.Dot(normal, C1 - C2) > 0);

                    if (pointInsideFrustum)
                    {
                        normal = crossSign * Vector3.Cross(C7 - C3, C4 - C3);
                        pointInsideFrustum = (Vector3.Dot(normal, worldPositionPoint - C3) >= 0) && (Vector3.Dot(normal, C2 - C3) > 0);

                        if (pointInsideFrustum)
                        {
                            normal = crossSign * Vector3.Cross(C8 - C4, C1 - C4);
                            pointInsideFrustum = (Vector3.Dot(normal, worldPositionPoint - C4) >= 0) && (Vector3.Dot(normal, C3 - C4) > 0);

                            if (pointInsideFrustum)
                            {
                                normal = crossSign * Vector3.Cross(C8 - C5, C6 - C5);
                                pointInsideFrustum = (Vector3.Dot(normal, worldPositionPoint - C5) >= 0) && (Vector3.Dot(normal, C1 - C5) > 0);
                            }
                        }
                    }
                }
            }

            return pointInsideFrustum;
        }

        public static bool FaceIntersectSphere(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4, Vector3 SphereWorldPosition, float SphereRadius)
        {
            bool planeIntersected = false;
            bool edgeIntersected = false;

            // (1) - We check if edges cross the sphere 
            edgeIntersected
                = SegmentSphereIntersection(C1, (C2 - C1).normalized, Vector3.Distance(C2, C1), SphereWorldPosition, SphereRadius)
               || SegmentSphereIntersection(C1, (C3 - C1).normalized, Vector3.Distance(C3, C1), SphereWorldPosition, SphereRadius)
               || SegmentSphereIntersection(C3, (C4 - C3).normalized, Vector3.Distance(C4, C3), SphereWorldPosition, SphereRadius)
               || SegmentSphereIntersection(C2, (C4 - C2).normalized, Vector3.Distance(C4, C2), SphereWorldPosition, SphereRadius);

            // (2) - If edges doesn't cross the sphere, we try to find the intersection from sphere to cube face plane //http://www.ambrsoft.com/TrigoCalc/Sphere/SpherePlaneIntersection_.htm
            // Intersection is valid only if intercention circle center is contained insibe cube face https://math.stackexchange.com/a/190373
            if (!edgeIntersected)
            {
                Vector3 normal = Vector3.Cross(C2 - C1, C3 - C1);
                float a = normal.x;
                float b = normal.y;
                float c = normal.z;
                float d = -1 * (a * C1.x + b * C1.y + c * C1.z);

                float nom = (a * SphereWorldPosition.x + b * SphereWorldPosition.y + c * SphereWorldPosition.z + d);
                float denom = (a * a) + (b * b) + (c * c);

                Vector3 planeIntersectionPoint = new Vector3(
                     SphereWorldPosition.x - (a * nom / denom),
                     SphereWorldPosition.y - (b * nom / denom),
                     SphereWorldPosition.z - (c * nom / denom)
                    );

                if (Vector3.Distance(planeIntersectionPoint, SphereWorldPosition) <= SphereRadius)
                {
                    planeIntersected = Vector3.Dot(C2 - C1, C2 - C1) > Vector3.Dot(planeIntersectionPoint - C1, C2 - C1) && Vector3.Dot(planeIntersectionPoint - C1, C2 - C1) > 0
                        && Vector3.Dot(C3 - C1, C3 - C1) > Vector3.Dot(planeIntersectionPoint - C1, C3 - C1) && Vector3.Dot(planeIntersectionPoint - C1, C3 - C1) > 0;
                }
            }

            return (planeIntersected || edgeIntersected);
        }

        public static bool FaceEntirelyContainedInSphere(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4, Vector3 SphereWorldPosition, float SphereRadius)
        {
            bool edgeContained = false;

            // (1) - We check if edges are fully contained inside
            edgeContained
                = SegmentEntirelyContainedInSphere(C1, (C2 - C1).normalized, Vector3.Distance(C2, C1), SphereWorldPosition, SphereRadius)
               || SegmentEntirelyContainedInSphere(C1, (C3 - C1).normalized, Vector3.Distance(C3, C1), SphereWorldPosition, SphereRadius)
               || SegmentEntirelyContainedInSphere(C3, (C4 - C3).normalized, Vector3.Distance(C4, C3), SphereWorldPosition, SphereRadius)
               || SegmentEntirelyContainedInSphere(C2, (C4 - C2).normalized, Vector3.Distance(C4, C2), SphereWorldPosition, SphereRadius);

            return edgeContained;
        }

        public static bool SegmentSphereIntersection(Vector3 segmentOrigin, Vector3 segmentDirection, float segmentDistance, Vector3 sphereCenterPoint, float sphereRadius)
        {
            // Segment sphere intersection https://en.wikipedia.org/wiki/Line%E2%80%93sphere_intersection

            float a = -1 * (Vector3.Dot(segmentDirection, segmentOrigin - sphereCenterPoint));
            float b = Mathf.Pow(-1 * a, 2) - ((segmentOrigin - sphereCenterPoint).sqrMagnitude - (sphereRadius * sphereRadius));

            bool intersect = false;

            if (b == 0)
            {
                float d = a;
                intersect = (d > 0 && d < segmentDistance);
            }
            if (b >= 0)
            {
                float d = a + Mathf.Sqrt(b);
                intersect = (d > 0 && d < segmentDistance);
                if (!intersect)
                {
                    d = a - Mathf.Sqrt(b);
                    intersect = (d > 0 && d < segmentDistance);
                }
            }

            return intersect;
        }

        public static bool SegmentEntirelyContainedInSphere(Vector3 segmentOrigin, Vector3 segmentDirection, float segmentDistance, Vector3 sphereCenterPoint, float sphereRadius)
        {
            return Vector3.Distance(segmentOrigin, sphereCenterPoint) <= sphereRadius && Vector3.Distance(segmentOrigin + (segmentDirection * segmentDistance), sphereCenterPoint) <= sphereRadius;
        }

        #endregion


        #region FRUSTUM<->BOX
        public static bool BoxEntirelyContainedInFrustum(FrustumPointsPositions frustumPoints, BoxCollider boxCollider)
        {
            ExtractBoxColliderWorldPoints(boxCollider, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);
            return (PointInsideFrustumComputation(BC1, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8)
                && PointInsideFrustumComputation(BC2, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8)
                && PointInsideFrustumComputation(BC3, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8)
                && PointInsideFrustumComputation(BC4, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8)
                && PointInsideFrustumComputation(BC5, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8)
                && PointInsideFrustumComputation(BC6, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8)
                && PointInsideFrustumComputation(BC7, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8)
                && PointInsideFrustumComputation(BC8, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8));
        }

        public static bool FrustumBoxIntersection(FrustumPointsPositions frustumPoints, BoxCollider boxCollider)
        {
            ExtractBoxColliderWorldPoints(boxCollider, out Vector3 BC1, out Vector3 BC2, out Vector3 BC3, out Vector3 BC4, out Vector3 BC5, out Vector3 BC6, out Vector3 BC7, out Vector3 BC8);

            if (!LineFrustumIntersection(BC1, BC2, frustumPoints) && !LineFrustumIntersection(BC2, BC3, frustumPoints) && !LineFrustumIntersection(BC3, BC4, frustumPoints) && !LineFrustumIntersection(BC4, BC1, frustumPoints)
                && !LineFrustumIntersection(BC1, BC5, frustumPoints) && !LineFrustumIntersection(BC2, BC6, frustumPoints) && !LineFrustumIntersection(BC4, BC8, frustumPoints) && !LineFrustumIntersection(BC3, BC7, frustumPoints)
                && !LineFrustumIntersection(BC8, BC7, frustumPoints) && !LineFrustumIntersection(BC7, BC6, frustumPoints) && !LineFrustumIntersection(BC6, BC5, frustumPoints) && !LineFrustumIntersection(BC5, BC8, frustumPoints))
            {
                return false;
            }
            return true;
        }
        #endregion

        public static bool LineFrustumIntersection(Vector3 lineOrigin, Vector3 lineEnd, FrustumPointsPositions frustumPoints)
        {
            //First face for now
            float crossSign = Mathf.Sign(Vector3.Dot(frustumPoints.FC5 - frustumPoints.FC1, Vector3.Cross(frustumPoints.FC2 - frustumPoints.FC1, frustumPoints.FC4 - frustumPoints.FC1)));
            Vector3 normal = crossSign * Vector3.Cross(frustumPoints.FC2 - frustumPoints.FC1, frustumPoints.FC3 - frustumPoints.FC1);
            if (!SegmentAccuratePlaneIntersection(lineOrigin, lineEnd, frustumPoints.FC1, frustumPoints.FC2, frustumPoints.FC3, frustumPoints.FC4, normal))
            {
                normal = crossSign * Vector3.Cross(frustumPoints.FC5 - frustumPoints.FC1, frustumPoints.FC2 - frustumPoints.FC1);
                if (!SegmentAccuratePlaneIntersection(lineOrigin, lineEnd, frustumPoints.FC1, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC2, normal))
                {
                    normal = crossSign * Vector3.Cross(frustumPoints.FC6 - frustumPoints.FC2, frustumPoints.FC3 - frustumPoints.FC2);
                    if (!SegmentAccuratePlaneIntersection(lineOrigin, lineEnd, frustumPoints.FC2, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC3, normal))
                    {
                        normal = crossSign * Vector3.Cross(frustumPoints.FC7 - frustumPoints.FC3, frustumPoints.FC4 - frustumPoints.FC3);
                        if (!SegmentAccuratePlaneIntersection(lineOrigin, lineEnd, frustumPoints.FC3, frustumPoints.FC7, frustumPoints.FC8, frustumPoints.FC4, normal))
                        {
                            normal = crossSign * Vector3.Cross(frustumPoints.FC8 - frustumPoints.FC4, frustumPoints.FC1 - frustumPoints.FC4);
                            if (!SegmentAccuratePlaneIntersection(lineOrigin, lineEnd, frustumPoints.FC4, frustumPoints.FC8, frustumPoints.FC5, frustumPoints.FC1, normal))
                            {
                                normal = crossSign * Vector3.Cross(frustumPoints.FC8 - frustumPoints.FC5, frustumPoints.FC6 - frustumPoints.FC5);
                                if (!SegmentAccuratePlaneIntersection(lineOrigin, lineEnd, frustumPoints.FC5, frustumPoints.FC6, frustumPoints.FC7, frustumPoints.FC8, -normal))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        //http://geomalgorithms.com/a05-_intersect-1.html
        /*
         * 
             // intersect3D_SegmentPlane(): find the 3D intersection of a segment and a plane
    //    Input:  S = a segment, and Pn = a plane = {Point V0;  Vector n;}
    //    Output: *I0 = the intersect point (when it exists)
    //    Return: 0 = disjoint (no intersection)
    //            1 =  intersection in the unique point *I0
    //            2 = the  segment lies in the plane
    int
    intersect3D_SegmentPlane( Segment S, Plane Pn, Point* I )
    {
        Vector    u = S.P1 - S.P0;
        Vector    w = S.P0 - Pn.V0;

        float     D = dot(Pn.n, u);
        float     N = -dot(Pn.n, w);

        if (fabs(D) < SMALL_NUM) {           // segment is parallel to plane
            if (N == 0)                      // segment lies in plane
                return 2;
            else
                return 0;                    // no intersection
        }
        // they are not parallel
        // compute intersect param
        float sI = N / D;
        if (sI < 0 || sI > 1)
            return 0;                        // no intersection

        *I = S.P0 + sI * u;                  // compute segment intersect point
        return 1;
    }
    //===================================================================
        */
        private static bool SegmentAccuratePlaneIntersection(Vector3 segmentStart, Vector3 segmentEnd, Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4, Vector3 insideNormal)
        {

            Vector3 u = segmentEnd - segmentStart;
            Vector3 w = segmentStart - C1;

            float D = Vector3.Dot(insideNormal, u);
            float N = -Vector3.Dot(insideNormal, w);

            //if segment and plane are //

            if (Mathf.Abs(D) == 0)
            {
                return false;
            }

            //If lines extremities are in the inside normal side

            float sI = N / D;
            if (sI < 0 || sI > 1)
            {
                return false;
            }

            Vector3 I = segmentStart + (sI * u);

            //If the intersection point is outside of the plane
            //We project intersection point to normal
            Vector3 normal12 = Vector3.Cross(C2 - C1, insideNormal);
            Vector3 normal23 = Vector3.Cross(C3 - C2, insideNormal);
            Vector3 normal34 = Vector3.Cross(C4 - C3, insideNormal);
            Vector3 normal41 = Vector3.Cross(C1 - C4, insideNormal);

            if (Vector3.Dot(normal12, I - C1) < 0 || Vector3.Dot(normal23, I - C2) < 0 || Vector3.Dot(normal34, I - C3) < 0 || Vector3.Dot(normal41, I - C4) < 0)
            {
                return false;
            }

            return true;
        }

        public static void ExtractBoxColliderWorldPoints(BoxCollider boxCollider, out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8)
        {
            Vector3 diagDirection = Vector3.zero;
            Vector3 boxColliderSize = boxCollider.size;

            diagDirection = diagDirection.SetVector(-boxColliderSize.x, boxColliderSize.y, -boxColliderSize.z);
            C1 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

            diagDirection = diagDirection.SetVector(boxColliderSize.x, boxColliderSize.y, -boxColliderSize.z);
            C2 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

            diagDirection = diagDirection.SetVector(-boxColliderSize.x, -boxColliderSize.y, -boxColliderSize.z);
            C3 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

            diagDirection = diagDirection.SetVector(boxColliderSize.x, -boxColliderSize.y, -boxColliderSize.z);
            C4 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

            diagDirection = diagDirection.SetVector(-boxColliderSize.x, boxColliderSize.y, boxColliderSize.z);
            C5 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

            diagDirection = diagDirection.SetVector(boxColliderSize.x, boxColliderSize.y, boxColliderSize.z);
            C6 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

            diagDirection = diagDirection.SetVector(-boxColliderSize.x, -boxColliderSize.y, boxColliderSize.z);
            C7 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

            diagDirection = diagDirection.SetVector(boxColliderSize.x, -boxColliderSize.y, boxColliderSize.z);
            C8 = boxCollider.transform.TransformPoint(boxCollider.center + diagDirection / 2f).Round(3);

        }

    }

}
