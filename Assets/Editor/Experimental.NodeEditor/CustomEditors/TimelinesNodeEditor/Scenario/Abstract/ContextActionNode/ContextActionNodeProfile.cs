using AdventureGame;
using NodeGraph;
using System.Collections.Generic;
using UnityEngine;

namespace Editor_ScenarioNodeEditor
{
    public interface IContextActionNodeProfile
    {
        IContextActionDrawable GetContextAction();
    }

    [System.Serializable]
    public abstract class ContextActionNodeProfile<T, E> : NodeProfile, IContextActionNodeProfile where E : ContextActionEdge<T> where T : IContextActionDrawable
    {
        [SerializeField]
        private ContextActionInputEdge contextActionInputEdge;
        [SerializeField]
        private E contextActionEdge;
        [SerializeField]
        private ContextActionOutputEdge contextActionOutputEdge;

        public IContextActionDrawable GetContextAction()
        {
            var nodeContextAction = this.contextActionEdge.ContextAction;
            var nextContextAction = this.contextActionInputEdge.GetAContextAction();
            ((AContextAction)((object)nodeContextAction)).SetNextContextAction(nextContextAction);
            return nodeContextAction;
        }

        public override List<NodeEdgeProfile> InitInputEdges()
        {
            this.contextActionInputEdge = NodeEdgeProfile.CreateNodeEdge<ContextActionInputEdge>(this, NodeEdgeType.SINGLE_INPUT);
            this.contextActionEdge = NodeEdgeProfile.CreateNodeEdge<E>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.contextActionInputEdge, this.contextActionEdge };
        }

        public override List<NodeEdgeProfile> InitOutputEdges()
        {
            this.contextActionOutputEdge = NodeEdgeProfile.CreateNodeEdge<ContextActionOutputEdge>(this, NodeEdgeType.SINGLE_INPUT);
            return new List<NodeEdgeProfile>() { this.contextActionOutputEdge };
        }

        protected override Vector2 Size()
        {
            return new Vector3(300, 200);
        }

        protected override string NodeTitle()
        {
            return base.NodeTitle().Replace("Profile", "").Replace("Node", "");
        }
    }

}