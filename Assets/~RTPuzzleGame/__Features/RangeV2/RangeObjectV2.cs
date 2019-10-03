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
        private RangeIntersectionV2System RangeIntersectionV2System;
        private RangeExternalPhysicsOnlyListenersSystem RangeExternalPhysicsOnlyListenersSystem;
        
        public virtual void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.RangeGameObjectV2 = RangeGameObjectV2;
            this.RangeObjectInitialization = RangeObjectInitialization;

            this.RangeIntersectionV2System = new RangeIntersectionV2System(this);
            this.RangeExternalPhysicsOnlyListenersSystem = new RangeExternalPhysicsOnlyListenersSystem(this, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListener);
            if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
            {
                this.RangeObstacleListenerSystem = new RangeObstacleListenerSystem(this, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListener);
            }

            RTPuzzle.PuzzleGameSingletonInstances.RangeEventsManager.RANGE_EVT_Range_Created(this);
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
            RTPuzzle.PuzzleGameSingletonInstances.RangeEventsManager.RANGE_EVT_Range_Destroy(this);
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
        public RangeObjectV2GetWorldTransformEventReturn GetTransform() { return this.RangeGameObjectV2.GetTransform(); }
    }

    public class SphereRangeObjectV2 : RangeObjectV2
    {
        private SphereRangeObjectInitialization SphereRangeObjectInitialization;
        public SphereCollider SphereBoundingCollider { get; private set; }

        public SphereRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, SphereRangeObjectInitialization SphereRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.SphereRangeObjectInitialization = SphereRangeObjectInitialization;
            RangeGameObjectV2.Init(this.SphereRangeObjectInitialization, this, AssociatedInteractiveObject);
            this.SphereBoundingCollider = (SphereCollider)RangeGameObjectV2.BoundingCollider;
            this.Init(RangeGameObjectV2, SphereRangeObjectInitialization, AssociatedInteractiveObject);
        }

        public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            base.Init(RangeGameObjectV2, RangeObjectInitialization, AssociatedInteractiveObject);
        }

    }

    public class BoxRangeObjectV2 : RangeObjectV2
    {
        private BoxRangeObjectInitialization BoxRangeObjectInitialization;
        public BoxCollider BoxBoundingCollider { get; private set; }

        public BoxRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, BoxRangeObjectInitialization BoxRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.BoxRangeObjectInitialization = BoxRangeObjectInitialization;
            RangeGameObjectV2.Init(this.BoxRangeObjectInitialization, this, AssociatedInteractiveObject);
            this.BoxBoundingCollider = (BoxCollider)RangeGameObjectV2.BoundingCollider;
            this.Init(RangeGameObjectV2, BoxRangeObjectInitialization, AssociatedInteractiveObject);
        }

        public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            RangeGameObjectV2.Init(this.BoxRangeObjectInitialization, this, AssociatedInteractiveObject);
            base.Init(RangeGameObjectV2, RangeObjectInitialization, AssociatedInteractiveObject);
        }
    }

    public class FrustumRangeObjectV2 : RangeObjectV2
    {
        private FrustumRangeObjectInitialization FrustumRangeObjectInitialization;

        private RangeWorldPositionChangeSystem RangeWorldPositionChangeSystem;
        private FrustumRangeWorldPositionCalulcationSystem FrustumRangeWorldPositionCalulcationSystem;

        public FrustumV2 GetFrustum() { return this.FrustumRangeWorldPositionCalulcationSystem.FrustumV2; }

        public FrustumRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, FrustumRangeObjectInitialization FrustumRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.RangeWorldPositionChangeSystem = new RangeWorldPositionChangeSystem(this);
            this.FrustumRangeWorldPositionCalulcationSystem = new FrustumRangeWorldPositionCalulcationSystem(this, FrustumRangeObjectInitialization.FrustumRangeTypeDefinition.FrustumV2);

            this.FrustumRangeObjectInitialization = FrustumRangeObjectInitialization;
            RangeGameObjectV2.Init(this.FrustumRangeObjectInitialization, this, AssociatedInteractiveObject);
            this.Init(RangeGameObjectV2, FrustumRangeObjectInitialization, AssociatedInteractiveObject);
        }

        public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            base.Init(RangeGameObjectV2, RangeObjectInitialization, AssociatedInteractiveObject);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.RangeWorldPositionChangeSystem.Tick(d);
        }

        public override void ReceiveEvent(RangeTransformChanged RangeTransformChanged)
        {
            RangeObjectV2Commons.HandlingRangeTransformChangedForFrustums(this.RangeGameObjectV2, this.FrustumRangeWorldPositionCalulcationSystem);
        }
    }

    public class RoundedFrustumRangeObjectV2 : RangeObjectV2
    {
        private RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization;

        private RangeWorldPositionChangeSystem RangeWorldPositionChangeSystem;
        private FrustumRangeWorldPositionCalulcationSystem FrustumRangeWorldPositionCalulcationSystem;

        public FrustumV2 GetFrustum() { return this.FrustumRangeWorldPositionCalulcationSystem.FrustumV2; }

        public RoundedFrustumRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            this.RangeWorldPositionChangeSystem = new RangeWorldPositionChangeSystem(this);
            this.FrustumRangeWorldPositionCalulcationSystem = new FrustumRangeWorldPositionCalulcationSystem(this, RoundedFrustumRangeObjectInitialization.RoundedFrustumRangeTypeDefinition.FrustumV2);

            this.RoundedFrustumRangeObjectInitialization = RoundedFrustumRangeObjectInitialization;
            RangeGameObjectV2.Init(this.RoundedFrustumRangeObjectInitialization, this, AssociatedInteractiveObject);
            this.Init(RangeGameObjectV2, RoundedFrustumRangeObjectInitialization, AssociatedInteractiveObject);
        }

        public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization, CoreInteractiveObject AssociatedInteractiveObject)
        {
            base.Init(RangeGameObjectV2, RangeObjectInitialization, AssociatedInteractiveObject);
        }

        public override void Tick(float d)
        {
            base.Tick(d);
            this.RangeWorldPositionChangeSystem.Tick(d);
        }

        public override void ReceiveEvent(RangeTransformChanged RangeTransformChanged)
        {
            RangeObjectV2Commons.HandlingRangeTransformChangedForFrustums(this.RangeGameObjectV2, this.FrustumRangeWorldPositionCalulcationSystem);
        }
    }

    public struct SetWorldPositionEvent
    {
        public Vector3 WorldPosition;
    }

    public static class RangeObjectV2Commons
    {
        public static void HandlingRangeTransformChangedForFrustums(RangeGameObjectV2 RangeGameObjectV2, FrustumRangeWorldPositionCalulcationSystem FrustumRangeWorldPositionCalulcationSystem)
        {
            var RangeObjectV2GetWorldTransformEventReturn = RangeGameObjectV2.GetTransform();
            FrustumRangeWorldPositionCalulcationSystem.ReceiveEvent(new FrustumWorldPositionRecalculation
            {
                WorldPosition = RangeObjectV2GetWorldTransformEventReturn.WorldPosition,
                WorldRotation = RangeObjectV2GetWorldTransformEventReturn.WorldRotation,
                LossyScale = RangeObjectV2GetWorldTransformEventReturn.LossyScale
            });
        }
    }
}
