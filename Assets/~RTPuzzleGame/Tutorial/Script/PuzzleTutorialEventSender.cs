using CoreGame;
using GameConfigurationID;

namespace RTPuzzle
{
    public class PuzzleTutorialEventSenderManager : GameSingleton<PuzzleTutorialEventSenderManager>
    {
        private TutorialManager tutorialManager;

        public PuzzleTutorialEventSenderManager()
        {
            tutorialManager = CoreGameSingletonInstances.TutorialManager;
        }

        public void Tick(float d)
        {
            if (!tutorialManager.GetTutorialCurrentState(TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE) && !tutorialManager.IsTutorialStepPlaying()) tutorialManager.PlayTutorialStep(TutorialStepID.PUZZLE_CONTEXT_ACTION_AWAKE);
        }
    }
}