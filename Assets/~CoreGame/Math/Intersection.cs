using UnityEngine;

namespace CoreGame
{
    public class Intersection
    {

        public static bool BoxIntersectsSphere(BoxCollider boxCollider, Vector3 SphereWorldPosition, float SphereRadius)
        {
            var boxPosition = boxCollider.transform.position + boxCollider.center;

            Vector3 diagDirection = new Vector3();

            SetVector(ref diagDirection, boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            if(DiagonalIntersection(ref diagDirection, boxPosition, boxCollider, SphereWorldPosition, SphereRadius)) { return true; }

            SetVector(ref diagDirection, -boxCollider.size.x, -boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            if (DiagonalIntersection(ref diagDirection, boxPosition, boxCollider, SphereWorldPosition, SphereRadius)) { return true; }

            SetVector(ref diagDirection, -boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            if (DiagonalIntersection(ref diagDirection, boxPosition, boxCollider, SphereWorldPosition, SphereRadius)) { return true; }

            SetVector(ref diagDirection, boxCollider.size.x, -boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);
            if (DiagonalIntersection(ref diagDirection, boxPosition, boxCollider, SphereWorldPosition, SphereRadius)) { return true; }

            return false;
        }

        private static bool DiagonalIntersection(ref Vector3 diagDirection, Vector3 boxPosition, BoxCollider boxCollider, Vector3 SphereWorldPosition, float SphereRadius)
        {
            Vector3 C1 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            Vector3 C2 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            if (DiagonalCheck(SphereWorldPosition, C1, C2, SphereRadius)) { return true; }
            return false;
        }

        private static bool DiagonalCheck(Vector3 S, Vector3 point1, Vector3 point2, float radius)
        {
            if (!DiagonalProjectedCheck(S.x, point1.x, point2.x, radius)) { return false; }
            if (!DiagonalProjectedCheck(S.y, point1.y, point2.y, radius)) { return false; }
            if (!DiagonalProjectedCheck(S.z, point1.z, point2.z, radius)) { return false; }
            return true;
        }

        private static bool DiagonalProjectedCheck(float SProjected, float point1Projected, float point2Projected, float radius)
        {
            float min = (float)(System.Math.Round((double)Mathf.Min(point1Projected, point2Projected), 3));
            float max = (float)(System.Math.Round((double)Mathf.Max(point1Projected, point2Projected), 3));

            return ((min >= SProjected - radius && min <= SProjected + radius) ||
                     (max >= SProjected - radius && max <= SProjected + radius) ||
                        (min <= SProjected - radius && max >= SProjected + radius));
        }

        private static void SetVector(ref Vector3 vector, float x, float y, float z)
        {
            vector.x = x;
            vector.y = y;
            vector.z = z;
        }


    }

}
