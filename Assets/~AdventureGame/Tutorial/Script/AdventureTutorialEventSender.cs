using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public class AdventureTutorialEventSender : MonoBehaviour
    {
        private TutorialManager tutorialManager;

        public void Init()
        {
            this.tutorialManager = GameObject.FindObjectOfType<TutorialManager>();
        }

        public void Tick(float d)
        {
            if (!this.tutorialManager.GetTutorialCurrentState(TutorialStepID.TUTORIAL_MOVEMENT) && !this.tutorialManager.IsTutorialStepPlaying(TutorialStepID.TUTORIAL_MOVEMENT))
            {
                this.tutorialManager.PlayTutorialStep(TutorialStepID.TUTORIAL_MOVEMENT);
            }
        }
    }
}
