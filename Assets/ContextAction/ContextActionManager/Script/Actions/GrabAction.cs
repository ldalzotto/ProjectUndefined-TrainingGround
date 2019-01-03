using System.Collections;
using UnityEngine;

public class GrabAction : AContextAction
{
    private GrabActionInput grabActionInput;
    private bool animationEnded;
    public override bool ComputeFinishedConditions()
    {
        return animationEnded;
    }

    public override void FirstExecutionAction(AContextActionInput ContextActionInput)
    {
        animationEnded = false;
        grabActionInput = (GrabActionInput)ContextActionInput;
        StartCoroutine(GrabActionCoroutine());
    }

    public override void Tick(float d)
    {

    }

    private IEnumerator GrabActionCoroutine()
    {
        grabActionInput.PlayerAnimator.Play(grabActionInput.AnimationName);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfAnimation(grabActionInput.PlayerAnimator, grabActionInput.AnimationName, grabActionInput.LayerIndex);
        animationEnded = true;
    }
}

public class GrabActionInput : AContextActionInput
{

    private Animator playerAnimator;
    private string animationName;
    private int layerIndex;

    public GrabActionInput(Animator playerAnimator, string animationName, int layerIndex)
    {
        this.playerAnimator = playerAnimator;
        this.animationName = animationName;
        this.layerIndex = layerIndex;
    }

    public Animator PlayerAnimator { get => playerAnimator; }
    public string AnimationName { get => animationName; }
    public int LayerIndex { get => layerIndex; }
}