using CoreGame;
using Experimental.Editor_NodeEditor;
using GameConfigurationID;
using RTPuzzle;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    public class DiscussionTreeNodeEditor : NodeEditor
    {
        protected override Type NodeEditorProfileType => typeof(DiscussionTreeNodeEditorProfile);

        protected override Dictionary<string, Type> NodePickerConfiguration => new Dictionary<string, Type>()
        {
            {"DiscussionStartNode", typeof(DiscussionStartNodeProfile) },
            {"FixedPositionTextOnlyNode", typeof(FixedScreenPositionDiscussionTextOnlyNodeProfile) },
            {"DiscussionChoiceTextNode", typeof(DiscussionChoiceTextNodeProfile) },
            {"Adventure.//TextOnlyNode", typeof(AdventureDiscussionTextOnlyNodeProfile)},
            {"Adventure.//DiscussionChoiceNode", typeof(AdventureDiscussionChoiceNodeProfile) },
        };

        protected override void OnEnable_Impl()
        {
        }

        protected override void OnGUI_Impl()
        {
            if (GUILayout.Button("GENERATE", GUILayout.Width(75f)))
            {
                var discussionTree = (DiscussionTree)DiscussionTree.CreateInstance(typeof(DiscussionTree).Name);
                var discussionTreeId = ((DiscussionTreeNodeEditorProfile)this.nodeEditorProfile).DiscussionTreeID;

                var allNodes = this.nodeEditorProfile.Nodes.Values.ToList();
                var startNode = (DiscussionStartNodeProfile)allNodes.Find(nodeProfile => (nodeProfile as DiscussionStartNodeProfile) != null);
                var firstTextNodeEdge = startNode.DiscussionStartEdge.GetLinkedEdge();

                discussionTree.DiscussionRootNode = firstTextNodeEdge.DiscussionNodeId;
                discussionTree.DiscussionNodes = new Dictionary<DiscussionNodeId, DiscussionTreeNode>();

                foreach (var editorNode in allNodes)
                {
                    if (editorNode.GetType() == typeof(AdventureDiscussionTextOnlyNodeProfile))
                    {
                        var discussionTextOnlyNodeProfile = (AdventureDiscussionTextOnlyNodeProfile)editorNode;
                        discussionTree.DiscussionNodes.Add(discussionTextOnlyNodeProfile.DiscussionNodeEdge.DiscussionNodeId,
                            new AdventureDiscussionTextOnlyNode(discussionTextOnlyNodeProfile.DiscussionNodeEdge.DiscussionNodeId, discussionTextOnlyNodeProfile.DiscussionNodeEdge.DisplayedText,
                                  discussionTextOnlyNodeProfile.ConnectionEdge.GetConnectedNodeEdgeDiscussionNodeID(), discussionTextOnlyNodeProfile.DiscussionNodeEdge.Talker)
                            );
                    }
                    else if (editorNode.GetType() == typeof(FixedScreenPositionDiscussionTextOnlyNodeProfile))
                    {
                        var fixedScreenPositionTextOnlyProfile = (FixedScreenPositionDiscussionTextOnlyNodeProfile)editorNode;
                        discussionTree.DiscussionNodes.Add(fixedScreenPositionTextOnlyProfile.DiscussionNodeEdge.DiscussionNodeId,
                            new FixedScreenPositionDiscussionTextOnlyNode(fixedScreenPositionTextOnlyProfile.DiscussionNodeEdge.DiscussionNodeId, fixedScreenPositionTextOnlyProfile.DiscussionNodeEdge.DisplayedText,
                                  fixedScreenPositionTextOnlyProfile.ConnectionEdge.GetConnectedNodeEdgeDiscussionNodeID(), fixedScreenPositionTextOnlyProfile.DiscussionNodeEdge.DiscussionPositionMarkerID)
                            );
                    }
                    else if (editorNode.GetType() == typeof(AdventureDiscussionChoiceNodeProfile))
                    {
                        var discussionChoiceNodeProfile = (AdventureDiscussionChoiceNodeProfile)editorNode;
                        discussionTree.DiscussionNodes.Add(discussionChoiceNodeProfile.DiscussionChoiceInputEdge.DiscussionNodeId,
                            new AdventureDiscussionChoiceNode(discussionChoiceNodeProfile.DiscussionChoiceInputEdge.DiscussionNodeId, discussionChoiceNodeProfile.ChoicesEdge.ConvertAll(choice => choice.GetConnectedDiscussionNodeId()),
                              discussionChoiceNodeProfile.DiscussionChoiceInputEdge.Talker));
                    }
                    else if (editorNode.GetType() == typeof(DiscussionChoiceTextNodeProfile))
                    {
                        var discussionChoiceTextNodeProfile = (DiscussionChoiceTextNodeProfile)editorNode;
                        discussionTree.DiscussionNodes.Add(discussionChoiceTextNodeProfile.DiscussionChoiceTextInputEdge.DiscussionNodeId,
                            new DiscussionChoice(discussionChoiceTextNodeProfile.DiscussionChoiceTextInputEdge.DiscussionNodeId, discussionChoiceTextNodeProfile.DiscussionChoiceTextInputEdge.DisplayedText,
                            discussionChoiceTextNodeProfile.DiscussionConnectionNodeEdge.GetConnectedNodeEdgeDiscussionNodeID())
                            );
                    }
                }
                discussionTree.DiscussionNodes.Add(DiscussionNodeId.NONE, null);

                var creationAssetPath = MyFileUtil.GetAssetDirectoryPath(this.nodeEditorProfile) + discussionTreeId.ToString() + "_DiscussionTree.asset";
                AssetDatabase.CreateAsset(discussionTree, creationAssetPath);
                var DiscussionTreeConfiguration = AssetFinder.SafeSingleAssetFind<DiscussionTreeConfiguration>("t:" + typeof(DiscussionTreeConfiguration).Name);
                DiscussionTreeConfiguration.SetEntry(discussionTreeId, (DiscussionTree)AssetDatabase.LoadAssetAtPath(creationAssetPath, typeof(DiscussionTree)));
            }
        }
    }

}
