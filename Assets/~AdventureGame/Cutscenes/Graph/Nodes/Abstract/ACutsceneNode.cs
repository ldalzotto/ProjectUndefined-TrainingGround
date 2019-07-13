using CoreGame;
using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AdventureGame
{
    public interface ICutsceneNode
    {
        SequencedAction GetAction();
        void BuildAction();
    }

    [System.Serializable]
    public abstract class ACutsceneNode<T, E> : NodeProfile, ICutsceneNode where T : SequencedAction where E : ACutsceneEdge<T>
    {
        [SerializeField]
        private CutsceneActionConnectionEdge inputCutsceneConnectionEdge;
        [SerializeField]
        private CutsceneActionConnectionEdge outputCutsceneConnectionEdge;
        [SerializeField]
        private CutsceneWorkflowAbortEdge workflowNodeReferenceEdge;

        [SerializeField]
        protected E actionEdge;

        public SequencedAction GetAction()
        {
            return actionEdge.AssociatedAction;
        }

        public void BuildAction()
        {
            var nexLevelnodes = GetNextNodes();
            if (nexLevelnodes != null && nexLevelnodes.Count > 0)
            {
                foreach (var nexLevelnode in nexLevelnodes)
                {
                    nexLevelnode.BuildAction();
                }
                this.GetAction().SetNextContextAction(nexLevelnodes.ConvertAll(n => n.GetAction()));
            }
            this.AfterActionInitialized();
        }

        public virtual void AfterActionInitialized() { }

        private List<ICutsceneNode> GetNextNodes()
        {
            List<ICutsceneNode> nextNodes = new List<ICutsceneNode>();
            foreach (var connectedNode in outputCutsceneConnectionEdge.ConnectedNodeEdges.ConvertAll(e => (CutsceneActionConnectionEdge)e))
            {
                var ICutsceneNode = connectedNode.NodeProfileRef as ICutsceneNode;
                if (ICutsceneNode != null)
                {
                    nextNodes.Add(ICutsceneNode);
                }
            }
            return nextNodes;
        }

#if UNITY_EDITOR
        public virtual bool DisplayWorkflowEdge()
        {
            return true;
        }

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.inputCutsceneConnectionEdge = CutsceneActionConnectionEdge.CreateNodeEdge<CutsceneActionConnectionEdge>(this, NodeEdgeType.SINGLE_INPUT);
            this.actionEdge = CutsceneActionConnectionEdge.CreateNodeEdge<E>(this, NodeEdgeType.MULTIPLE_INPUT);
            return new List<NodeEdgeProfile>()
            {
                this.inputCutsceneConnectionEdge, this.actionEdge
            };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.outputCutsceneConnectionEdge = CutsceneActionConnectionEdge.CreateNodeEdge<CutsceneActionConnectionEdge>(this, NodeEdgeType.MULTIPLE_INPUT);
            this.workflowNodeReferenceEdge = CutsceneActionConnectionEdge.CreateNodeEdge<CutsceneWorkflowAbortEdge>(this, NodeEdgeType.MULTIPLE_INPUT);
            return new List<NodeEdgeProfile>()
            {
                 this.outputCutsceneConnectionEdge,  this.workflowNodeReferenceEdge
            };
        }

        protected override void NodeGUI(ref NodeEditorProfile nodeEditorProfileRef)
        {
            EditorGUILayout.BeginHorizontal();
            this.inputCutsceneConnectionEdge.GUIEdgeRectangles(this.OffsettedBounds);
            this.outputCutsceneConnectionEdge.GUIEdgeRectangles(this.OffsettedBounds);
            EditorGUILayout.EndHorizontal();
            this.actionEdge.GUIEdgeRectangles(this.OffsettedBounds);
            if (this.DisplayWorkflowEdge())
            {
                EditorGUILayout.BeginHorizontal();
                this.workflowNodeReferenceEdge.GUIEdgeRectangles(this.OffsettedBounds);
                EditorGUILayout.EndHorizontal();
            }
        }


        protected override Vector2 Size()
        {
            return new Vector2(300, 100);
        }

        protected override Color NodeColor()
        {
            return MyColors.Coral;
        }
#endif
    }

}
