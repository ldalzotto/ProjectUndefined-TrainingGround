using UnityEngine;

namespace CoreGame
{
    public class Intersection
    {

        public static bool BoxIntersectsSphere(BoxCollider boxCollider, Vector3 SphereWorldPosition, float SphereRadius)
        {
            var boxPosition = boxCollider.transform.position + boxCollider.center;

            Vector3 diagDirection = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            Vector3 C1 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            Vector3 C2 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            diagDirection = new Vector3(-boxCollider.size.x, -boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            Vector3 C3 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            Vector3 C4 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            diagDirection = new Vector3(-boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            Vector3 C5 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            Vector3 C6 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            diagDirection = new Vector3(boxCollider.size.x, -boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            Vector3 C7 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            Vector3 C8 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            float dist_squared = SphereRadius * SphereRadius;
            if (DiagonalCheck(SphereWorldPosition, C1, C2, SphereRadius)) { return true; }
            else if (DiagonalCheck(SphereWorldPosition, C3, C4, SphereRadius)) { return true; }
            else if (DiagonalCheck(SphereWorldPosition, C5, C6, SphereRadius)) { return true; }
            else if (DiagonalCheck(SphereWorldPosition, C7, C8, SphereRadius)) { return true; }
            return false;
        }

        private static bool DiagonalCheck(Vector3 S, Vector3 point1, Vector3 point2, float radius)
        {

            //X projection
            bool xProjectionInside = false;
            float minX = (float)(System.Math.Round((double)Mathf.Min(point1.x, point2.x), 3));
            float maxX = (float)(System.Math.Round((double)Mathf.Max(point1.x, point2.x), 3));

            if (minX >= S.x - radius && minX <= S.x + radius) { xProjectionInside = true; }
            if (maxX >= S.x - radius && maxX <= S.x + radius) { xProjectionInside = true; }
            if (minX <= S.x - radius && maxX >= S.x + radius) { xProjectionInside = true; }

            //Y projection
            bool yProjectionInside = false;
            float minY = (float)(System.Math.Round((double)Mathf.Min(point1.y, point2.y), 3));
            float maxY = (float)(System.Math.Round((double)Mathf.Max(point1.y, point2.y), 3));

            if (minY >= S.y - radius && minY <= S.y + radius) { yProjectionInside = true; }
            if (maxY >= S.y - radius && maxY <= S.y + radius) { yProjectionInside = true; }
            if (minY <= S.y - radius && maxY >= S.y + radius) { yProjectionInside = true; }

            //Z projection
            bool zProjectionInside = false;
            float minZ = (float)(System.Math.Round((double)Mathf.Min(point1.z, point2.z), 3));
            float maxZ = (float)(System.Math.Round((double)Mathf.Max(point1.z, point2.z), 3));

            if (minZ >= S.z - radius && minZ <= S.z + radius) { zProjectionInside = true; }
            if (maxZ >= S.z - radius && maxZ <= S.z + radius) { zProjectionInside = true; }
            if (minZ <= S.z - radius && maxZ >= S.z + radius) { zProjectionInside = true; }

            return (xProjectionInside && yProjectionInside && zProjectionInside);
        }


    }

}
