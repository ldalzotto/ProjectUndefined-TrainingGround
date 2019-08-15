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

        public bool IsTutorialStepPlaying()
        {
            return this.PlayingTutorialStepManager != null;
        }
        #endregion

        private TutorialStepManager PlayingTutorialStepManager;

        #region External Events
        public void PlayTutorialStep(TutorialStepID tutorialStepID)
        {
            if (this.PlayingTutorialStepManager != null)
            {
                this.PlayingTutorialStepManager.Interrupt();
            }

            if (!this.TutorialStatePersister.GetTutorialState(tutorialStepID))
            {
                this.TutorialActionInput.TutorialStepID = tutorialStepID;
                this.PlayingTutorialStepManager = new TutorialStepManager(this.TutorialActionInput, tutorialStepID, this);
                this.PlayingTutorialStepManager.Play(this.TutorialStepConfiguration.ConfigurationInherentData[tutorialStepID].TutorialGraph);
            }
        }

        public void Abort()
        {
            if (this.PlayingTutorialStepManager != null)
            {
                this.PlayingTutorialStepManager.Interrupt();
            }
        }
        #endregion

        public void Init()
        {
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
            if (this.PlayingTutorialStepManager != null)
            {
                this.PlayingTutorialStepManager.Tick(d);
            }
        }

        public void OnTutorialStepManagerEnd(TutorialStepID tutorialStepID)
        {
            this.TutorialStatePersister.SetTutorialState(tutorialStepID, true);
            this.PlayingTutorialStepManager = null;
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
