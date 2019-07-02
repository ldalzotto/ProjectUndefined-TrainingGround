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
    }

}
