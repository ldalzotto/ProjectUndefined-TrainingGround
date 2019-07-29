using UnityEngine;

public class SpeakerPlayingAnimationBehavior : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    public float DeltaTimeThresholdForChange = 1f;

    [Range(0f,1f)]
    public float SmoothDamp = 0.5f;

    private float CurrentDynamicSize = 0f;
    private float TargetDynamicSize = 0f;
    private float elapsedTime = 0f;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.elapsedTime += Time.deltaTime;
        if(this.elapsedTime >= DeltaTimeThresholdForChange)
        {
            this.elapsedTime -= DeltaTimeThresholdForChange;
            this.TargetDynamicSize = Random.Range(0f, 1f);
        }
        this.CurrentDynamicSize = Mathf.Lerp(this.CurrentDynamicSize, this.TargetDynamicSize, this.SmoothDamp);

        animator.SetFloat("SpeakerSize", this.CurrentDynamicSize);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
