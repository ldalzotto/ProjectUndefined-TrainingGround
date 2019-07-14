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
    public class DiscussionTextOnlyNodeEdge : NodeEdgeProfile
    {
        public override List<Type> AllowedConnectedNodeEdges => new List<Type>() {
            typeof(DiscussionConnectionNodeEdge)
        };

        [CustomEnum(isCreateable: true)]
        public DiscussionNodeId DiscussionNodeId;
        [CustomEnum(isCreateable: true, choosedOpenRepertoire: true)]
        public DisucssionSentenceTextId DisplayedText;
        public PointOfInterestId Talker;

        protected override void GUI_Impl(Rect rect, ref NodeEditorProfile nodeEditorProfileRef)
        {
            EditorGUILayout.BeginVertical();
            this.DiscussionNodeId = (DiscussionNodeId)NodeEditorGUILayout.EnumField("Node : ", string.Empty, this.DiscussionNodeId);
            this.DisplayedText = (DisucssionSentenceTextId)NodeEditorGUILayout.EnumField("Text : ", string.Empty, this.DisplayedText);
            this.Talker = (PointOfInterestId)NodeEditorGUILayout.EnumField("POI : ", string.Empty, this.Talker);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Separator();
            EditorGUILayout.HelpBox(((DiscussionTreeNodeEditorProfile)nodeEditorProfileRef).DiscussionTextRepertoire.SentencesText[this.DisplayedText], MessageType.None);
        }

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