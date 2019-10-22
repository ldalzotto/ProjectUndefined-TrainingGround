using GameConfigurationID;
using UnityEngine;

namespace CoreGame
{
    public class TutorialActionInput : SequencedActionInput
    {
        public TutorialStepID TutorialStepID;
        public Canvas MainCanvas;
        public DiscussionTextConfiguration DiscussionTextConfiguration;
        public DiscussionPositionManager DiscussionPositionManager;

        public TutorialActionInput(Canvas mainCanvas, DiscussionTextConfiguration discussionTextConfiguration, DiscussionPositionManager discussionPositionManager)
        {
            MainCanvas = mainCanvas;
            DiscussionTextConfiguration = discussionTextConfiguration;
            DiscussionPositionManager = discussionPositionManager;
        }
    }
}