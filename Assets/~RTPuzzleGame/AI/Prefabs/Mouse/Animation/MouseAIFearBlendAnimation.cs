using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class MouseAIFearBlendAnimation : StateMachineBehaviour
    {

        private float FearBlendPosition = 0;
        private float timeCounter = 0f;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            this.FearBlendPosition = Random.Range(0f, 1f);
            animator.SetFloat("FearBlendPosition", this.FearBlendPosition);
            this.timeCounter = 0f;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            this.timeCounter += Time.deltaTime;
            if (this.timeCounter >= 0.2f)
            {
                this.FearBlendPosition = Random.Range(0f, 1f);
                animator.SetFloat("FearBlendPosition", this.FearBlendPosition);
                this.timeCounter -= 0.1f;
            }
        }
    }

}
