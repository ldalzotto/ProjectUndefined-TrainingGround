using UnityEngine;

public class AniamtionBuilderTestModelScript : MonoBehaviour
{

    public string PositioningAnimationStateName;

    private Animator Animator;

    void Start()
    {
        this.Animator = GetComponent<Animator>();

        this.Animator.Play(this.PositioningAnimationStateName);
        this.Animator.Update(0f); //take the play into account
        this.Animator.Update(this.Animator.GetCurrentAnimatorStateInfo(0).length);
    }

}
