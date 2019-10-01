﻿using CoreGame;
using RTPuzzle;
using UnityEngine;

public abstract class ARangeObjectSystem
{
    protected RangeObjectV2 RangeObjectV2Ref;

    protected ARangeObjectSystem(RangeObjectV2 rangeObjectV2Ref)
    {
        RangeObjectV2Ref = rangeObjectV2Ref;
    }

    public virtual void Tick(float d) { }
}

#region Range Type Object Movement
public class RangeTypeObjectMovementSystem : ARangeObjectSystem
{
    private bool localPositionChanged;
    private Vector3 localPosition;

    public RangeTypeObjectMovementSystem(RangeObjectV2 rangeObjectV2Ref) : base(rangeObjectV2Ref)
    {
    }

    public void ReceiveEvent(RangeTypeObjectSetLocalPositionEvent RangeTypeObjectSetLocalPositionEvent)
    {
        this.localPositionChanged = true;
        this.localPosition = RangeTypeObjectSetLocalPositionEvent.LocalPosition;
    }
    public void ExtractData(out RangeTypeObjectMovementSystemExtractedData RangeTypeObjectMovementSystemExtractedData)
    {
        RangeTypeObjectMovementSystemExtractedData =
            new RangeTypeObjectMovementSystemExtractedData
            { LocalPosition = this.localPosition, LocalPositionChanged = this.localPositionChanged };
    }
}

public struct RangeTypeObjectSetLocalPositionEvent
{
    public Vector3 LocalPosition;
}
public struct RangeTypeObjectMovementSystemExtractedData
{
    public bool LocalPositionChanged;
    public Vector3 LocalPosition;
}

public class RangeWorldPositionChangeSystem : ARangeObjectSystem
{
    private BlittableTransformChangeListenerManager BlittableTransformChangeListenerManager;
    private RangeWorldPositionChangeListener RangeWorldPositionChangeListener;

    public RangeWorldPositionChangeSystem(RangeObjectV2 rangeObjectV2Ref) : base(rangeObjectV2Ref)
    {
        this.RangeWorldPositionChangeListener = new RangeWorldPositionChangeListener();
        this.BlittableTransformChangeListenerManager = new BlittableTransformChangeListenerManager(true, true, this.RangeWorldPositionChangeListener);
    }
    
    public override void Tick(float d)
    {
        this.RangeWorldPositionChangeListener.Clear();
        this.RangeObjectV2Ref.ExtractData(out RangeObjectV2GetWorldTransformEventReturn RangeObjectV2GetWorldTransformEventReturn);
        this.BlittableTransformChangeListenerManager.Tick(RangeObjectV2GetWorldTransformEventReturn.WorldPosition, RangeObjectV2GetWorldTransformEventReturn.WorldRotation);
        if (this.RangeWorldPositionChangeListener.hasChanged)
        {
            this.RangeObjectV2Ref.ReceiveEvent(new RangeTransformChanged { });
        }
    }
}

public class RangeWorldPositionChangeListener : TransformChangeListener
{
    public bool hasChanged { get; private set; }
    public RangeWorldPositionChangeListener()
    {
    }

    public void Clear() { this.hasChanged = false; }

    public void onPositionChange()
    {
        this.hasChanged = true;
    }

    public void onRotationChange()
    {
        this.hasChanged = true;
    }
}

public struct RangeTransformChanged { }
#endregion

#region Range Type
public abstract class ARangeTypeDefinitionV2 { }
[System.Serializable]
public class SphereRangeTypeDefinition : ARangeTypeDefinitionV2
{
    public float Radius;
}
[System.Serializable]
public class BoxRangeTypeDefinition : ARangeTypeDefinitionV2
{
    public Vector3 Center;
    public Vector3 Size;
}
[System.Serializable]
public class FrustumRangeTypeDefinition : ARangeTypeDefinitionV2
{
    public FrustumV2 FrustumV2;
}
#endregion

#region Range Obstacle Listener
public class RangeObstacleListenerSystem : ARangeObjectSystem
{
    private RangeObstaclePhysicsEventListener RangeObstaclePhysicsEventListener;

    public RangeObstacleListenerSystem(RangeObjectV2 rangeObjectV2Ref, RangeObjectV2PhysicsEventListener RangeObjectV2PhysicsEventListener, ObstacleListener obstacleListener) : base(rangeObjectV2Ref)
    {
        this.RangeObstaclePhysicsEventListener = new RangeObstaclePhysicsEventListener(obstacleListener);
        RangeObjectV2PhysicsEventListener.AddPhysicsEventListener(this.RangeObstaclePhysicsEventListener);
    }
}
public class RangeObstaclePhysicsEventListener : ARangeObjectV2PhysicsEventListener
{
    private ObstacleListener AssociatedObstacleListener;

    public RangeObstaclePhysicsEventListener(ObstacleListener associatedObstacleListener)
    {
        AssociatedObstacleListener = associatedObstacleListener;
    }

    public override void OnTriggerEnter(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
    {
        if (PhysicsTriggerInfo.OtherObstacle != null) { this.AssociatedObstacleListener.AddNearSquareObstacle(PhysicsTriggerInfo.OtherObstacle); }
    }

    public override void OnTriggerExit(RangeObjectPhysicsTriggerInfo PhysicsTriggerInfo)
    {
        if (PhysicsTriggerInfo.OtherObstacle != null) { this.AssociatedObstacleListener.RemoveNearSquareObstacle(PhysicsTriggerInfo.OtherObstacle); }
    }
}
#endregion

#region Frustum Specifics Systems
public class FrustumRangeWorldPositionCalulcationSystem : ARangeObjectSystem
{
    private FrustumV2 FrustumV2;

    public FrustumRangeWorldPositionCalulcationSystem(RangeObjectV2 rangeObjectV2Ref, FrustumRangeTypeDefinition FrustumRangeTypeDefinition) : base(rangeObjectV2Ref)
    {
        this.FrustumV2 = FrustumRangeTypeDefinition.FrustumV2.Clone();
    }

    public void ReceiveEvent(FrustumWorldPositionRecalculation FrustumWorldPositionRecalculation)
    {
        this.FrustumV2.SetCalculationDataForFaceBasedCalculation(FrustumWorldPositionRecalculation.WorldPosition, FrustumWorldPositionRecalculation.WorldRotation, FrustumWorldPositionRecalculation.LossyScale);
    }
}

public struct FrustumWorldPositionRecalculation
{
    public Vector3 WorldPosition;
    public Quaternion WorldRotation;
    public Vector3 LossyScale;
}
#endregion