using UnityEngine;

namespace CoreGame
{
    public static class QuaternionHelper
    {
        /// <summary>
        /// This method return a quaternion operation representing the rotation reduction needed to clamp to the cone described by <paramref name="maxAngleUnsigned"/>.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxAngleUnsigned"></param>
        /// <returns></returns>
        public static Quaternion ConeReduction(Vector3 from, Vector3 to, float maxAngleUnsigned)
        {
            var initialAngle = Vector3.Angle(from, to);
            var adjustedAngle = 0f;
            if (initialAngle >= maxAngleUnsigned)
            {
                adjustedAngle = initialAngle - maxAngleUnsigned;
            }
            return Quaternion.AngleAxis(adjustedAngle, Vector3.Cross(to, from));
        }

    }
}