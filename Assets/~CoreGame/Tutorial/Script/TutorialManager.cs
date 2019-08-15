using GameConfigurationID;
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

        #region Data Retrieval 
        public bool GetTutorialCurrentState(TutorialStepID TutorialStepID)
        {
            return this.TutorialStatePersister.GetTutorialState(TutorialStepID);
        }
        public bool IsTutorialStepPlaying(TutorialStepID tutorialStepID)
        {
            return this.PlayingTutorialStepManagers.ContainsKey(tutorialStepID);
        }
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

        public void AbortAllTutorials()
        {
            foreach (var PlayingTutorialStepManager in PlayingTutorialStepManagers.Values)
            {
                PlayingTutorialStepManager.Interrupt();
            }
            this.PlayingTutorialStepManagers.Clear();
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

        public void Interrupt()
        {
            this.tutorialPlayer.InterruptAllActions();
        }
    }
}
