using UnityEngine;
using System.Collections;
using OdinSerializer;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System;

namespace NodeGraph
{
    [System.Serializable]
    public abstract class NodeEdgeProfile : SerializedScriptableObject
    {
        public int Id;
        public object Value;

        public NodeProfile NodeProfileRef;
        public abstract List<Type> AllowedConnectedNodeEdges { get; }

        public List<NodeEdgeProfile> ConnectedNodeEdges;
        public List<NodeEdgeProfile> BackwardConnectedNodeEdges;

#if UNITY_EDITOR
        public Rect Bounds;
        public NodeEdgeType NodeEdgeType;
        protected virtual Color EdgeColor()
        {
            return Color.gray;
        }

        private bool IsSelected = false;
        private Color selectedColor;
        public static T CreateNodeEdge<T>(NodeProfile NodeProfileRef, NodeEdgeType NodeEdgeType) where T : NodeEdgeProfile
        {
            T nodeEdgeInstance = (T)ScriptableObject.CreateInstance(typeof(T));
            nodeEdgeInstance.Id = NodeProfileRef.GetNextEdgeId();
            nodeEdgeInstance.NodeProfileRef = NodeProfileRef;
            nodeEdgeInstance.NodeEdgeType = NodeEdgeType;
            nodeEdgeInstance.Bounds.size = new Vector2(nodeEdgeInstance.Bounds.size.x, nodeEdgeInstance.DefaultGetEdgeHeight());
            nodeEdgeInstance.ConnectedNodeEdges = new List<NodeEdgeProfile>();
            nodeEdgeInstance.BackwardConnectedNodeEdges = new List<NodeEdgeProfile>();
            AssetDatabase.CreateAsset(nodeEdgeInstance, NodeProfileRef.EdgesDirectoryPath + "/" + nodeEdgeInstance.GetType().Name + "_" + nodeEdgeInstance.Id.ToString() + ".asset");
            return nodeEdgeInstance;
        }

        protected virtual float DefaultGetEdgeHeight()
        {
            return 20f;
        }

        public void GUIEdgeRectangles(Rect parentNodeRect)
        {
            var oldBackground = GUI.backgroundColor;
            if (this.IsSelected)
            {
                GUI.backgroundColor = this.selectedColor;
            }
            else
            {
                GUI.backgroundColor = this.EdgeColor();
            }

            EditorGUI.BeginChangeCheck();

            var verticalBound = EditorGUILayout.BeginVertical(GUI.skin.box);
            this.GUI_Impl(this.Bounds);
            EditorGUILayout.EndVertical();

            this.Bounds.position = verticalBound.position + parentNodeRect.position;
            this.Bounds.size = verticalBound.size;

            GUI.backgroundColor = oldBackground;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }
        }

        public void GUIConnectionLines()
        {
            var oldBackground = GUI.backgroundColor;
            if (this.IsSelected)
            {
                GUI.backgroundColor = this.selectedColor;
            }
            else
            {
                GUI.backgroundColor = this.EdgeColor();
            }
            foreach (var connectedNodeEdge in this.ConnectedNodeEdges)
            {
                var startPoint = this.Bounds.center + new Vector2(this.Bounds.width / 2, 0);
                var endPoint = connectedNodeEdge.Bounds.position - new Vector2(0, -connectedNodeEdge.Bounds.height / 2);
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

                    if (NodeEdge.BackwardConnectedNodeEdges == null)
                    {
                        NodeEdge.BackwardConnectedNodeEdges = new List<NodeEdgeProfile>();
                    }

                    if (NodeEdge.NodeEdgeType == NodeEdgeType.SINGLE_INPUT)
                    {
                        if (NodeEdge.BackwardConnectedNodeEdges == null)
                        {
                            NodeEdge.BackwardConnectedNodeEdges = new List<NodeEdgeProfile>();
                        }
                        if (NodeEdge.BackwardConnectedNodeEdges.Count > 0)
                        {
                            NodeEdge.BackwardConnectedNodeEdges[0].ClearConnection(NodeEdge);
                            NodeEdge.BackwardConnectedNodeEdges[0] = this;
                        }
                        else
                        {
                            NodeEdge.BackwardConnectedNodeEdges.Add(this);
                        }
                    }
                    else
                    {
                        NodeEdge.BackwardConnectedNodeEdges.Add(this);
                    }

                    EditorUtility.SetDirty(NodeEdge);
                    EditorUtility.SetDirty(this);
                }
            }
        }

        public void SetIsSelected(bool value, Color selectionColor)
        {
            this.IsSelected = value;
            this.selectedColor = selectionColor;
        }

        protected virtual void GUI_Impl(Rect rect) { GUILayout.Label(""); }

        public void ClearConnections()
        {
            foreach (var connectedEdges in this.ConnectedNodeEdges)
            {
                if (connectedEdges.BackwardConnectedNodeEdges != null)
                {
                    foreach (var connectedEdgeBackgoundNode in new List<NodeEdgeProfile>(connectedEdges.BackwardConnectedNodeEdges))
                    {
                        if (connectedEdgeBackgoundNode.Id == this.Id)
                        {
                            connectedEdges.BackwardConnectedNodeEdges.Remove(connectedEdgeBackgoundNode);
                        }
                    }
                    connectedEdges.BackwardConnectedNodeEdges = null;
                    EditorUtility.SetDirty(connectedEdges);
                }
            }
            this.ConnectedNodeEdges.Clear();
            EditorUtility.SetDirty(this);
        }

        public void ClearConnection(NodeEdgeProfile removedEdgeConnection)
        {
            bool remove = false;
            foreach (var connectedEdges in this.ConnectedNodeEdges)
            {
                if (connectedEdges != null && connectedEdges.Id == removedEdgeConnection.Id)
                {
                    remove = true;
                }
            }
            if (remove)
            {
                this.ConnectedNodeEdges.Remove(removedEdgeConnection);
                EditorUtility.SetDirty(this);
            }
        }

        public void ClearBackwardConnections()
        {

            if (this.BackwardConnectedNodeEdges != null)
            {
                foreach (var BackwardConnectedNodeEdge in this.BackwardConnectedNodeEdges)
                {
                    var edgeToDeleteReference = BackwardConnectedNodeEdge.ConnectedNodeEdges.Find(edge => edge.Id == this.Id);
                    if (edgeToDeleteReference != null)
                    {
                        BackwardConnectedNodeEdge.ConnectedNodeEdges.Remove(this);
                        EditorUtility.SetDirty(BackwardConnectedNodeEdge);
                    }
                }

                this.BackwardConnectedNodeEdges.Clear();
                EditorUtility.SetDirty(this);
            }

        }

#endif
    }

#if UNITY_EDITOR
    public enum NodeEdgeType
    {
        SINGLE_INPUT,
        MULTIPLE_INPUT
    }
#endif
}

