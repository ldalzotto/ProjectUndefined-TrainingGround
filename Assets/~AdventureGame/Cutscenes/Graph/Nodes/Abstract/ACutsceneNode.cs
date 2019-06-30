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
    public class ACutsceneNode<T, E> : NodeProfile, ICutsceneNode where T : SequencedAction where E : ACutsceneEdge<T>
    {
        [SerializeField]
        private CutsceneActionConnectionEdge inputCutsceneConnectionEdge;
        [SerializeField]
        private CutsceneActionConnectionEdge outputCutsceneConnectionEdge;

        [SerializeField]
        private E actionEdge;

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
        }

        private List<ICutsceneNode> GetNextNodes()
        {
            return outputCutsceneConnectionEdge.ConnectedNodeEdges.ConvertAll(e => ((ICutsceneNode)((CutsceneActionConnectionEdge)e).NodeProfileRef));
        }

#if UNITY_EDITOR
        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.inputCutsceneConnectionEdge = CutsceneActionConnectionEdge.CreateNodeEdge<CutsceneActionConnectionEdge>(this, NodeEdgeType.SINGLE_INPUT);
            this.actionEdge = CutsceneActionConnectionEdge.CreateNodeEdge<E>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>()
            {
                this.inputCutsceneConnectionEdge, this.actionEdge
            };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.outputCutsceneConnectionEdge = CutsceneActionConnectionEdge.CreateNodeEdge<CutsceneActionConnectionEdge>(this, NodeEdgeType.MULTIPLE_INPUT);
            return new List<NodeEdgeProfile>()
            {
                 this.outputCutsceneConnectionEdge
            };
        }

        protected override void NodeGUI(ref NodeEditorProfile nodeEditorProfileRef)
        {
            EditorGUILayout.BeginHorizontal();
            this.inputCutsceneConnectionEdge.GUIEdgeRectangles(this.OffsettedBounds);
            this.outputCutsceneConnectionEdge.GUIEdgeRectangles(this.OffsettedBounds);
            EditorGUILayout.EndHorizontal();
            this.actionEdge.GUIEdgeRectangles(this.OffsettedBounds);
        }

        protected override Vector2 Size()
        {
            return new Vector2(300, 100);
        }
#endif
    }

}
