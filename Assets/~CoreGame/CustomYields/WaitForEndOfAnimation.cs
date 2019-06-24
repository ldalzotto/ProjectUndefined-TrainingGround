using UnityEngine;

public class WaitForEndOfAnimation : CustomYieldInstruction
{

    private Animator animator;
    private int animatorLayer;
    private string animationName;

    public WaitForEndOfAnimation(Animator animator, string animationName, int animatorLayer)
    {
        this.animator = animator;
        this.animationName = animationName;
        this.animatorLayer = animatorLayer;
    }

    public override bool keepWaiting
    {
        get
        {
            var currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(animatorLayer);
            var nexAnimatorStateInfo = animator.GetNextAnimatorStateInfo(animatorLayer);
            bool currentAnimatorStateIsCorrectName = currentAnimatorStateInfo.IsName(animationName);
            bool nextAnimatorStateIsCorrectName = nexAnimatorStateInfo.IsName(animationName);

            return currentAnimatorStateIsCorrectName || nextAnimatorStateIsCorrectName;


            //            return currentAnimatorStateIsCorrectName || nextAnimatorStateIsCorrectName;
            //  (animator.GetCurrentAnimatorStateInfo(animatorLayer).normalizedTime < 1);
        }
    }
}
