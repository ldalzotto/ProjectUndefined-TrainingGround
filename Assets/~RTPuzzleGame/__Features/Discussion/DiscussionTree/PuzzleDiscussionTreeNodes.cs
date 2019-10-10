using CoreGame;
using GameConfigurationID;
using InteractiveObjectTest;

namespace RTPuzzle
{
    [System.Serializable]
    public class PuzzleDiscussionTextOnlyNode : AbstractDiscussionTextOnlyNode
    {
        public CoreInteractiveObject Talker { get; private set; }

        public PuzzleDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode,
            CoreInteractiveObject Talker) : base(DiscussionNodeId, displayedText, nextNode)
        {
            this.Talker = Talker;
        }

    }
}
