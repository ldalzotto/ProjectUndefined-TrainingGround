﻿using InteractiveObjectTest;
using UnityEngine;

namespace RTPuzzle
{
    public class RangeGameObjectV2
    {
        private GameObject attachedGameObject;
        public GameObject RangeGameObject { get; private set; }
        private RangeObjectV2 assocaitedRangeObject;

        public RangeObjectV2PhysicsEventListener RangeObjectV2PhysicsEventListener { get; private set; }
        
        public RangeGameObjectV2(GameObject attachedGameObject, SphereRangeObjectInitialization SphereRangeObjectInitialization, RangeObjectV2 RangeObjectV2, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.attachedGameObject = attachedGameObject;
            this.CommontInit(SphereRangeObjectInitialization, RangeObjectV2, AssociatedInteractiveObject);
            this.BoundingCollider = RangeObjectBoundingColliderBuilder.BuildBoundingCollider(SphereRangeObjectInitialization, this);
        }

        public RangeGameObjectV2(GameObject attachedGameObject, BoxRangeObjectInitialization BoxRangeObjectInitialization, RangeObjectV2 RangeObjectV2, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.attachedGameObject = attachedGameObject;
            this.CommontInit(BoxRangeObjectInitialization, RangeObjectV2, AssociatedInteractiveObject);
            this.BoundingCollider = RangeObjectBoundingColliderBuilder.BuildBoundingCollider(BoxRangeObjectInitialization, this);
        }

        public RangeGameObjectV2(GameObject attachedGameObject, FrustumRangeObjectInitialization FrustumRangeObjectInitialization, RangeObjectV2 RangeObjectV2, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.attachedGameObject = attachedGameObject;
            this.CommontInit(FrustumRangeObjectInitialization, RangeObjectV2, AssociatedInteractiveObject);
            this.BoundingCollider = RangeObjectBoundingColliderBuilder.BuildBoundingCollider(FrustumRangeObjectInitialization, this);
        }

        public RangeGameObjectV2(GameObject attachedGameObject, RoundedFrustumRangeObjectInitialization FrustumRangeObjectInitialization, RangeObjectV2 RangeObjectV2, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.attachedGameObject = attachedGameObject;
            this.CommontInit(FrustumRangeObjectInitialization, RangeObjectV2, AssociatedInteractiveObject);
            this.BoundingCollider = RangeObjectBoundingColliderBuilder.BuildBoundingCollider(FrustumRangeObjectInitialization, this);
        }


        public Collider BoundingCollider { get; private set; }

        private void CommontInit(RangeObjectInitialization RangeObjectInitialization, RangeObjectV2 RangeObjectV2, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.assocaitedRangeObject = RangeObjectV2;
            this.RangeGameObject = new GameObject();
            if (this.attachedGameObject != null)
            {
                this.RangeGameObject.transform.parent = this.attachedGameObject.transform;
            }
            this.RangeGameObject.transform.localPosition = Vector3.zero;

            var rigidbody = this.RangeGameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;

            this.RangeObjectV2PhysicsEventListener = this.RangeGameObject.AddComponent<RangeObjectV2PhysicsEventListener>();
            this.RangeObjectV2PhysicsEventListener.Init(AssociatedInteractiveObject);
        }

        public void ReceiveEvent(SetWorldPositionEvent SetWorldPositionEvent) { this.RangeGameObject.transform.position = SetWorldPositionEvent.WorldPosition; }
        public TransformStruct GetTransform()
        {
            return new TransformStruct(this.RangeGameObject.transform);
        }
        public Matrix4x4 GetLocalToWorldMatrix()
        {
            return this.RangeGameObject.transform.localToWorldMatrix;
        }
    }

    public struct RangeObjectV2GetWorldToLocalMatrixEventReturn
    {
        public Matrix4x4 WorldToLocalMatrix;
    }
}
