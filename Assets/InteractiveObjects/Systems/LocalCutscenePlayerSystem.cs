using System;
using System.Collections.Generic;
using CoreGame;

namespace InteractiveObjects
{
    public class LocalCutscenePlayerSystem : AInteractiveObjectSystem
    {
        private SequencedActionPlayer CurrentPlayingCutscene;

        public override void TickAlways(float d)
        {
            if (CurrentPlayingCutscene != null) CurrentPlayingCutscene.Tick(d);
        }

        public void PlayCutscene(List<SequencedAction> SequencedActions, Action OnCutsceneEnded = null, Action OnCutsceneKilled = null)
        {
            //A local cutscene is already playing
            if (CurrentPlayingCutscene != null) CurrentPlayingCutscene.Kill();

            CurrentPlayingCutscene = new SequencedActionPlayer(SequencedActions, null,
                () =>
                {
                    OnCutsceneEndedOrKilled();
                    if (OnCutsceneEnded != null) OnCutsceneEnded.Invoke();
                },
                () =>
                {
                    OnCutsceneEndedOrKilled();
                    if (OnCutsceneKilled != null) OnCutsceneKilled.Invoke();
                });
            CurrentPlayingCutscene.Play();
        }

        public void KillCurrentCutscene()
        {
            CurrentPlayingCutscene.Kill();
        }

        private void OnCutsceneEndedOrKilled()
        {
            CurrentPlayingCutscene = null;
        }
    }
}