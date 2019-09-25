using GameConfigurationID;
using NodeGraph_Editor;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class PuzzleDiscussionTextOnlyNodeEdge : AbstractTextOnlyNodeEdge
    {
        public InteractiveObjectID Talker;

        protected override void AdditionalGUI()
        {
            this.Talker = (InteractiveObjectID)NodeEditorGUILayout.EnumField("InteractiveObject : ", string.Empty, this.Talker);
        }
    }
}