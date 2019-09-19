using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class BlockingCutscenePlayerManager : MonoBehaviour
    {
        private bool playing;

        public bool Playing { get => playing; }

        private SequencedActionManager cutscenePlayer;

        public void Play(PuzzleCutsceneActionInput puzzleCutsceneActionInput, PuzzleCutsceneGraph puzzleCutsceneGraph, Action onCutsceneEnd = null)
        {
            this.playing = true;
            this.cutscenePlayer = new SequencedActionManager((action) => this.cutscenePlayer.OnAddAction(action, puzzleCutsceneActionInput), null, OnNoMoreActionToPlay: () =>
            {
                this.cutscenePlayer = null;
                this.playing = false;
                if (onCutsceneEnd != null) { onCutsceneEnd.Invoke(); }
            });
            this.cutscenePlayer.OnAddActions(puzzleCutsceneGraph.GetRootActions(), puzzleCutsceneActionInput);
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
