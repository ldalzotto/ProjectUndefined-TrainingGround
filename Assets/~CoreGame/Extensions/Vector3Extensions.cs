using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public static class Vector3Extensions
    {
        public static Vector3 Add(this Vector3 vector, float x, float y, float z)
        {
            vector.x += x;
            vector.y += y;
            vector.z += z;
            return vector;
        }

        public static Vector3 SetVector(this Vector3 vector, float x, float y, float z)
        {
            vector.x = x;
            vector.y = y;
            vector.z = z;
            return vector;
        }

        public static Vector3 Round(this Vector3 vector, int decimalNb)
        {
            vector.x = (float)(System.Math.Round((double)vector.x, decimalNb));
            vector.y = (float)(System.Math.Round((double)vector.y, decimalNb));
            vector.z = (float)(System.Math.Round((double)vector.z, decimalNb));
            return vector;
        }
    }

}
