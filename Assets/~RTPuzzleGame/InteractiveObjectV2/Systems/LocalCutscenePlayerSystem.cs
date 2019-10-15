using CoreGame;
using System;
using System.Collections.Generic;

namespace InteractiveObjectTest
{
    public class LocalCutscenePlayerSystem : AInteractiveObjectSystem
    {
        private SequencedActionPlayer CurrentPlayingCutscene;

        public override void TickAlways(float d)
        {
            if (this.CurrentPlayingCutscene != null) { this.CurrentPlayingCutscene.Tick(d); }
        }

        public void PlayCutscene(List<SequencedAction> SequencedActions, Action OnCutsceneEnded = null, Action OnCutsceneKilled = null)
        {
            //A local cutscene is already playing
            if (this.CurrentPlayingCutscene != null)
            {
                this.CurrentPlayingCutscene.Kill();
            }

            this.CurrentPlayingCutscene = new SequencedActionPlayer(SequencedActions, null,
                OnCutsceneEnded: () =>
                {
                    this.OnCutsceneEndedOrKilled();
                    if (OnCutsceneEnded != null) { OnCutsceneEnded.Invoke(); }
                },
                OnCutsceneKilled: () =>
                {
                    this.OnCutsceneEndedOrKilled();
                    if (OnCutsceneKilled != null) { OnCutsceneKilled.Invoke(); }
                });
            this.CurrentPlayingCutscene.Play();
        }

        public void KillCurrentCutscene()
        {
            this.CurrentPlayingCutscene.Kill();
        }

        private void OnCutsceneEndedOrKilled()
        {
            this.CurrentPlayingCutscene = null;
        }

    }
}
