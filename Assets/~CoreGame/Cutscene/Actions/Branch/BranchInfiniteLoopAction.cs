using System;
using System.Collections.Generic;

namespace CoreGame
{
    [System.Serializable]
    public class BranchInfiniteLoopAction : SequencedAction
    {
        [NonSerialized]
        private List<SequencedAction> loopActions;
        [NonSerialized]
        private SequencedActionPlayer loopActionPlayer;

        public BranchInfiniteLoopAction(List<SequencedAction> nextActions) : base(new List<SequencedAction>())
        {
            this.loopActions = nextActions;
        }

        public override void SetNextContextAction(List<SequencedAction> nextActions)
        {
            this.loopActions = nextActions;
            base.SetNextContextAction(new List<SequencedAction>());
        }

        [NonSerialized]
        private bool hasEnded;

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.hasEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.hasEnded = false;
            this.CreateAndPlayLoopAction(ContextActionInput);
        }

        private void CreateAndPlayLoopAction(SequencedActionInput ContextActionInput)
        {
            this.loopActionPlayer = new SequencedActionPlayer(this.loopActions, ContextActionInput, onFinished: () => { this.CreateAndPlayLoopAction(ContextActionInput); });
            this.loopActionPlayer.Play();
        }

        public override void Tick(float d)
        {
            if (this.loopActionPlayer != null) { this.loopActionPlayer.Tick(d); }
        }

        public override void Interupt()
        {
            this.hasEnded = true;
            if (this.loopActionPlayer != null) { this.loopActionPlayer.Kill(); }
        }
    }
}