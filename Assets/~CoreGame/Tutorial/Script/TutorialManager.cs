﻿using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class TutorialManager : MonoBehaviour, TutorialStepManagerEventListener
    {
        private TutorialActionInput TutorialActionInput;

        #region External Dependencies
        private TutorialStepConfiguration TutorialStepConfiguration;
        #endregion

        #region Internal Managers
        private TutorialStatePersister TutorialStatePersister;
        #endregion

        #region State
        private List<TutorialStepID> TutorialStepFInishedThisFrame;
        #endregion

        private Dictionary<TutorialStepID, TutorialStepManager> PlayingTutorialStepManagers;

        #region External Events
        public void PlayTutorialStep(TutorialStepID tutorialStepID)
        {
            if (!this.TutorialStatePersister.GetTutorialState(tutorialStepID))
            {
                this.PlayingTutorialStepManagers[tutorialStepID] = new TutorialStepManager(this.TutorialActionInput, tutorialStepID, this);
                this.PlayingTutorialStepManagers[tutorialStepID].Play(this.TutorialStepConfiguration.ConfigurationInherentData[tutorialStepID].TutorialGraph);
            }
        }
        #endregion

        public void Init()
        {
            this.PlayingTutorialStepManagers = new Dictionary<TutorialStepID, TutorialStepManager>();
            this.TutorialStepConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.TutorialStepConfiguration();
            this.TutorialActionInput = new TutorialActionInput(CoreGameSingletonInstances.GameCanvas,
               CoreGameSingletonInstances.CoreConfigurationManager.DiscussionTextConfiguration(),
               CoreGameSingletonInstances.DiscussionPositionManager,
                CoreGameSingletonInstances.PlayerManagerType);
            this.TutorialStatePersister = new TutorialStatePersister();
            this.TutorialStepFInishedThisFrame = new List<TutorialStepID>();
        }

        public void Tick(float d)
        {
            foreach (var playingTutorialStepManagerEntry in this.PlayingTutorialStepManagers)
            {
                playingTutorialStepManagerEntry.Value.Tick(d);
            }

            foreach (var tutorialStepFinished in this.TutorialStepFInishedThisFrame)
            {
                this.PlayingTutorialStepManagers.Remove(tutorialStepFinished);
            }
        }

        public void OnTutorialStepManagerEnd(TutorialStepID tutorialStepID)
        {
            this.TutorialStatePersister.SetTutorialState(tutorialStepID, true);
            this.TutorialStepFInishedThisFrame.Add(tutorialStepID);
        }
    }

    public interface TutorialStepManagerEventListener
    {
        void OnTutorialStepManagerEnd(TutorialStepID tutorialStepID);
    }

    class TutorialStepManager
    {

        private TutorialStepManagerEventListener TutorialStepManagerEventListener;

        private TutorialActionInput TutorialActionInput;
        private SequencedActionManager tutorialPlayer;
        private TutorialStepID tutorialStepID;

        public TutorialStepManager(TutorialActionInput TutorialActionInput, TutorialStepID tutorialStepID, TutorialStepManagerEventListener TutorialStepManagerEventListener)
        {
            this.TutorialActionInput = TutorialActionInput;
            this.tutorialPlayer = new SequencedActionManager((action) => this.tutorialPlayer.OnAddAction(action, this.TutorialActionInput), null, OnNoMoreActionToPlay: () =>
            {
                TutorialStepManagerEventListener.OnTutorialStepManagerEnd(tutorialStepID);
            });
        }

        public void Play(TutorialGraph TutorialGraph)
        {
            this.tutorialPlayer.OnAddAction(TutorialGraph.GetRootAction(), this.TutorialActionInput);
        }

        public void Tick(float d)
        {
            this.tutorialPlayer.Tick(d);
        }
    }
}
