using System;
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
        public Quaternion DeltaRotation = Quaternion.identity;
        [SerializeField]
        public FrustumFaceV2 F1;
        [SerializeField]
        public FrustumFaceV2 F2;
        [SerializeField]
        public float FaceDistance;
        
        
        private Vector3 worldPosition;
        private Quaternion worldRotation;
        private Vector3 lossyScale;
        private Vector3 worldStartAngleProjection;
        private FrustumCalculationType frustumCalculationType;

        public Quaternion WorldRotation { set => worldRotation = value; }

        public FrustumV2()
        {
            this.F1 = new FrustumFaceV2();
            this.F2 = new FrustumFaceV2();
        }

        public void SetCalculationDataForFaceBasedCalculation(Vector3 frustumWorldPosition, Quaternion frustumWorldRotation, Vector3 frustumLossyScale)
        {
            this.worldPosition = frustumWorldPosition;
            this.worldRotation = frustumWorldRotation;
            this.lossyScale = frustumLossyScale;
            this.frustumCalculationType = FrustumCalculationType.FACE;
        }

        public void SetCalculationDataForPointProjection(Vector3 worldPositionPoint, Vector3 frustumWorldPosition, Quaternion frustumWorldRotation, Vector3 frustumLossyScale)
        {
            this.worldPosition = frustumWorldPosition;
            this.worldRotation = frustumWorldRotation;
            this.lossyScale = frustumLossyScale;
            this.worldStartAngleProjection = worldPositionPoint;
            this.frustumCalculationType = FrustumCalculationType.PROJECTION;
        }

        public Quaternion GetRotation()
        {
            return this.worldRotation * this.DeltaRotation;
        }

        private Vector3 LocalToWorld(Vector3 localPoint)
        {
            return (this.worldPosition + this.worldRotation * ((this.DeltaRotation * localPoint) + this.Center).Mul(this.lossyScale)).Round(3);
        }

        public void CalculateFrustumPoints(out Vector3 C1, out Vector3 C2, out Vector3 C3, out Vector3 C4, out Vector3 C5, out Vector3 C6, out Vector3 C7, out Vector3 C8)
        {

            C1 = this.LocalToWorld((new Vector3(-this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            C2 = this.LocalToWorld((new Vector3(this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            C3 = this.LocalToWorld((new Vector3(this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            C4 = this.LocalToWorld((new Vector3(-this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));

            if (this.frustumCalculationType == FrustumCalculationType.FACE)
            {
                C5 = this.LocalToWorld((new Vector3(-this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                C6 = this.LocalToWorld((new Vector3(this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                C7 = this.LocalToWorld((new Vector3(this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                C8 = this.LocalToWorld((new Vector3(-this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
            }
            else
            {
                C5 = C1 + ((C1 - worldStartAngleProjection) * this.FaceDistance);
                C6 = C2 + ((C2 - worldStartAngleProjection) * this.FaceDistance);
                C7 = C3 + ((C3 - worldStartAngleProjection) * this.FaceDistance);
                C8 = C4 + ((C4 - worldStartAngleProjection) * this.FaceDistance);
            }
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

    [System.Serializable]
    public enum FrustumCalculationType
    {
        FACE, PROJECTION
    }
}
