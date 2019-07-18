using UnityEngine;

namespace RTPuzzle
{
    //Angles definition are based on local forward direction
    public class RangeAngleLimitationModule : MonoBehaviour
    {
        [Range(0f, Mathf.PI)]
        public float MaxAngleRad = Mathf.PI;

        public bool HasAngleLimitation()
        {
            return this.MaxAngleRad < Mathf.PI;
        }

        public bool IsInside(Vector3 worldPointComparison)
        {
            return Vector3.Angle(this.transform.forward, (worldPointComparison - this.transform.position).normalized) * Mathf.Deg2Rad <= this.MaxAngleRad;
        }
    }
}
