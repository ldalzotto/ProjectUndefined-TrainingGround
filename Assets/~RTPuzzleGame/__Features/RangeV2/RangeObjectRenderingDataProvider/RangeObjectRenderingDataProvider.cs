using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public abstract class ARangeObjectRenderingDataProvider
    {
        protected RangeObjectV2 RangeObjectV2Ref;
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
    
    public class FrustumRangeObjectRenderingDataProvider : ARangeObjectRenderingDataProvider
    {
        private RangeFrustumWorldPositioning RangeFrustumWorldPositioning;
        public FrustumV2 Frustum { get; private set; }

        public FrustumPointsPositions GetFrustumWorldPosition()
        {
            return this.RangeFrustumWorldPositioning.GetWorldFrustumPositions(this.RangeObjectV2Ref.RangeGameObjectV2.GetLocalToWorldMatrix());
        }

        public FrustumRangeObjectRenderingDataProvider(RoundedFrustumRangeObjectV2 RoundedFrustumRangeObjectV2, RangeTypeID RangeTypeID) : base(RoundedFrustumRangeObjectV2, RangeTypeID)
        {
            RoundedFrustumRangeObjectV2.GetFrustum().CalculateFrustumWorldPositionyFace(out FrustumPointsPositions LocalFrustumPointPositions, new TransformStruct { WorldPosition = Vector3.zero, WorldRotation = Quaternion.identity, LossyScale = Vector3.one });
            this.RangeFrustumWorldPositioning = new RangeFrustumWorldPositioning
            {
                LocalFrustumPositions = LocalFrustumPointPositions
            };
            this.Frustum = RoundedFrustumRangeObjectV2.GetFrustum();
        }
    }

    public struct RangeFrustumWorldPositioning
    {
        public FrustumPointsPositions LocalFrustumPositions;

        public FrustumPointsPositions GetWorldFrustumPositions(Matrix4x4 LocalToWorld)
        {
            return new FrustumPointsPositions(
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC1),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC2),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC3),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC4),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC5),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC6),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC7),
                LocalToWorld.MultiplyPoint(this.LocalFrustumPositions.FC8)
               );
        }
    }
}
