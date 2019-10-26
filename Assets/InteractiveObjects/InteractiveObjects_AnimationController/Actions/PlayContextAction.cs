using System;
using System.Collections.Generic;
using AnimatorPlayable;
using SequencedAction;

namespace InteractiveObjects_AnimationController
{
    public class PlayContextAction : ASequencedAction
    {
        private AnimationController AnimationController;
        private SequencedAnimationInput SequencedAnimationInput;

        private bool AnimationFinished;

        public PlayContextAction(AnimationController AnimationController, SequencedAnimationInput SequencedAnimationInput, Func<List<ASequencedAction>> nextActionsDeffered) : base(nextActionsDeffered)
        {
            this.AnimationController = AnimationController;
            this.SequencedAnimationInput = SequencedAnimationInput;
            this.AnimationFinished = false;
        }

        public override void FirstExecutionAction()
        {
            this.AnimationFinished = false;
            this.AnimationController.PlayContextAction(this.SequencedAnimationInput, () => { this.AnimationFinished = true; });
        }

        public override bool ComputeFinishedConditions()
        {
            return this.AnimationFinished;
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override void Tick(float d)
        {
        }

        public override void Interupt()
        {
            this.AnimationController.KillContextAction(this.SequencedAnimationInput);
            base.Interupt();
        }
    }
}