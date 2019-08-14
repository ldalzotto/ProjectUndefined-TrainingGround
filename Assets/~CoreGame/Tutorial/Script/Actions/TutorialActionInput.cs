using UnityEngine;
using System.Collections;

namespace CoreGame
{
    public class TutorialActionInput : SequencedActionInput
    {
        public Canvas MainCanvas;
        public DiscussionTextConfiguration DiscussionTextConfiguration;
        public DiscussionPositionManager DiscussionPositionManager;
        public PlayerManagerType PlayerManagerType;

        public TutorialActionInput(Canvas mainCanvas, DiscussionTextConfiguration discussionTextConfiguration, DiscussionPositionManager discussionPositionManager, PlayerManagerType playerManagerType)
        {
            MainCanvas = mainCanvas;
            DiscussionTextConfiguration = discussionTextConfiguration;
            DiscussionPositionManager = discussionPositionManager;
            PlayerManagerType = playerManagerType;
        }
    }
}
