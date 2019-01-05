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
        isActionEnded = false;
        var giveActionInput = (GiveActionInput)ContextActionInput;
        if (giveActionInput.TargetPOI != null && giveActionInput.TargetPOI.IsElligibleToGiveItem(itemGiven))
        {
            //TODO process to give
            //TODO animation
            Debug.Log("Elligible");
            isActionEnded = true;
        }
        else
        {
            StartCoroutine(AnimationPlayerHelper.Play(giveActionInput.PlayerAnimator, PlayerAnimatioNnamesEnum.PLAYER_ACTION_FORBIDDEN, 0f, () =>
            {
                isActionEnded = true;
            }));
        }
    }


    public override void OnStart()
    {
        itemGiven = GetComponentInParent<Item>();
    }

    public override void Tick(float d)
    {
    }
}


public class GiveActionInput : AContextActionInput
{
    private PointOfInterestType targetPOI;
    private Animator playerAnimator;

    public GiveActionInput(PointOfInterestType targetPOI, Animator playerAnimator)
    {
        this.targetPOI = targetPOI;
        this.playerAnimator = playerAnimator;
    }

    public PointOfInterestType TargetPOI { get => targetPOI; }
    public Animator PlayerAnimator { get => playerAnimator; }
}