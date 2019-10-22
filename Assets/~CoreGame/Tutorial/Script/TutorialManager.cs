using System.Collections.Generic;
using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    public class TutorialManager : MonoBehaviour, TutorialStepManagerEventListener
    {
        private TutorialStepManager PlayingTutorialStepManager;
        private TutorialActionInput TutorialActionInput;

        #region Internal Managers

        private TutorialStatePersister TutorialStatePersister;

        #endregion

        #region External Dependencies

        private TutorialStepConfiguration TutorialStepConfiguration;

        #endregion

        #region State

        private List<TutorialStepID> TutorialStepFInishedThisFrame;

        #endregion

        public void Init()
        {
            #region Event Registering

            #endregion

            TutorialStepConfiguration = CoreGameSingletonInstances.CoreConfigurationManager.TutorialStepConfiguration();
            TutorialActionInput = new TutorialActionInput(CoreGameSingletonInstances.GameCanvas,
                CoreGameSingletonInstances.CoreConfigurationManager.DiscussionTextConfiguration(),
                CoreGameSingletonInstances.DiscussionPositionManager);
            TutorialStatePersister = new TutorialStatePersister();
            TutorialStepFInishedThisFrame = new List<TutorialStepID>();
        }

        public void Tick(float d)
        {
            if (PlayingTutorialStepManager != null) PlayingTutorialStepManager.Tick(d);
        }

        #region Data Retrieval 

        public bool GetTutorialCurrentState(TutorialStepID TutorialStepID)
        {
            return TutorialStatePersister.GetTutorialState(TutorialStepID);
        }

        public bool IsTutorialStepPlaying()
        {
            return PlayingTutorialStepManager != null;
        }

        #endregion

        #region External Events

        public void PlayTutorialStep(TutorialStepID tutorialStepID)
        {
            if (PlayingTutorialStepManager != null) PlayingTutorialStepManager.Interrupt();

            if (!TutorialStatePersister.GetTutorialState(tutorialStepID))
            {
                TutorialActionInput.TutorialStepID = tutorialStepID;
                PlayingTutorialStepManager = new TutorialStepManager(TutorialActionInput, tutorialStepID, this);
                PlayingTutorialStepManager.Play(TutorialStepConfiguration.ConfigurationInherentData[tutorialStepID].TutorialGraph);
            }
        }

        public void Abort()
        {
            if (PlayingTutorialStepManager != null) PlayingTutorialStepManager.Interrupt();
        }

        public void OnTutorialStepManagerEnd(TutorialStepID tutorialStepID)
        {
            TutorialStatePersister.SetTutorialState(tutorialStepID, true);
            PlayingTutorialStepManager = null;
        }

        public void SendEventToTutorialGraph(TutorialGraphEventType tutorialGraphEvent)
        {
            if (PlayingTutorialStepManager != null) PlayingTutorialStepManager.ReceiveTutorialGraphEvent(tutorialGraphEvent);
        }

        #endregion
    }

    public interface TutorialStepManagerEventListener
    {
        void OnTutorialStepManagerEnd(TutorialStepID tutorialStepID);
    }

    internal class TutorialStepManager
    {
        private TutorialActionInput TutorialActionInput;
        private SequencedActionManager tutorialPlayer;
        private TutorialStepID tutorialStepID;
        private TutorialStepManagerEventListener TutorialStepManagerEventListener;

        public TutorialStepManager(TutorialActionInput TutorialActionInput, TutorialStepID tutorialStepID, TutorialStepManagerEventListener TutorialStepManagerEventListener)
        {
            this.TutorialActionInput = TutorialActionInput;
            tutorialPlayer = new SequencedActionManager((action) => tutorialPlayer.OnAddAction(action, this.TutorialActionInput), null, () => { TutorialStepManagerEventListener.OnTutorialStepManagerEnd(tutorialStepID); });
        }

        public void Play(TutorialGraph TutorialGraph)
        {
            tutorialPlayer.OnAddActions(TutorialGraph.GetRootActions(), TutorialActionInput);
        }

        public void Tick(float d)
        {
            tutorialPlayer.Tick(d);
        }

        public void Interrupt()
        {
            tutorialPlayer.InterruptAllActions();
        }

        public void ReceiveTutorialGraphEvent(TutorialGraphEventType tutorialGraphEvent)
        {
            foreach (var currentAction in tutorialPlayer.GetCurrentActions())
                if (typeof(ITutorialEventListener).IsAssignableFrom(currentAction.GetType()))
                    if (tutorialGraphEvent == TutorialGraphEventType.PUZZLE_ACTION_WHEEL_AWAKE)
                        ((ITutorialEventListener) currentAction).OnPlayerActionWheelAwake();
        }
    }
}