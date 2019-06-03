using Experimental.Editor_NodeEditor;
using System;
using System.Collections.Generic;

namespace Editor_DiscussionTreeNodeEditor
{
    public class DiscussionTreeNodeEditor : NodeEditor
    {
        protected override Type NodeEditorProfileType => typeof(DiscussionTreeNodeEditorProfile);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>()
        {
            {"DiscussionStartNode", typeof(DiscussionStartNodeProfile) },
            {"TextOnlyNode", typeof(DiscussionTextOnlyNodeProfile)},
            {"DiscussionChoiceNode", typeof(DiscussionChoiceNodeProfile) },
            {"DiscussionChoiceTextNode", typeof(DiscussionChoiceTextNodeProfile) }
        };

        protected override void OnEnable_Impl()
        {
        }

        protected override void OnGUI_Impl()
        {
        }
    }

}
