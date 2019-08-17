using CoreGame;
using System;
using UnityEngine;

namespace RTPuzzle
{
    public class BlockingCutscenePlayer : MonoBehaviour
    {
        #region External Dependencies
        private PlayerManagerDataRetriever PlayerManagerDataRetriever;
        #endregion

        public void Init()
        {
            this.PlayerManagerDataRetriever = GameObject.FindObjectOfType<PlayerManagerDataRetriever>();
        }

        private bool playing;

        public bool Playing { get => playing; }

        private SequencedActionManager cutscenePlayer;
        
        public void Play(PuzzleCutsceneActionInput puzzleCutsceneActionInput, PuzzleCutsceneGraph puzzleCutsceneGraph, Action onCutsceneEnd = null)
        {
            this.PlayerManagerDataRetriever.GetPlayerRigidBody().constraints = RigidbodyConstraints.FreezeAll;
            this.playing = true;
            this.cutscenePlayer = new SequencedActionManager((action) => this.cutscenePlayer.OnAddAction(action, puzzleCutsceneActionInput), null, OnNoMoreActionToPlay: () => {
                this.cutscenePlayer = null;
                this.playing = false;
                if(onCutsceneEnd != null) { onCutsceneEnd.Invoke(); }
                this.PlayerManagerDataRetriever.GetPlayerRigidBody().constraints = RigidbodyConstraints.FreezeRotation;
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
