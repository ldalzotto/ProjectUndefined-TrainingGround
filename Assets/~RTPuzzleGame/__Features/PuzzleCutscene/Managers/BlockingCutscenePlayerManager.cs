using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public class BlockingCutscenePlayerManager : MonoBehaviour
    {
        private bool playing;

        public bool Playing { get => playing; }

        private SequencedActionManager cutscenePlayer;

        public void Play(List<SequencedAction> SequencingActions, Action onCutsceneEnd = null)
        {
            this.playing = true;
            this.cutscenePlayer = new SequencedActionManager((action) => this.cutscenePlayer.OnAddAction(action, null), null, OnNoMoreActionToPlay: () =>
            {
                this.cutscenePlayer = null;
                this.playing = false;
                if (onCutsceneEnd != null) { onCutsceneEnd.Invoke(); }
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
