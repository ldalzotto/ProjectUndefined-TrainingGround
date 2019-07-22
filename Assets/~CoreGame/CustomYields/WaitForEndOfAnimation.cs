using UnityEngine;

public class WaitForEndOfAnimation : CustomYieldInstruction
{

    private Animator animator;
    private int animatorLayer;
    private string animationName;
    private bool framePerfectEndDetection;

    public WaitForEndOfAnimation(Animator animator, string animationName, int animatorLayer, bool framePerfectEndDetection = false)
    {
        this.animator = animator;
        this.animationName = animationName;
        this.animatorLayer = animatorLayer;
        this.framePerfectEndDetection = framePerfectEndDetection;
    }

    public override bool keepWaiting
    {
        get
        {
            var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(animatorLayer);
            var nexAnimatorStateInfo = animator.GetNextAnimatorStateInfo(animatorLayer);
            bool currentAnimatorStateIsCorrectName = currentAnimatorStateInfo.IsName(animationName);
            bool nextAnimatorStateIsCorrectName = nexAnimatorStateInfo.IsName(animationName);

            bool isCurrentAnimationEndedFramePerfrect = true;
            if (this.framePerfectEndDetection)
            {
                isCurrentAnimationEndedFramePerfrect = currentAnimatorStateInfo.normalizedTime <= 1 - (Time.deltaTime * 2 / currentAnimatorStateInfo.length);
            }

            return (currentAnimatorStateIsCorrectName && isCurrentAnimationEndedFramePerfrect) || nextAnimatorStateIsCorrectName;
        }
    }
}
