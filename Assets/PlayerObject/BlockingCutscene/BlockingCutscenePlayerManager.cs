using System;
using System.Collections.Generic;
using CoreGame;

namespace PlayerObject
{
    public class BlockingCutscenePlayerManager : GameSingleton<BlockingCutscenePlayerManager>
    {
        public bool Playing { get; private set; }

        [VE_Ignore] private SequencedActionManager cutscenePlayer;

        public void Play(List<SequencedAction> SequencingActions, Action onCutsceneEnd = null)
        {
            this.Playing = true;
            this.cutscenePlayer = new SequencedActionManager((action) => this.cutscenePlayer.OnAddAction(action, null), null, OnNoMoreActionToPlay: () =>
            {
                this.cutscenePlayer = null;
                this.Playing = false;
                if (onCutsceneEnd != null)
                {
                    onCutsceneEnd.Invoke();
                }
            });
            this.cutscenePlayer.OnAddActions(SequencingActions, null);
        }

        public void Tick(float d)
        {
            if (this.cutscenePlayer != null)
            {
                this.cutscenePlayer.Tick(d);
            }
        }
    }
}