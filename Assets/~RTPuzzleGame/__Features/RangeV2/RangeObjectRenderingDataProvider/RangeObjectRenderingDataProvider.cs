using UnityEngine;
using System.Collections;
using GameConfigurationID;
using CoreGame;

namespace RTPuzzle
{
    public abstract class ARangeObjectRenderingDataProvider
    {
        private RangeObjectV2 RangeObjectV2Ref;
        public RangeTypeID RangeTypeID { get; private set; }
        public Collider BoundingCollider { get; private set; }
        public ObstacleListener ObstacleListener { get; private set; }

        protected ARangeObjectRenderingDataProvider(RangeObjectV2 rangeObjectV2Ref, RangeTypeID RangeTypeID)
        {
            RangeObjectV2Ref = rangeObjectV2Ref;
            this.RangeTypeID = RangeTypeID;
            this.BoundingCollider = rangeObjectV2Ref.RangeGameObjectV2.BoundingCollider;
            this.ObstacleListener = rangeObjectV2Ref.GetObstacleListener();
        }

        public bool IsTakingObstacleIntoConsideration()
        {
            return this.RangeObjectV2Ref.RangeObstacleListenerSystem != null;
        }
    }

    public class BoxRangeObjectRenderingDataProvider : ARangeObjectRenderingDataProvider
    {
        public BoxCollider BoundingBoxCollider { get; private set; }

        public BoxRangeObjectRenderingDataProvider(BoxRangeObjectV2 BoxRangeObjectV2, RangeTypeID RangeTypeID) : base(BoxRangeObjectV2, RangeTypeID)
        {
            this.BoundingBoxCollider = BoxRangeObjectV2.BoxBoundingCollider;
        }
    }

    public class SphereRangeObjectRenderingDataProvider : ARangeObjectRenderingDataProvider
    {
        public SphereCollider BoundingSphereCollider { get; private set; }

        public SphereRangeObjectRenderingDataProvider(SphereRangeObjectV2 SphereRangeObjectV2, RangeTypeID RangeTypeID) : base(SphereRangeObjectV2, RangeTypeID)
        {
            this.BoundingSphereCollider = SphereRangeObjectV2.SphereBoundingCollider;
        }

        public float GetRadius() { return this.BoundingSphereCollider.radius; }
        public Vector3 GetWorldPositionCenter() { return this.BoundingSphereCollider.transform.position; }
    }

    public class FrustumRangeObjectRenderingDataprovider : ARangeObjectRenderingDataProvider
    {
        public FrustumV2 Frustum { get; private set; }

        public FrustumRangeObjectRenderingDataprovider(FrustumRangeObjectV2 FrustumRangeObjectV2, RangeTypeID RangeTypeID) : base(FrustumRangeObjectV2, RangeTypeID)
        {
            this.Frustum = FrustumRangeObjectV2.GetFrustum();
        }
    }

    public class RoundedFrustumRangeObjectRenderingDataProvider : ARangeObjectRenderingDataProvider
    {

        public FrustumV2 Frustum { get; private set; }

        public RoundedFrustumRangeObjectRenderingDataProvider(RoundedFrustumRangeObjectV2 RoundedFrustumRangeObjectV2, RangeTypeID RangeTypeID) : base(RoundedFrustumRangeObjectV2, RangeTypeID)
        {
            this.Frustum = RoundedFrustumRangeObjectV2.GetFrustum();
        }
    }
}
