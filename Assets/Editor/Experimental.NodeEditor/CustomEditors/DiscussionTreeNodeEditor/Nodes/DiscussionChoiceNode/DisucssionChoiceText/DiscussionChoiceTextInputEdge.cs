using AdventureGame;
using GameConfigurationID;
using NodeGraph;
using NodeGraph_Editor;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionChoiceTextInputEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>();

        [CustomEnum(isCreateable: true, choosedOpenRepertoire: true)]
        public DiscussionTextID DisplayedText;
        [CustomEnum(isCreateable: true)]
        public DiscussionNodeId DiscussionNodeId;

        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            this.DiscussionNodeId = (DiscussionNodeId)NodeEditorGUILayout.EnumField("Node : ", string.Empty, this.DiscussionNodeId);
            this.DisplayedText = (DiscussionTextID)NodeEditorGUILayout.EnumField("Text : ", string.Empty, this.DisplayedText);
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(((DiscussionTreeNodeEditorProfile)nodeEditorProfileRef).DiscussionTextRepertoire.ConfigurationInherentData[this.DisplayedText].Text, MessageType.None);
        }

        protected override Color EdgeColor()
        {
            return MyColors.Coral;
        }
    }
}