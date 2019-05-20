using UnityEngine;
using System.Collections;
using OdinSerializer;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Experimental.Editor_NodeEditor
{
    [System.Serializable]
    public abstract class NodeEdgeProfile : SerializedScriptableObject
    {
        public int Id;
        public object Value;
        public Rect Bounds;
        public NodeProfile NodeProfileRef;
        public abstract List<Type> AllowedConnectedNodeEdges { get; }
        protected virtual Color EdgeColor()
        {
            return Color.gray;
        }

        public List<NodeEdgeProfile> ConnectedNodeEdges = new List<NodeEdgeProfile>();
        public List<NodeEdgeProfile> BackwardConnectedNodeEdges = new List<NodeEdgeProfile>();

        private bool IsSelected = false;
        private Color selectedColor;
        public static T CreateNodeEdge<T>(NodeProfile NodeProfileRef) where T : NodeEdgeProfile
        {
            T nodeEdgeInstance = (T)ScriptableObject.CreateInstance(typeof(T));
            nodeEdgeInstance.Id = NodeProfileRef.GetNextEdgeId();
            nodeEdgeInstance.NodeProfileRef = NodeProfileRef;
            nodeEdgeInstance.Bounds.size = new Vector2(nodeEdgeInstance.Bounds.size.x, nodeEdgeInstance.DefaultGetEdgeHeight());
            AssetDatabase.CreateAsset(nodeEdgeInstance, NodeProfileRef.EdgesDirectoryPath + "/" + nodeEdgeInstance.GetType().Name + "_" + nodeEdgeInstance.Id.ToString() + ".asset");
            return nodeEdgeInstance;
        }

        protected virtual float DefaultGetEdgeHeight()
        {
            return 20f;
        }

        public void GUITick(Rect rect)
        {
            rect.size = new Vector2(rect.size.x, this.Bounds.size.y);
            this.Bounds = rect;
            var oldBackground = GUI.backgroundColor;
            if (this.IsSelected)
            {
                GUI.backgroundColor = this.selectedColor;
            }
            else
            {
                GUI.backgroundColor = this.EdgeColor();
            }

            GUI.Box(this.Bounds, "");
            this.GUI_Impl(this.Bounds);


            foreach (var connectedNodeEdge in this.ConnectedNodeEdges)
            {
                var startPoint = this.Bounds.center + new Vector2(this.Bounds.width / 2, 0);
                var endPoint = connectedNodeEdge.Bounds.center - new Vector2(connectedNodeEdge.Bounds.width / 2, 0);
                Handles.DrawBezier(startPoint, endPoint,
                     startPoint - Vector2.left * 50f,
               endPoint + Vector2.left * 50f, GUI.backgroundColor, null, 5f);
            }

            GUI.backgroundColor = oldBackground;
        }

        public void AddConnectedNode(NodeEdgeProfile NodeEdge)
        {
            if (this.AllowedConnectedNodeEdges.Contains(NodeEdge.GetType()))
            {
                if (!this.ConnectedNodeEdges.Contains(NodeEdge))
                {
                    this.ConnectedNodeEdges.Add(NodeEdge);
                    NodeEdge.BackwardConnectedNodeEdges.Add(this);
                }
            }
        }

        public void SetIsSelected(bool value, Color selectionColor)
        {
            this.IsSelected = value;
            this.selectedColor = selectionColor;
        }

        protected abstract void GUI_Impl(Rect rect);

        internal void ClearConnections()
        {
            foreach (var connectedEdges in this.ConnectedNodeEdges)
            {
                var backwardNodeEdge = connectedEdges.BackwardConnectedNodeEdges.Find(edge => edge.Id == this.Id);
                if (backwardNodeEdge != null)
                {
                    connectedEdges.BackwardConnectedNodeEdges.Remove(backwardNodeEdge);
                }
            }
            this.ConnectedNodeEdges.Clear();
        }

        public void ClearBackwardConnections()
        {
            foreach (var backardEdge in this.BackwardConnectedNodeEdges)
            {
                var edgeToDeleteReference = backardEdge.ConnectedNodeEdges.Find(edge => edge.Id == this.Id);
                if (edgeToDeleteReference != null)
                {
                    backardEdge.ConnectedNodeEdges.Remove(this);
                }
            }
            this.BackwardConnectedNodeEdges.Clear();
        }
    }
}

