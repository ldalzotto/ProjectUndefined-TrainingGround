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
            return (animator.GetCurrentAnimatorStateInfo(animatorLayer).IsName(animationName));
        }
    }
}
