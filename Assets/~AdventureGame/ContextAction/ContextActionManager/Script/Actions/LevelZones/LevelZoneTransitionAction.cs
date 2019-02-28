using UnityEngine;

public class LevelZoneTransitionAction : AContextAction
{

    private LevelZonesID nextZone;

    public LevelZoneTransitionAction(LevelZonesID nextZone) : base(null)
    {
        this.nextZone = nextZone;
    }

    public LevelZonesID NextZone { get => nextZone; }

    public override void AfterFinishedEventProcessed()
    {

    }

    public override bool ComputeFinishedConditions()
    {
        return true;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        var LevelZonesEventManager = GameObject.FindObjectOfType<AdventureLevelZonesEventManager>();
        LevelZonesEventManager.OnLevelZoneChange(nextZone);
    }

    public override void Tick(float d)
    {

    }
}
