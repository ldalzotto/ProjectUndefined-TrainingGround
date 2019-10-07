using CoreGame;
using InteractiveObjectTest;
using UnityEngine;
using UnityEngine.Profiling;

namespace RTPuzzle
{
    public abstract class RangeObjectV2
    {
        public RangeGameObjectV2 RangeGameObjectV2 { get; private set; }

        public RangeObjectInitialization RangeObjectInitialization { get; private set; }

        public RangeObstacleListenerSystem RangeObstacleListenerSystem { get; private set; }
        public RangeIntersectionV2System RangeIntersectionV2System { get; private set; }
        private RangeExternalPhysicsOnlyListenersSystem RangeExternalPhysicsOnlyListenersSystem;

        protected void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
        {
            this.RangeGameObjectV2 = RangeGameObjectV2;
            this.RangeObjectInitialization = RangeObjectInitialization;

            this.RangeIntersectionV2System = new RangeIntersectionV2System(this);
            this.RangeExternalPhysicsOnlyListenersSystem = new RangeExternalPhysicsOnlyListenersSystem(this, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListener);
            if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
            {
                this.RangeObstacleListenerSystem = new RangeObstacleListenerSystem(this, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListener);
            }

            PuzzleGameSingletonInstances.RangeEventsManager.RANGE_EVT_Range_Created(this);
        }

        public virtual void Tick(float d)
        {
            Profiler.BeginSample("RangeObjectV2 : Tick");
            this.RangeIntersectionV2System.Tick(d);
            Profiler.EndSample();
        }

        public void OnDestroy()
        {
            //we call listeners callbacks
            this.RangeIntersectionV2System.OnDestroy();
            if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
            {
                this.RangeObstacleListenerSystem.OnDestroy();
            }
            PuzzleGameSingletonInstances.RangeEventsManager.RANGE_EVT_Range_Destroy(this);
        }

        public void ReceiveEvent(SetWorldPositionEvent SetWorldPositionEvent)
        {
            this.RangeGameObjectV2.ReceiveEvent(SetWorldPositionEvent);
        }
        public virtual void ReceiveEvent(RangeTransformChanged RangeTransformChanged) { }
        public void ReceiveEvent(RangeIntersectionAddIntersectionListenerEvent RangeIntersectionAddIntersectionListenerEvent)
        {
            this.RangeIntersectionV2System.ReceiveEvent(RangeIntersectionAddIntersectionListenerEvent, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListener);

        }
        public void ReceiveEvent(RangeExternalPhysicsOnlyAddListener RangeExternalPhysicsOnlyAddListener) { this.RangeExternalPhysicsOnlyListenersSystem.ReceiveEvent(RangeExternalPhysicsOnlyAddListener); }

        public ObstacleListener GetObstacleListener() { return this.RangeObstacleListenerSystem != null ? this.RangeObstacleListenerSystem.ObstacleListener : null; }
        public TransformStruct GetTransform() { return this.RangeGameObjectV2.GetTransform(); }
    }

    public class SphereRangeObjectV2 : RangeObjectV2
    {
        private SphereRangeObjectInitialization SphereRangeObjectInitialization;
        public SphereCollider SphereBoundingCollider { get; private set; }

        public SphereRangeObjectV2(GameObject AssociatedGameObject, SphereRangeObjectInitialization SphereRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.SphereRangeObjectInitialization = SphereRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.SphereRangeObjectInitialization, this, AssociatedInteractiveObject);
            this.SphereBoundingCollider = (SphereCollider)RangeGameObjectV2.BoundingCollider;
            base.Init(RangeGameObjectV2, SphereRangeObjectInitialization);
        }

    }

    public class BoxRangeObjectV2 : RangeObjectV2
    {
        private BoxRangeObjectInitialization BoxRangeObjectInitialization;
        public BoxCollider BoxBoundingCollider { get; private set; }

        public BoxRangeObjectV2(GameObject AssociatedGameObject, BoxRangeObjectInitialization BoxRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.BoxRangeObjectInitialization = BoxRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.BoxRangeObjectInitialization, this, AssociatedInteractiveObject);
            this.BoxBoundingCollider = (BoxCollider)RangeGameObjectV2.BoundingCollider;
            base.Init(RangeGameObjectV2, BoxRangeObjectInitialization);
        }
    }

    public class FrustumRangeObjectV2 : RangeObjectV2
    {
        private FrustumRangeObjectInitialization FrustumRangeObjectInitialization;

        private RangeWorldPositionChangeSystem RangeWorldPositionChangeSystem;

        public FrustumV2 GetFrustum() { return this.FrustumRangeObjectInitialization.FrustumRangeTypeDefinition.FrustumV2; }

        public FrustumRangeObjectV2(GameObject AssociatedGameObject, FrustumRangeObjectInitialization FrustumRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.RangeWorldPositionChangeSystem = new RangeWorldPositionChangeSystem(this);

            this.FrustumRangeObjectInitialization = FrustumRangeObjectInitialization;
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.FrustumRangeObjectInitialization, this, AssociatedInteractiveObject);
            base.Init(RangeGameObjectV2, FrustumRangeObjectInitialization);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.RangeWorldPositionChangeSystem.Tick(d);
        }

    }

    public class RoundedFrustumRangeObjectV2 : RangeObjectV2
    {
        private RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization;

        private RangeWorldPositionChangeSystem RangeWorldPositionChangeSystem;
        
        public FrustumV2 GetFrustum() { return this.RoundedFrustumRangeObjectInitialization.RoundedFrustumRangeTypeDefinition.FrustumV2; }

        public RoundedFrustumRangeObjectV2(GameObject AssociatedGameObject, RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.RangeWorldPositionChangeSystem = new RangeWorldPositionChangeSystem(this);

            this.RoundedFrustumRangeObjectInitialization = RoundedFrustumRangeObjectInitialization;
            
            var RangeGameObjectV2 = new RangeGameObjectV2(AssociatedGameObject, this.RoundedFrustumRangeObjectInitialization, this, AssociatedInteractiveObject);
            base.Init(RangeGameObjectV2, RoundedFrustumRangeObjectInitialization);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.RangeWorldPositionChangeSystem.Tick(d);
        }
    }

 

    public struct SetWorldPositionEvent
    {
        public Vector3 WorldPosition;
    }
}
