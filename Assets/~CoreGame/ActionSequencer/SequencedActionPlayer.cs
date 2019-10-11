using System;
using System.Collections.Generic;

namespace CoreGame
{
    public class SequencedActionPlayer
    {
        private SequencedActionManager SequencedActionManager;
        private SequencedActionInput SequencedActionInput;
        public List<SequencedAction> SequencedActions;

        private Action OnCutsceneEnded;
        private Action OnCutsceneKilled;

        public SequencedActionPlayer(List<SequencedAction> SequencedActions, SequencedActionInput SequencedActionInput, Action OnCutsceneEnded = null, Action OnCutsceneKilled = null)
        {
            this.OnCutsceneEnded = OnCutsceneEnded;
            this.OnCutsceneKilled = OnCutsceneKilled;

            this.SequencedActions = SequencedActions;
            this.SequencedActionInput = SequencedActionInput;
            this.SequencedActionManager = new SequencedActionManager((action) => this.SequencedActionManager.OnAddAction(action, this.SequencedActionInput), null, this.OnCutsceneEnded);
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
            if (this.OnCutsceneKilled != null) { this.OnCutsceneKilled.Invoke(); }
        }

        public bool IsPlaying()
        {
            return this.SequencedActionManager.IsPlaying();
        }

        public List<SequencedAction> GetCurrentActions(bool includeWorkflowNested = false)
        {
            return this.SequencedActionManager.GetCurrentActions(includeWorkflowNested);
        }
    }
}
