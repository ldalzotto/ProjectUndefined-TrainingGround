using GameConfigurationID;
using NodeGraph_Editor;
using RTPuzzle;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class PuzzleDiscussionTextOnlyNodeEdge : AbstractTextOnlyNodeEdge
    {
        public ParametrizedInteractiveObject ParametrizedInteractiveObject;

        protected override void AdditionalGUI()
        {
            this.ParametrizedInteractiveObject.ActionGUI("InteractiveObject : ");
        }
    }
}