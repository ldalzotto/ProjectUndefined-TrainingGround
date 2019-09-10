using System;
using System.Collections.Generic;

namespace CoreGame
{
    public class SequencedActionPlayer
    {
        private SequencedActionManager SequencedActionManager;
        private SequencedActionInput SequencedActionInput;
        public List<SequencedAction> SequencedActions;

        public SequencedActionPlayer(List<SequencedAction> SequencedActions, SequencedActionInput SequencedActionInput, Action onFinished = null)
        {
            this.SequencedActions = SequencedActions;
            this.SequencedActionInput = SequencedActionInput;
            this.SequencedActionManager = new SequencedActionManager((action) => this.SequencedActionManager.OnAddAction(action, this.SequencedActionInput), null, onFinished);
        }



        public void Play()
        {
            this.SequencedActionManager.OnAddActions(this.SequencedActions, this.SequencedActionInput);
        }

        public void Tick(float d)
        {
            if (this.SequencedActionManager.IsPlaying())
            {
                this.SequencedActionManager.Tick(d);
            }
        }

        public void Kill()
        {
            this.SequencedActionManager.InterruptAllActions();
            this.SequencedActionManager.CleatAllActions();
        }

        public bool IsPlaying()
        {
            return this.SequencedActionManager.IsPlaying();
        }
    }

}
