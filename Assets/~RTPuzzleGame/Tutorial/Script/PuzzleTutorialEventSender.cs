using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    public class PuzzleTutorialEventSenderManager : GameSingleton<PuzzleTutorialEventSenderManager>
    {
        private TutorialManager tutorialManager;

        public PuzzleTutorialEventSenderManager()
        {
            this.tutorialManager = CoreGameSingletonInstances.TutorialManager;
        }
        
        public void Tick(float d)
        {
            if (!this.tutorialManager.GetTutorialCurrentState(TutorialStepID.PUZZLE_TIME_ELAPSING) && !this.tutorialManager.IsTutorialStepPlaying())
            {
                this.tutorialManager.PlayTutorialStep(TutorialStepID.PUZZLE_TIME_ELAPSING);
            }
            if (!this.tutorialManager.GetTutorialCurrentState(TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE) && !this.tutorialManager.IsTutorialStepPlaying())
            {
                this.tutorialManager.PlayTutorialStep(TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE);
            }
        }
    }
}
