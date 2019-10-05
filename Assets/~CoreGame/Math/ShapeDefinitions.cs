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

        public FrustumPointsPositions FrustumPointsPositions { get; private set; }

        private Vector3 worldPosition;
        private Quaternion worldRotation;
        private Vector3 lossyScale;
        private Vector3 worldStartAngleProjection;
        private FrustumCalculationType frustumCalculationType;

        public FrustumV2 Clone()
        {
            FrustumV2 cloned = new FrustumV2();
            cloned.Center = this.Center;
            cloned.DeltaRotation = this.DeltaRotation;
            cloned.F1 = this.F1.Clone();
            cloned.F2 = this.F2.Clone();
            cloned.FaceDistance = this.FaceDistance;
            return cloned;
        }

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
            this.CalculateFrustumPointsWorldPosV2();
        }

        public void SetCalculationDataForPointProjection(Vector3 worldPositionPoint, Vector3 frustumWorldPosition, Quaternion frustumWorldRotation, Vector3 frustumLossyScale)
        {
            this.worldPosition = frustumWorldPosition;
            this.worldRotation = frustumWorldRotation;
            this.lossyScale = frustumLossyScale;
            this.worldStartAngleProjection = worldPositionPoint;
            this.frustumCalculationType = FrustumCalculationType.PROJECTION;
            this.CalculateFrustumPointsWorldPosV2();
        }

        public float GetFrustumFaceRadius()
        {
            return this.F2.FaceOffsetFromCenter.z / 2f;
        }

        private Vector3 LocalToWorld(Vector3 localPoint)
        {
            return (this.worldPosition + this.worldRotation * ((this.DeltaRotation * localPoint) + this.Center).Mul(this.lossyScale)).Round(3);
        }

        private void CalculateFrustumPointsWorldPosV2()
        {
            Vector3 C1 = this.LocalToWorld((new Vector3(-this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C2 = this.LocalToWorld((new Vector3(this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C3 = this.LocalToWorld((new Vector3(this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C4 = this.LocalToWorld((new Vector3(-this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));

            if (this.frustumCalculationType == FrustumCalculationType.FACE)
            {
                Vector3 C5 = this.LocalToWorld((new Vector3(-this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                Vector3 C6 = this.LocalToWorld((new Vector3(this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                Vector3 C7 = this.LocalToWorld((new Vector3(this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                Vector3 C8 = this.LocalToWorld((new Vector3(-this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));

                this.FrustumPointsPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);
            }
            else
            {
                Vector3 C5 = C1 + ((C1 - worldStartAngleProjection) * this.FaceDistance);
                Vector3 C6 = C2 + ((C2 - worldStartAngleProjection) * this.FaceDistance);
                Vector3 C7 = C3 + ((C3 - worldStartAngleProjection) * this.FaceDistance);
                Vector3 C8 = C4 + ((C4 - worldStartAngleProjection) * this.FaceDistance);

                this.FrustumPointsPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);
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

        public FrustumFaceV2 Clone()
        {
            FrustumFaceV2 cloned = new FrustumFaceV2();
            cloned.FaceOffsetFromCenter = this.FaceOffsetFromCenter;
            cloned.Height = this.Height;
            cloned.Width = this.Width;
            return cloned;
        }
    }

    [System.Serializable]
    public enum FrustumCalculationType
    {
        FACE, PROJECTION
    }

    public struct FrustumV2Struct
    {
        public Vector3 Center;
        public Quaternion DeltaRotation;
        public FrustumFaceV2Struct F1;
        public FrustumFaceV2Struct F2;
        public float FaceDistance;

        public TransformStruct FrustumWorldTransform;
        public Vector3 WorldStartAngleProjection;
        public FrustumCalculationType FrustumCalculationType;

        private Vector3 LocalToWorld(Vector3 localPoint)
        {
            return (this.FrustumWorldTransform.WorldPosition + this.FrustumWorldTransform.WorldRotation * ((this.DeltaRotation * localPoint) + this.Center).Mul(this.FrustumWorldTransform.LossyScale)).Round(3);
        }

        public void CalculateFrustumPointsWorldPosByProjection(out FrustumPointsPositions FrustumPointsPositions, out bool IsFacing)
        {
            Vector3 C1 = this.LocalToWorld((new Vector3(-this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C2 = this.LocalToWorld((new Vector3(this.F1.Width, this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C3 = this.LocalToWorld((new Vector3(this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));
            Vector3 C4 = this.LocalToWorld((new Vector3(-this.F1.Width, -this.F1.Height, 0) + this.F1.FaceOffsetFromCenter).Mul(0.5f));

            Vector3 frontFaceNormal = Vector3.Cross(C2 - C1, C4 - C1).normalized;
            IsFacing = Vector3.Dot(frontFaceNormal, C1 - WorldStartAngleProjection) >= 0;

            //We abort calculation if not facing
            if (IsFacing)
            {
                Vector3 C5 = this.LocalToWorld((new Vector3(-this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                Vector3 C6 = this.LocalToWorld((new Vector3(this.F2.Width, this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                Vector3 C7 = this.LocalToWorld((new Vector3(this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));
                Vector3 C8 = this.LocalToWorld((new Vector3(-this.F2.Width, -this.F2.Height, 0) + this.F2.FaceOffsetFromCenter).Mul(0.5f));

                FrustumPointsPositions = new FrustumPointsPositions(C1, C2, C3, C4, C5, C6, C7, C8);
            }
            else
            {
                FrustumPointsPositions = default;
            }
        }

        public static FrustumV2Struct FromFrustumV2PointProjection(TransformStruct FrustumWorldTransform, Vector3 WorldStartAngleProjection, FrustumV2 FrustumV2)
        {
            return new FrustumV2Struct
            {
                Center = FrustumV2.Center,
                DeltaRotation = FrustumV2.DeltaRotation,
                FaceDistance = FrustumV2.FaceDistance,
                F1 = FrustumFaceV2Struct.FromFrustumFaveV2(FrustumV2.F1),
                F2 = FrustumFaceV2Struct.FromFrustumFaveV2(FrustumV2.F2),
                FrustumWorldTransform = FrustumWorldTransform,
                FrustumCalculationType = FrustumCalculationType.PROJECTION,
                WorldStartAngleProjection = WorldStartAngleProjection
            };
        }
    }

    public struct FrustumFaceV2Struct
    {
        public Vector3 FaceOffsetFromCenter;
        public float Height;
        public float Width;

        public static FrustumFaceV2Struct FromFrustumFaveV2(FrustumFaceV2 FrustumFaceV2)
        {
            return new FrustumFaceV2Struct
            {
                FaceOffsetFromCenter = FrustumFaceV2.FaceOffsetFromCenter,
                Height = FrustumFaceV2.Height,
                Width = FrustumFaceV2.Width
            };
        }
    }
}
