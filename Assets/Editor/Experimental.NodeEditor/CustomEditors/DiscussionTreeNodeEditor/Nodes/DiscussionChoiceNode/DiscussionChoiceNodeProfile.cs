using NodeGraph;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_DiscussionTreeNodeEditor
{
    [System.Serializable]
    public class DiscussionChoiceNodeProfile : NodeProfile
    {
        public DiscussionChoiceInputEdge DiscussionChoiceInputEdge;
        public List<DiscussionChoiceToTextEdge> ChoicesEdge;

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.DiscussionChoiceInputEdge = DiscussionChoiceInputEdge.CreateNodeEdge<DiscussionChoiceInputEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.DiscussionChoiceInputEdge };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.ChoicesEdge = new List<DiscussionChoiceToTextEdge>();
            return this.ChoicesEdge.ConvertAll(e => (NodeEdgeProfile)e);
        }

        protected override void NodeGUI(ref NodeEditorProfile nodeEditorProfileRef)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
            {
                var createdEdge = DiscussionChoiceToTextEdge.CreateNodeEdge<DiscussionChoiceToTextEdge>(this, NodeEdgeType.SINGLE_INPUT);
                this.ChoicesEdge.Add(createdEdge);
                this.AddOutputEdge(createdEdge);
            }
            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
            {
                List<DiscussionChoiceToTextEdge> choicesEdgeToDelete = new List<DiscussionChoiceToTextEdge>();
                foreach (var selectedObject in nodeEditorProfileRef.NodeEdtitorSelectionProfile.CurrentSelectedObjects)
                {
                    var selectedChoiceEdge = selectedObject as DiscussionChoiceToTextEdge;
                    if (selectedChoiceEdge != null && this.ChoicesEdge.Contains(selectedChoiceEdge))
                    {
                        choicesEdgeToDelete.Add(selectedChoiceEdge);
                    }
                }

                foreach (var chocieEdgeToDelete in choicesEdgeToDelete)
                {
                    this.ChoicesEdge.Remove(chocieEdgeToDelete);
                    this.DeleteEdge(chocieEdgeToDelete);
                }
            }
            EditorGUILayout.EndHorizontal();

            this.DiscussionChoiceInputEdge.GUIEdgeRectangles(this.OffsettedBounds);
            foreach(var choiceEdge in this.ChoicesEdge)
            {
                choiceEdge.GUIEdgeRectangles(this.OffsettedBounds);
            }
        }

        protected override Color NodeColor()
        {
            return MyColors.Coral;
        }

        protected override Vector2 Size()
        {
            return new Vector2(200, 100);
        }
        
        protected override string NodeTitle()
        {
            return this.DiscussionChoiceInputEdge.DiscussionNodeId.ToString();
        }
    }
}