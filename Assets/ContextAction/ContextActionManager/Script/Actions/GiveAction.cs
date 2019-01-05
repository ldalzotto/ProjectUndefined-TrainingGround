using UnityEngine;

public class GiveAction : AContextAction
{
    private Item itemGiven;

    private bool isActionEnded;

    public override bool ComputeFinishedConditions()
    {
        return isActionEnded;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        var giveActionInput = (GiveActionInput)ContextActionInput;
        if (giveActionInput.TargetPOI != null && giveActionInput.TargetPOI.IsElligibleToGiveItem(itemGiven))
        {
            //TODO process to give
            Debug.Log("Elligible");
            isActionEnded = true;
        }
        else
        {
            //TODO, the NO animation
            Debug.Log("Not Elligible");
            isActionEnded = true;
        }
    }

    public override void OnStart()
    {
        isActionEnded = false;
        itemGiven = GetComponentInParent<Item>();
    }

    public override void Tick(float d)
    {
    }
}


public class GiveActionInput : AContextActionInput
{
    private PointOfInterestType targetPOI;

    public GiveActionInput(PointOfInterestType targetPOI)
    {
        this.targetPOI = targetPOI;
    }

    public PointOfInterestType TargetPOI { get => targetPOI; }
}