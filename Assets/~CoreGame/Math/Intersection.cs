using UnityEngine;

namespace CoreGame
{


    public class Intersection
    {

        #region BOX<->SPHERE

        public static bool BoxIntersectsSphereV2(BoxCollider boxCollider, Vector3 SphereWorldPosition, float SphereRadius)
        {
            bool boxIntersectSphere = false;
            Vector3 diagDirection = new Vector3();
            Vector3 boxColliderSize = boxCollider.size;
            Vector3 boxColliderWorldPosition = boxCollider.transform.position;
            Quaternion boxColliderRotation = boxCollider.transform.rotation;
            Vector3 rotatedBoxColliderCenter = boxColliderRotation * boxCollider.center;

            //TODO -> Optimisations can be made in order to not trigger the full calcuation if objects are too far.
            //TODO -> Also, order of faces can be sorted by distance check.

            SetVector(ref diagDirection, boxColliderSize.x, boxColliderSize.y, boxColliderSize.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C1);

            SetVector(ref diagDirection, boxColliderSize.x, -boxColliderSize.y, boxColliderSize.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C2);

            SetVector(ref diagDirection, -boxColliderSize.x, boxColliderSize.y, boxColliderSize.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C3);

            SetVector(ref diagDirection, -boxColliderSize.x, -boxColliderSize.y, boxColliderSize.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C4);

            //Face intersection
            boxIntersectSphere = FaceIntersectOrContainsSphere(C1, C2, C3, C4, SphereWorldPosition, SphereRadius);

            if (!boxIntersectSphere)
            {

                SetVector(ref diagDirection, boxColliderSize.x, boxColliderSize.y, -boxColliderSize.z);
                diagDirection.Scale(boxCollider.transform.lossyScale);
                BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C5);

                SetVector(ref diagDirection, boxColliderSize.x, -boxColliderSize.y, -boxColliderSize.z);
                diagDirection.Scale(boxCollider.transform.lossyScale);
                BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C6);

                SetVector(ref diagDirection, -boxColliderSize.x, boxColliderSize.y, -boxColliderSize.z);
                diagDirection.Scale(boxCollider.transform.lossyScale);
                BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C7);

                SetVector(ref diagDirection, -boxColliderSize.x, -boxColliderSize.y, -boxColliderSize.z);
                diagDirection.Scale(boxCollider.transform.lossyScale);
                BoxPointCalculationV2(boxColliderWorldPosition, boxColliderRotation, rotatedBoxColliderCenter, diagDirection / 2f, out Vector3 C8);

                boxIntersectSphere = FaceIntersectOrContainsSphere(C5, C6, C7, C8, SphereWorldPosition, SphereRadius);

                if (!boxIntersectSphere)
                {
                    boxIntersectSphere = FaceIntersectOrContainsSphere(C1, C2, C5, C6, SphereWorldPosition, SphereRadius);
                    if (!boxIntersectSphere)
                    {
                        boxIntersectSphere = FaceIntersectOrContainsSphere(C3, C4, C7, C8, SphereWorldPosition, SphereRadius);
                        if (!boxIntersectSphere)
                        {
                            boxIntersectSphere = FaceIntersectOrContainsSphere(C1, C5, C3, C7, SphereWorldPosition, SphereRadius);
                            if (!boxIntersectSphere)
                            {
                                boxIntersectSphere = FaceIntersectOrContainsSphere(C2, C6, C4, C8, SphereWorldPosition, SphereRadius);
                            }
                        }
                    }
                }
            }

            return boxIntersectSphere;
        }

        public static void BoxPointCalculationV2(Vector3 boxColliderWorldPosition, Quaternion boxColliderRotation, Vector3 rotatedBoxColliderCenter, Vector3 diagonalDirection, out Vector3 C1)
        {
            C1 = boxColliderWorldPosition + boxColliderRotation * (diagonalDirection) + rotatedBoxColliderCenter;
            C1 = new Vector3((float)(System.Math.Round((double)C1.x, 3)), (float)(System.Math.Round((double)C1.y, 3)), (float)(System.Math.Round((double)C1.z, 3)));
        }

        #endregion

        #region FRUSTUM<->POINT
        //This method trigger frustum points recalculation
        public static bool PointInsideFrustum(FrustumV2 frustum, Vector3 worldPositionPoint)
        {
            frustum.CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8);
            return PointInsideFrustumComputation(worldPositionPoint, C1, C2, C3, C4, C5, C6, C7, C8);
        }

        public static bool PointInsideFrustum(FrustumPointsWorldPositions FrustumPointsWorldPositions, Vector3 worldPositionPoint)
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


        #endregion

        /*
        #region FRUSTUM<->SPHERE

        public static bool FrustumInteresectsSphere(FrustumV2 frustum, Vector3 SphereWorldPosition, float SphereRadius)
        {
            bool frustumIntersectSphere = false;
            Vector3 rotatedFrustumCenter = frustum.Rotation * frustum.Center;
            Vector3 diagDirection = new Vector3();

            diagDirection = frustum.F1.FaceOffsetFromCenter + new Vector3(-frustum.F1.Width / 2, frustum.F1.Height / 2, 0);
            diagDirection.Scale(frustum.LossyScale);
            Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C1);

            diagDirection = frustum.F1.FaceOffsetFromCenter + new Vector3(frustum.F1.Width / 2, frustum.F1.Height / 2, 0);
            diagDirection.Scale(frustum.LossyScale);
            Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C2);

            diagDirection = frustum.F1.FaceOffsetFromCenter + new Vector3(frustum.F1.Width / 2, -frustum.F1.Height / 2, 0);
            diagDirection.Scale(frustum.LossyScale);
            Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C3);

            diagDirection = frustum.F1.FaceOffsetFromCenter + new Vector3(-frustum.F1.Width / 2, -frustum.F1.Height / 2, 0);
            diagDirection.Scale(frustum.LossyScale);
            Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C4);
            
            frustumIntersectSphere = FaceIntersectOrContainsSphere(C1, C2, C3, C4, SphereWorldPosition, SphereRadius);
            if (!frustumIntersectSphere)
            {
                diagDirection = frustum.F2.FaceOffsetFromCenter + new Vector3(-frustum.F2.Width / 2, frustum.F2.Height / 2, 0);
                diagDirection.Scale(frustum.LossyScale);
                Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C5);

                diagDirection = frustum.F2.FaceOffsetFromCenter + new Vector3(frustum.F2.Width / 2, frustum.F2.Height / 2, 0);
                diagDirection.Scale(frustum.LossyScale);
                Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C6);

                diagDirection = frustum.F2.FaceOffsetFromCenter + new Vector3(frustum.F2.Width / 2, -frustum.F2.Height / 2, 0);
                diagDirection.Scale(frustum.LossyScale);
                Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C7);

                diagDirection = frustum.F2.FaceOffsetFromCenter + new Vector3(-frustum.F2.Width / 2, -frustum.F2.Height / 2, 0);
                diagDirection.Scale(frustum.LossyScale);
                Intersection.BoxPointCalculationV2(frustum.WorldPosition, frustum.Rotation, rotatedFrustumCenter, diagDirection / 2f, out Vector3 C8);


                frustumIntersectSphere = FaceIntersectOrContainsSphere(C1, C5, C6, C2, SphereWorldPosition, SphereRadius);
                if (!frustumIntersectSphere)
                {
                    frustumIntersectSphere = FaceIntersectOrContainsSphere(C2, C6, C7, C3, SphereWorldPosition, SphereRadius);
                    if (!frustumIntersectSphere)
                    {
                        frustumIntersectSphere = FaceIntersectOrContainsSphere(C4, C8, C7, C3, SphereWorldPosition, SphereRadius);
                        if (!frustumIntersectSphere)
                        {
                            frustumIntersectSphere = FaceIntersectOrContainsSphere(C4, C1, C5, C8, SphereWorldPosition, SphereRadius);
                            if (!frustumIntersectSphere)
                            {
                                frustumIntersectSphere = FaceIntersectOrContainsSphere(C5, C6, C7, C8, SphereWorldPosition, SphereRadius);
                            }
                        }
                    }
                }
            }

            return frustumIntersectSphere;

        }

        #endregion
        */
        public static bool FaceIntersectOrContainsSphere(Vector3 C1, Vector3 C2, Vector3 C3, Vector3 C4, Vector3 SphereWorldPosition, float SphereRadius)
        {
            bool planeIntersected = false;
            bool edgeIntersectedOrContained = false;

            // (1) - We check if edges cross the sphere or are fully contained inside
            edgeIntersectedOrContained = LineSphereIntersection(C1, (C2 - C1).normalized, Vector3.Distance(C2, C1), SphereWorldPosition, SphereRadius);
            if (!edgeIntersectedOrContained)
            {
                edgeIntersectedOrContained = LineSphereIntersection(C1, (C3 - C1).normalized, Vector3.Distance(C3, C1), SphereWorldPosition, SphereRadius);
            }
            if (!edgeIntersectedOrContained)
            {
                edgeIntersectedOrContained = LineSphereIntersection(C3, (C4 - C3).normalized, Vector3.Distance(C4, C3), SphereWorldPosition, SphereRadius);
            }
            if (!edgeIntersectedOrContained)
            {
                edgeIntersectedOrContained = LineSphereIntersection(C2, (C4 - C2).normalized, Vector3.Distance(C4, C2), SphereWorldPosition, SphereRadius);
            }

            // (2) - If edges doesn't cross the sphere, we try to find the intersection from sphere to cube face plane //http://www.ambrsoft.com/TrigoCalc/Sphere/SpherePlaneIntersection_.htm
            // Intersection is valid only if intercention circle center is contained insibe cube face https://math.stackexchange.com/a/190373
            if (!edgeIntersectedOrContained)
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



            return (planeIntersected || edgeIntersectedOrContained);
        }

        public static bool LineSphereIntersection(Vector3 lineOrigin, Vector3 lineDirection, float lineDistance, Vector3 sphereCenterPoint, float sphereRadius)
        {
            // Line sphere intersection https://en.wikipedia.org/wiki/Line%E2%80%93sphere_intersection

            float a = -1 * (Vector3.Dot(lineDirection, lineOrigin - sphereCenterPoint));
            float b = Mathf.Pow(-1 * a, 2) - ((lineOrigin - sphereCenterPoint).sqrMagnitude - (sphereRadius * sphereRadius));

            bool intersect = false;
            bool contained = false;

            if (b == 0)
            {
                float d = a;
                intersect = (d > 0 && d < lineDistance);
            }
            if (b >= 0)
            {
                float d = a + Mathf.Sqrt(b);
                intersect = (d > 0 && d < lineDistance);
                if (!intersect)
                {
                    d = a - Mathf.Sqrt(b);
                    intersect = (d > 0 && d < lineDistance);
                }
            }

            // If line is not intersecting, we check if line is fully contained inside sphere
            if (!intersect)
            {
                contained = Vector3.Distance(lineOrigin, sphereCenterPoint) <= sphereRadius && Vector3.Distance(lineOrigin + (lineDirection * lineDistance), sphereCenterPoint) <= sphereRadius;
            }

            return intersect || contained;
        }


        private static void SetVector(ref Vector3 vector, float x, float y, float z)
        {
            vector.x = x;
            vector.y = y;
            vector.z = z;
        }
        private static void SetVector(ref Vector3 vector, Vector3 newVector)
        {
            vector.x = newVector.x;
            vector.y = newVector.y;
            vector.z = newVector.z;
        }

    }

}
