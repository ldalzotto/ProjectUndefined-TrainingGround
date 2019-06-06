using UnityEngine;

namespace CoreGame
{
    public class Intersection
    {

        public static bool BoxIntersectsSphere(BoxCollider boxCollider, Vector3 S, float R)
        {
            var boxPosition = boxCollider.transform.position + boxCollider.center;

            Vector3 diagDirection = new Vector3(boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            var C1 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            var C2 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            diagDirection = new Vector3(-boxCollider.size.x, -boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            var C3 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            var C4 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            diagDirection = new Vector3(-boxCollider.size.x, boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            var C5 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            var C6 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            diagDirection = new Vector3(boxCollider.size.x, -boxCollider.size.y, boxCollider.size.z);
            diagDirection.Scale(boxCollider.transform.lossyScale);

            var C7 = (boxPosition - boxCollider.transform.rotation * (diagDirection / 2f));
            var C8 = (boxPosition + boxCollider.transform.rotation * (diagDirection / 2f));

            float dist_squared = R * R;
            if (DiagonalCheck(S, C1, C2, R * R) > 0) { return true; }
            else if (DiagonalCheck(S, C3, C4, R * R) > 0) { return true; }
            else if (DiagonalCheck(S, C5, C6, R * R) > 0) { return true; }
            else if (DiagonalCheck(S, C7, C8, R * R) > 0) { return true; }
            return false;
        }

        private static float DiagonalCheck(Vector3 S, Vector3 point1, Vector3 point2, float dist_squared)
        {
            if (S.x < point1.x) dist_squared -= Mathf.Pow(S.x - point1.x, 2);
            else if (S.x > point2.x) dist_squared -= Mathf.Pow(S.x - point2.x, 2);
            if (S.y < point1.y) dist_squared -= Mathf.Pow(S.y - point1.y, 2);
            else if (S.y > point2.y) dist_squared -= Mathf.Pow(S.y - point2.y, 2);
            if (S.z < point1.z) dist_squared -= Mathf.Pow(S.z - point1.z, 2);
            else if (S.z > point2.z) dist_squared -= Mathf.Pow(S.z - point2.z, 2);
            return dist_squared;
        }
    }

}
