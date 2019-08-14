using GameConfigurationID;
using System.Collections.Generic;
using UnityEngine;

namespace CoreGame
{
    public class TutorialManager : MonoBehaviour
    {
        private TutorialActionInput TutorialActionInput;

        #region External Dependencies
        private TutorialStepConfiguration TutorialStepConfiguration;
        #endregion

        private Dictionary<TutorialStepID, TutorialStepManager> PlayingTutorialStepManagers;

        #region External Events
        public void PlayTutorialStep(TutorialStepID tutorialStepID)
        {
            this.PlayingTutorialStepManagers[tutorialStepID] = new TutorialStepManager(this.TutorialActionInput);
            this.PlayingTutorialStepManagers[tutorialStepID].Play(this.TutorialStepConfiguration.ConfigurationInherentData[tutorialStepID].TutorialGraph);
        }
        #endregion

        public void Init()
        {
            this.PlayingTutorialStepManagers = new Dictionary<TutorialStepID, TutorialStepManager>();
            this.TutorialStepConfiguration = GameObject.FindObjectOfType<CoreConfigurationManager>().TutorialStepConfiguration();
            this.TutorialActionInput = new TutorialActionInput(GameObject.FindObjectOfType<Canvas>(),
               GameObject.FindObjectOfType<CoreConfigurationManager>().DiscussionTextConfiguration(),
               GameObject.FindObjectOfType<DiscussionPositionManager>(),
               GameObject.FindObjectOfType<PlayerManagerType>());
        }

        public void Tick(float d)
        {
            foreach (var playingTutorialStepManagerEntry in this.PlayingTutorialStepManagers)
            {
                playingTutorialStepManagerEntry.Value.Tick(d);
            }
        }
        
    }

    class TutorialStepManager
    {

        private TutorialActionInput TutorialActionInput;
        private SequencedActionManager tutorialPlayer;

        public TutorialStepManager(TutorialActionInput TutorialActionInput)
        {
            this.TutorialActionInput = TutorialActionInput;
            this.tutorialPlayer = new SequencedActionManager((action) => this.tutorialPlayer.OnAddAction(action, this.TutorialActionInput), null, OnNoMoreActionToPlay: () =>
            { });
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
