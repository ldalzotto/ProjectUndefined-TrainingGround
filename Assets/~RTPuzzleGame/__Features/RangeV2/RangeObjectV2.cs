using CoreGame;
using GameConfigurationID;
using UnityEngine;
using UnityEngine.Profiling;

public abstract class RangeObjectV2
{
    public RangeGameObjectV2 RangeGameObjectV2 { get; private set; }

    public RangeObjectInitialization RangeObjectInitialization { get; private set; }

    private RangeTypeObjectMovementSystem RangeTypeObjectMovementSystem;
    public RangeObstacleListenerSystem RangeObstacleListenerSystem { get; private set; }

    public virtual void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
    {
        this.RangeGameObjectV2 = RangeGameObjectV2;
        this.RangeObjectInitialization = RangeObjectInitialization;

        this.RangeTypeObjectMovementSystem = new RangeTypeObjectMovementSystem(this);

        if (RangeObjectInitialization.IsTakingIntoAccountObstacles)
        {
            this.RangeObstacleListenerSystem = new RangeObstacleListenerSystem(this, this.RangeGameObjectV2.RangeObjectV2PhysicsEventListener, this.RangeGameObjectV2.ObstacleListener);
        }
        
    }

    public virtual void Tick(float d)
    {
        Profiler.BeginSample("RangeObjectV2 : Tick");
        this.RangeGameObjectV2.Tick(d);
        Profiler.EndSample();
    }

    public void ReceiveEvent(SetWorldPositionEvent SetWorldPositionEvent)
    {
        this.RangeGameObjectV2.ReceiveEvent(out RangeObjectV2GetWorldToLocalMatrixEventReturn RangeObjectV2GetWorldToLocalMatrixEventReturn);
        var localPosition = RangeObjectV2GetWorldToLocalMatrixEventReturn.WorldToLocalMatrix.MultiplyVector(SetWorldPositionEvent.WorldPosition);
        this.RangeTypeObjectMovementSystem.ReceiveEvent(new RangeTypeObjectSetLocalPositionEvent { LocalPosition = localPosition });
    }
    public virtual void ReceiveEvent(RangeTransformChanged RangeTransformChanged) { }

    public void ExtractData(out RangeObjectV2GetWorldTransformEventReturn RangeObjectV2GetWorldTransformEventReturn) { this.RangeGameObjectV2.ExtractData(out RangeObjectV2GetWorldTransformEventReturn); }
    public void ExtractData(out RangeTypeObjectMovementSystemExtractedData RangeTypeObjectMovementSystemExtractedData)
    {
        this.RangeTypeObjectMovementSystem.ExtractData(out RangeTypeObjectMovementSystemExtractedData);
    }
}

public class SphereRangeObjectV2 : RangeObjectV2
{
    private SphereRangeObjectInitialization SphereRangeObjectInitialization;
    public SphereCollider SphereBoundingCollider { get; private set; }

    public SphereRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, SphereRangeObjectInitialization SphereRangeObjectInitialization)
    {
        this.SphereRangeObjectInitialization = SphereRangeObjectInitialization;
        this.Init(RangeGameObjectV2, SphereRangeObjectInitialization);
        this.SphereBoundingCollider = (SphereCollider)RangeGameObjectV2.BoundingCollider;
    }

    public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
    {
        RangeGameObjectV2.Init(this.SphereRangeObjectInitialization, this);
        base.Init(RangeGameObjectV2, RangeObjectInitialization);
    }

}

public class BoxRangeObjectV2 : RangeObjectV2
{
    private BoxRangeObjectInitialization BoxRangeObjectInitialization;
    public BoxCollider BoxBoundingCollider { get; private set; }

    public BoxRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, BoxRangeObjectInitialization BoxRangeObjectInitialization)
    {
        this.BoxRangeObjectInitialization = BoxRangeObjectInitialization;
        this.Init(RangeGameObjectV2, BoxRangeObjectInitialization);
        this.BoxBoundingCollider = (BoxCollider)RangeGameObjectV2.BoundingCollider;
    }

    public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
    {
        RangeGameObjectV2.Init(this.BoxRangeObjectInitialization, this);
        base.Init(RangeGameObjectV2, RangeObjectInitialization);
    }
}

public class FrustumRangeObjectV2 : RangeObjectV2
{
    private FrustumRangeObjectInitialization FrustumRangeObjectInitialization;

    private RangeWorldPositionChangeSystem RangeWorldPositionChangeSystem;
    private FrustumRangeWorldPositionCalulcationSystem FrustumRangeWorldPositionCalulcationSystem;

    public FrustumV2 GetFrustum() { return this.FrustumRangeWorldPositionCalulcationSystem.FrustumV2; }

    public FrustumRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, FrustumRangeObjectInitialization FrustumRangeObjectInitialization)
    {
        this.RangeWorldPositionChangeSystem = new RangeWorldPositionChangeSystem(this);
        this.FrustumRangeWorldPositionCalulcationSystem = new FrustumRangeWorldPositionCalulcationSystem(this, FrustumRangeObjectInitialization.FrustumRangeTypeDefinition.FrustumV2);

        this.FrustumRangeObjectInitialization = FrustumRangeObjectInitialization;
        this.Init(RangeGameObjectV2, FrustumRangeObjectInitialization);
    }

    public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
    {
        RangeGameObjectV2.Init(this.FrustumRangeObjectInitialization, this);
        base.Init(RangeGameObjectV2, RangeObjectInitialization);
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

    public RoundedFrustumRangeObjectV2(RangeGameObjectV2 RangeGameObjectV2, RoundedFrustumRangeObjectInitialization RoundedFrustumRangeObjectInitialization)
    {
        this.RangeWorldPositionChangeSystem = new RangeWorldPositionChangeSystem(this);
        this.FrustumRangeWorldPositionCalulcationSystem = new FrustumRangeWorldPositionCalulcationSystem(this, RoundedFrustumRangeObjectInitialization.RoundedFrustumRangeTypeDefinition.FrustumV2);

        this.RoundedFrustumRangeObjectInitialization = RoundedFrustumRangeObjectInitialization;
        this.Init(RangeGameObjectV2, RoundedFrustumRangeObjectInitialization);
    }

    public override void Init(RangeGameObjectV2 RangeGameObjectV2, RangeObjectInitialization RangeObjectInitialization)
    {
        RangeGameObjectV2.Init(this.RoundedFrustumRangeObjectInitialization, this);
        base.Init(RangeGameObjectV2, RangeObjectInitialization);
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
        RangeGameObjectV2.ExtractData(out RangeObjectV2GetWorldTransformEventReturn RangeObjectV2GetWorldTransformEventReturn);
        FrustumRangeWorldPositionCalulcationSystem.ReceiveEvent(new FrustumWorldPositionRecalculation
        {
            WorldPosition = RangeObjectV2GetWorldTransformEventReturn.WorldPosition,
            WorldRotation = RangeObjectV2GetWorldTransformEventReturn.WorldRotation,
            LossyScale = RangeObjectV2GetWorldTransformEventReturn.LossyScale
        });
    }
}