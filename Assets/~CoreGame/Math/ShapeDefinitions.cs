using UnityEngine;

namespace CoreGame
{
    /*
     *     C5----C6
     *    / |    /|
     *   C1----C2 |
     *   |  C8  | C7   
     *   | /    |/     C3->C7  Forward
     *   C4----C3     
     */

    [System.Serializable]
    public class FrustumV2
    {
        [SerializeField]
        public Vector3 Center;
        [SerializeField]
        public Vector3 WorldPosition;
        [SerializeField]
        private Quaternion worldRotation;
        [SerializeField]
        public Quaternion DeltaRotation;
        [SerializeField]
        public Vector3 LossyScale;
        [SerializeField]
        public FrustumFaceV2 F1;
        [SerializeField]
        public FrustumFaceV2 F2;
        [SerializeField]
        public Vector3 LocalStartAngleProjection;
        [SerializeField]
        public float FaceDistance;

        public Quaternion WorldRotation { set => worldRotation = value; }

        public FrustumV2()
        {
            this.F1 = new FrustumFaceV2();
            this.F2 = new FrustumFaceV2();
        }

        public void SetLocalStartAngleProjection(Vector3 worldPositionPoint)
        {
            this.LocalStartAngleProjection = worldPositionPoint - this.WorldPosition;
        }

        public Quaternion GetRotation()
        {
            return this.worldRotation * this.DeltaRotation;
        }

        public void CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8)
        {
            Vector3 rotatedFrustumCenter = this.GetRotation() * this.Center;

            Vector3 diagDirection = this.F1.FaceOffsetFromCenter + new Vector3(-this.F1.Width, this.F1.Height, 0);
            diagDirection.Scale(this.LossyScale);
            Intersection.BoxPointCalculationV2(this.WorldPosition, this.GetRotation(), rotatedFrustumCenter, diagDirection / 2f, out C1);

            diagDirection = this.F1.FaceOffsetFromCenter + new Vector3(this.F1.Width, this.F1.Height, 0);
            diagDirection.Scale(this.LossyScale);
            Intersection.BoxPointCalculationV2(this.WorldPosition, this.GetRotation(), rotatedFrustumCenter, diagDirection / 2f, out C2);

            diagDirection = this.F1.FaceOffsetFromCenter + new Vector3(this.F1.Width, -this.F1.Height, 0);
            diagDirection.Scale(this.LossyScale);
            Intersection.BoxPointCalculationV2(this.WorldPosition, this.GetRotation(), rotatedFrustumCenter, diagDirection / 2f, out C3);

            diagDirection = this.F1.FaceOffsetFromCenter + new Vector3(-this.F1.Width, -this.F1.Height, 0);
            diagDirection.Scale(this.LossyScale);
            Intersection.BoxPointCalculationV2(this.WorldPosition, this.GetRotation(), rotatedFrustumCenter, diagDirection / 2f, out C4);

            //Projection point calculation
            Vector3 WorldStartAngleProjection;
            Intersection.BoxPointCalculationV2(this.WorldPosition, Quaternion.identity, rotatedFrustumCenter, this.LocalStartAngleProjection, out WorldStartAngleProjection);
            C5 = C1 + ((C1 - WorldStartAngleProjection) * this.FaceDistance);
            C6 = C2 + ((C2 - WorldStartAngleProjection) * this.FaceDistance);
            C7 = C3 + ((C3 - WorldStartAngleProjection) * this.FaceDistance);
            C8 = C4 + ((C4 - WorldStartAngleProjection) * this.FaceDistance);
        }
    }

    [System.Serializable]
    public class FrustumFaceV2
    {
        [SerializeField]
        public Vector3 FaceOffsetFromCenter;
        [SerializeField]
        public float Height;
        [SerializeField]
        public float Width;
    }

}
