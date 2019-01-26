using UnityEngine;

public class AnimatorAction : AContextAction
{

    private bool animationEnded;

    public AnimatorAction(AContextAction nextContextAction) : base(nextContextAction)
    {

    }

    public override void AfterFinishedEventProcessed()
    {
    }

    public override bool ComputeFinishedConditions()
    {
        return animationEnded;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        animationEnded = false;
        var animatorActionInput = (AnimatorActionInput)ContextActionInput;

        animatorActionInput.PlayerManager.StartCoroutine(AnimationPlayerHelper.Play(animatorActionInput.PlayerAnimator, animatorActionInput.PlayerAnimationEnum, 0f, () =>
        {
            animationEnded = true;
        }));


    }

    public override void Tick(float d)
    {
    }
}

public class AnimatorActionInput : AContextActionInput
{
    private Animator playerAnimator;
    private PlayerAnimatioNnamesEnum playerAnimationEnum;
    private PlayerManager playerManager;

    public AnimatorActionInput(Animator playerAnimator, PlayerAnimatioNnamesEnum playerAnimationEnum, PlayerManager playerManager)
    {
        this.playerAnimator = playerAnimator;
        this.playerAnimationEnum = playerAnimationEnum;
        this.playerManager = playerManager;
    }

    public Animator PlayerAnimator { get => playerAnimator; }
    public PlayerAnimatioNnamesEnum PlayerAnimationEnum { get => playerAnimationEnum; }
    public PlayerManager PlayerManager { get => playerManager; }
}