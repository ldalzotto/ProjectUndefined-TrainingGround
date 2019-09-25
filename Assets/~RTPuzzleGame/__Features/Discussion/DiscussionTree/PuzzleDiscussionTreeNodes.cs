using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    public class PuzzleDiscussionTextOnlyNode : AbstractDiscussionTextOnlyNode
    {
        [SerializeField]
        private ParametrizedInteractiveObject parametrizedTalker;

        public PuzzleDiscussionTextOnlyNode(DiscussionNodeId DiscussionNodeId, DiscussionTextID displayedText, DiscussionNodeId nextNode,
            ParametrizedInteractiveObject parametrizedTalker) : base(DiscussionNodeId, displayedText, nextNode)
        {
            this.parametrizedTalker = parametrizedTalker;
        }

        public ParametrizedInteractiveObject ParametrizedTalker { get => parametrizedTalker; set => parametrizedTalker = value; }

    }
}
