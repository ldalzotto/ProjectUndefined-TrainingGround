using System.Collections;
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
            //TODO animation
            Debug.Log("Elligible");
            isActionEnded = true;
        }
        else
        {
            StartCoroutine(NonElligibleAnimationCoroutine(giveActionInput.PlayerAnimator));
        }
    }

    private IEnumerator NonElligibleAnimationCoroutine(Animator playerAnimator)
    {
        var animationName = AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_ACTIOn_FORBIDDEN].AnimationName;
        var animationlayer = AnimationConstants.PlayerAnimationConstants[PlayerAnimatioNnamesEnum.PLAYER_ACTIOn_FORBIDDEN].LayerIndex;
        playerAnimator.CrossFade(animationName, 0.2f);
        //  playerAnimator.Play(animationName);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(playerAnimator, animationName, animationlayer);
        isActionEnded = true;
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
    private Animator playerAnimator;

    public GiveActionInput(PointOfInterestType targetPOI, Animator playerAnimator)
    {
        this.targetPOI = targetPOI;
        this.playerAnimator = playerAnimator;
    }

    public PointOfInterestType TargetPOI { get => targetPOI; }
    public Animator PlayerAnimator { get => playerAnimator; }
}