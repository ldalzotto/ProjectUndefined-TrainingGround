using System;

namespace CoreGame
{
    public class SequencedActionPlayer
    {
        private SequencedActionManager SequencedActionManager;
        private SequencedActionInput SequencedActionInput;
        private AbstractCutsceneGraph AbstractCutsceneGraph;

        public SequencedActionPlayer(AbstractCutsceneGraph cutsceneGraph, SequencedActionInput SequencedActionInput, Action onFinished = null)
        {
            AbstractCutsceneGraph = cutsceneGraph;
            this.SequencedActionInput = SequencedActionInput;
            this.SequencedActionManager = new SequencedActionManager((action) => this.SequencedActionManager.OnAddAction(action, this.SequencedActionInput), null, onFinished);
        }

        public void Play()
        {
            this.SequencedActionManager.OnAddActions(this.AbstractCutsceneGraph.GetRootActions(), this.SequencedActionInput);
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
        }

        public bool IsPlaying()
        {
            return this.SequencedActionManager.IsPlaying();
        }
    }

}
