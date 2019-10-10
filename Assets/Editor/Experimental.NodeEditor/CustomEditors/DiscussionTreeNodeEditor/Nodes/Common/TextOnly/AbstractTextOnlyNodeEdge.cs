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
    public abstract class AbstractTextOnlyNodeEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(DiscussionConnectionNodeEdge)
        };

        [CustomEnum()]
        public DiscussionNodeId DiscussionNodeId;
        [CustomEnum(choosedOpenRepertoire: true)]
        public DiscussionTextID DisplayedText;

        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            EditorGUILayout.BeginVertical();
            this.DiscussionNodeId = (DiscussionNodeId)NodeEditorGUILayout.EnumField("Node : ", string.Empty, this.DiscussionNodeId);
            this.DisplayedText = (DiscussionTextID)NodeEditorGUILayout.EnumField("Text : ", string.Empty, this.DisplayedText);
            this.AdditionalGUI();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();

            if (((DiscussionTreeNodeEditorProfile)nodeEditorProfileRef).DiscussionTextRepertoire.ConfigurationInherentData.ContainsKey(this.DisplayedText))
            {
                EditorGUILayout.HelpBox(((DiscussionTreeNodeEditorProfile)nodeEditorProfileRef).DiscussionTextRepertoire.ConfigurationInherentData[this.DisplayedText].Text, MessageType.None);
            }

        }

        protected abstract void AdditionalGUI();

        protected override Color EdgeColor()
        {
            return MyColors.PaleBlue;
        }

        public NodeEdgeProfile GetLinkedNodeEdge()
        {
            var outputEdge = (DiscussionConnectionNodeEdge)this.NodeProfileRef.OutputEdges[0];
            if (outputEdge.ConnectedNodeEdges != null && outputEdge.ConnectedNodeEdges.Count > 0)
            {
                return outputEdge.ConnectedNodeEdges[0];
            }
            return null;
        }
    }
}