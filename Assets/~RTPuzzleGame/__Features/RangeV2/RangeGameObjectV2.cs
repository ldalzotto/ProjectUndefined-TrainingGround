using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeGameObjectV2
    {
        private GameObject attachedGameObject;
        private GameObject rangeGameObject;
        private RangeObjectV2 assocaitedRangeObject;

        public RangeObjectV2PhysicsEventListener RangeObjectV2PhysicsEventListener { get; private set; }
        public CollisionType CollisionType { get; private set; }

        public RangeGameObjectV2(GameObject attachedGameObject)
        {
            this.attachedGameObject = attachedGameObject;
        }

        public Collider BoundingCollider { get; private set; }

        private void CommontInit(RangeObjectInitialization RangeObjectInitialization, RangeObjectV2 RangeObjectV2)
        {
            this.assocaitedRangeObject = RangeObjectV2;
            this.rangeGameObject = new GameObject();
            if (this.attachedGameObject != null)
            {
                this.rangeGameObject.transform.parent = this.attachedGameObject.transform;
            }
            this.rangeGameObject.transform.localPosition = Vector3.zero;

            var rigidbody = this.rangeGameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;

            this.CollisionType = this.rangeGameObject.AddComponent<CollisionType>();
            this.CollisionType.IsRange = true;

            this.RangeObjectV2PhysicsEventListener = this.rangeGameObject.AddComponent<RangeObjectV2PhysicsEventListener>();
        }

        public void Init(SphereRangeObjectInitialization SphereRangeObjectInitialization, RangeObjectV2 RangeObjectV2)
        {
            this.CommontInit(SphereRangeObjectInitialization, RangeObjectV2);
            var sphereCollider = this.rangeGameObject.AddComponent<SphereCollider>();
            sphereCollider.radius = SphereRangeObjectInitialization.SphereRangeTypeDefinition.Radius;
            this.BoundingCollider = sphereCollider;
            this.BoundingCollider.isTrigger = true;
        }

        public void Init(BoxRangeObjectInitialization BoxRangeObjectInitialization, RangeObjectV2 RangeObjectV2)
        {
            this.CommontInit(BoxRangeObjectInitialization, RangeObjectV2);
            var boxCollider = this.rangeGameObject.AddComponent<BoxCollider>();
            boxCollider.center = BoxRangeObjectInitialization.BoxRangeTypeDefinition.Center;
            boxCollider.size = BoxRangeObjectInitialization.BoxRangeTypeDefinition.Size;
            this.BoundingCollider = boxCollider;
            this.BoundingCollider.isTrigger = true;
        }

        public void Init(FrustumRangeObjectInitialization FrustumRangeObjectInitialization, RangeObjectV2 RangeObjectV2)
        {
            this.CommontInit(FrustumRangeObjectInitialization, RangeObjectV2);
            this.InitializeFrustum(FrustumRangeObjectInitialization.FrustumRangeTypeDefinition.FrustumV2);
        }

        public void Init(RoundedFrustumRangeObjectInitialization FrustumRangeObjectInitialization, RangeObjectV2 RangeObjectV2)
        {
            this.CommontInit(FrustumRangeObjectInitialization, RangeObjectV2);
            this.InitializeFrustum(FrustumRangeObjectInitialization.RoundedFrustumRangeTypeDefinition.FrustumV2);
        }

        private void InitializeFrustum(FrustumV2 frustum)
        {
            var boxCollider = this.rangeGameObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(0, 0, frustum.F2.FaceOffsetFromCenter.z / 4f);
            boxCollider.size = new Vector3(Mathf.Max(frustum.F1.Width, frustum.F2.Width), Math.Max(frustum.F1.Height, frustum.F2.Height), frustum.F2.FaceOffsetFromCenter.z / 2f);
            this.BoundingCollider = boxCollider;
            this.BoundingCollider.isTrigger = true;
        }

        public void ReceiveEvent(SetWorldPositionEvent SetWorldPositionEvent) { this.rangeGameObject.transform.position = SetWorldPositionEvent.WorldPosition; }
        public RangeObjectV2GetWorldTransformEventReturn GetTransform()
        {
            return new RangeObjectV2GetWorldTransformEventReturn
            {
                WorldPosition = this.rangeGameObject.transform.position,
                WorldRotation = this.rangeGameObject.transform.rotation,
                LossyScale = this.rangeGameObject.transform.lossyScale
            };
        }
    }

    public struct RangeObjectV2GetWorldToLocalMatrixEventReturn
    {
        public Matrix4x4 WorldToLocalMatrix;
    }

    public struct RangeObjectV2GetWorldTransformEventReturn
    {
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;
        public Vector3 LossyScale;
    }
}
