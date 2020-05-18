﻿using Editor_Zoom;
using NodeGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Experimental.Editor_NodeEditor
{
    public abstract class NodeEditor : EditorWindow
    {
        private void OnEnable()
        {
            this.DragNodeManager = new DragNodeManager();
            this.DragGridManager = new DragGridManager();
            this.CreateConnectionManager = new CreateConnectionManager();
            this.NodeSelectionInspector = new NodeSelectionInspector();
            this.DeleteNodeManager = new DeleteNodeManager();
            this.NodeCreationManager = new NodeCreationManager();
            this.EditorZoomManager = new EditorZoomManager();
            this.NodeSizeFitterManager = new NodeSizeFitterManager();
            this.OnEnable_Impl();
        }

        protected abstract void OnEnable_Impl();

        protected abstract Type NodeEditorProfileType { get; }

        protected abstract Dictionary<string, Type> NodePickerConfiguration { get; }

        public NodeEditorProfile NodeEditorProfile
        {
            set => nodeEditorProfile = value;
        }

        [SerializeField] protected NodeEditorProfile nodeEditorProfile;

        private DragNodeManager DragNodeManager;
        private DragGridManager DragGridManager;
        private NodeSelectionInspector NodeSelectionInspector;
        private CreateConnectionManager CreateConnectionManager;
        private DeleteNodeManager DeleteNodeManager;
        private NodeCreationManager NodeCreationManager;
        private EditorZoomManager EditorZoomManager;
        private NodeSizeFitterManager NodeSizeFitterManager;

        private TreePickerPopup NodePicker;

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            nodeEditorProfile = (NodeEditorProfile)EditorGUILayout.ObjectField(this.nodeEditorProfile,
                NodeEditorProfileType, false, GUILayout.Width(150f));
            if (EditorGUI.EndChangeCheck())
            {
                if (nodeEditorProfile != null)
                {
                    nodeEditorProfile.Init();
                }
            }

            if (nodeEditorProfile != null)
            {
                if (this.NodePicker == null)
                {
                    this.NodePicker = new TreePickerPopup(this.NodePickerConfiguration.Keys.ToList(),
                        () =>
                        {
                            this.NodeCreationManager.OnNodePickerChange(ref this.NodePicker, ref this.nodeEditorProfile,
                                NodePickerConfiguration, this);
                        }, this.nodeEditorProfile.NodeCreationPickerProfile.SelectedKey);
                    this.NodePicker.RepaintAction = () => { GUI.changed = true; };
                    this.NodePicker.WindowDimensions = this.nodeEditorProfile.NodeCreationPickerProfile.PickerSize;
                }

                nodeEditorProfile.NodeEditorZoomProfile.EditorZoomBound.size = this.position.size;

                nodeEditorProfile.EditorBound.size = this.position.size;

                EditorZoomArea.Begin(nodeEditorProfile.NodeEditorZoomProfile.ZoomScale,
                    nodeEditorProfile.NodeEditorZoomProfile.EditorZoomBound);

                foreach (var node in this.nodeEditorProfile.Nodes.Values)
                {
                    node.GUITick(ref this.nodeEditorProfile);
                }

                //draw connections 
                foreach (var node in this.nodeEditorProfile.Nodes.Values)
                {
                    foreach (var inputEdge in node.InputEdges)
                    {
                        inputEdge.GUIConnectionLines();
                    }

                    foreach (var outputEdge in node.OutputEdges)
                    {
                        outputEdge.GUIConnectionLines();
                    }
                }

                if (!this.NodeSelectionInspector.GUITick(ref this.nodeEditorProfile))
                {
                    if (!this.EditorZoomManager.GUITick(ref this.nodeEditorProfile))
                    {
                        if (!this.NodeCreationManager.GUITick(ref this.NodePicker))
                        {
                            if (!this.CreateConnectionManager.GUITick(ref this.nodeEditorProfile))
                            {
                                if (!this.DeleteNodeManager.GUITick(ref this.nodeEditorProfile))
                                {
                                    if (!this.NodeSizeFitterManager.GUITick(ref this.nodeEditorProfile))
                                    {
                                        if (!this.DragNodeManager.GUITick(ref this.nodeEditorProfile))
                                        {
                                            this.DragGridManager.GUITick(ref this.nodeEditorProfile);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                EditorZoomArea.End();

                this.OnGUI_Impl();
            }

            if (GUI.changed) Repaint();
        }

        protected abstract void OnGUI_Impl();



        #region External Event 
        public void OnAddNode(Vector2 mousePosition, Type nodeType)
        {
            NodeProfile.CreateNode((NodeProfile)CreateInstance(nodeType), ref this.nodeEditorProfile,
                mousePosition);
            this.nodeEditorProfile.RefreshNodes();
        }
        public void OnManuallyNodeSelected(int nodeID)
        {
            var selectedNode = this.nodeEditorProfile.Nodes[nodeID];
            this.NodeSelectionInspector.SetActiveObjects(new List<Object>() { selectedNode }, ref this.nodeEditorProfile);
            //move selected node to center
            var newWindowPosition = new Vector2((selectedNode.Bounds.position.x * -1) + (this.nodeEditorProfile.EditorBound.width / 2) / this.nodeEditorProfile.NodeEditorZoomProfile.ZoomScale,
                               (selectedNode.Bounds.position.y * -1) + (this.nodeEditorProfile.EditorBound.height / 2) / this.nodeEditorProfile.NodeEditorZoomProfile.ZoomScale);
            this.nodeEditorProfile.EditorBound.position = newWindowPosition;
        }
        #endregion
    }

    class NodeCreationManager
    {
        private Vector2 pickerPosition;

        public bool GUITick(ref TreePickerPopup NodePicker)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                this.pickerPosition = Event.current.mousePosition;
                PopupWindow.Show(new Rect(this.pickerPosition, Vector2.zero), NodePicker);
                return true;
            }

            return false;
        }

        public void OnNodePickerChange(ref TreePickerPopup NodePicker,
            ref NodeEditorProfile nodeEditorProfile,
            Dictionary<string, Type> nodePickerConfiguration,
            NodeEditor nodeEditor)
        {
            nodeEditorProfile.NodeCreationPickerProfile.SelectedKey = NodePicker.SelectedKey;
            var nodeType = nodePickerConfiguration[nodeEditorProfile.NodeCreationPickerProfile.SelectedKey];
            nodeEditor.OnAddNode(this.pickerPosition, nodeType);
            //  NodePicker
        }
    }

    class CreateConnectionManager
    {
        private NodeEdgeProfile StartEdge;
        private NodeEdgeProfile EndEdge;

        public bool GUITick(ref NodeEditorProfile NodeEditorProfile)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                this.StartEdge = this.GetOutputClickedEdge(ref NodeEditorProfile);
            }
            else
            {
                if (this.StartEdge != null)
                {
                    if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                    {
                        this.EndEdge = this.GetInputClickedEdge(ref NodeEditorProfile);

                        if (this.EndEdge == null || this.EndEdge == this.StartEdge)
                        {
                            this.CancelConnection();
                        }
                        else
                        {
                            this.StartEdge.AddConnectedNode(this.EndEdge);
                            GUI.changed = true;
                        }
                    }
                    else if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                    {
                        Handles.DrawLine(this.StartEdge.Bounds.position, Event.current.mousePosition);
                        GUI.changed = true;
                    }
                }
            }

            return this.StartEdge != null;
        }

        private NodeEdgeProfile GetInputClickedEdge(ref NodeEditorProfile NodeEditorProfile)
        {
            foreach (var node in NodeEditorProfile.Nodes.Values)
            {
                var clickedEdge = node.IsContainedInInputEdge(Event.current.mousePosition);
                if (clickedEdge != null)
                {
                    return clickedEdge;
                }
            }

            return null;
        }

        private NodeEdgeProfile GetOutputClickedEdge(ref NodeEditorProfile NodeEditorProfile)
        {
            foreach (var node in NodeEditorProfile.Nodes.Values)
            {
                var clickedEdge = node.IsContainedInOutputEdge(Event.current.mousePosition);
                if (clickedEdge != null)
                {
                    return clickedEdge;
                }
            }

            return null;
        }

        private void CancelConnection()
        {
            this.StartEdge = null;
            this.EndEdge = null;
        }
    }

    class DeleteNodeManager
    {
        public bool GUITick(ref NodeEditorProfile nodeEditorProfile)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.A)
            {
                foreach (var currentSelectedObject in nodeEditorProfile.NodeEdtitorSelectionProfile.CurrentSelectedObjects)
                {
                    if (currentSelectedObject != null)
                    {
                        if (currentSelectedObject.GetType().IsSubclassOf(typeof(NodeEdgeProfile)))
                        {
                            this.DeleteEdge((NodeEdgeProfile)currentSelectedObject, ref nodeEditorProfile);
                        }
                        else if (currentSelectedObject.GetType().IsSubclassOf(typeof(NodeProfile)))
                        {
                            this.DeleteNode((NodeProfile)currentSelectedObject, ref nodeEditorProfile);
                        }
                    }
                }
                GUI.changed = true;
                return true;

            }

            return false;
        }

        private void DeleteNode(NodeProfile nodeProfile, ref NodeEditorProfile nodeEditorProfile)
        {
            nodeProfile.DeleteNode(ref nodeEditorProfile);
        }

        private void DeleteEdge(NodeEdgeProfile nodeEdgeProfile, ref NodeEditorProfile nodeEditorProfile)
        {
            nodeEdgeProfile.ClearConnections();
        }
    }

    class EditorZoomManager
    {
        public bool GUITick(ref NodeEditorProfile NodeEditorProfileRef)
        {
            if (Event.current.type == EventType.ScrollWheel)
            {
                NodeEditorProfileRef.NodeEditorZoomProfile.ZoomScale += (-Event.current.delta.y * 0.01f);
                NodeEditorProfileRef.NodeEditorZoomProfile.ZoomScale =
                    Mathf.Clamp(NodeEditorProfileRef.NodeEditorZoomProfile.ZoomScale, 0.2f, 5f);
                GUI.changed = true;
                return true;
            }

            return false;
        }
    }

    class NodeSizeFitterManager
    {
        public bool GUITick(ref NodeEditorProfile NodeEditorProfileRef)
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Q)
            {
                foreach (var currentSelectedObject in NodeEditorProfileRef.NodeEdtitorSelectionProfile.CurrentSelectedObjects)
                {
                    if (currentSelectedObject.GetType()
                  .IsSubclassOf(typeof(NodeProfile)))
                    {
                        ((NodeProfile)currentSelectedObject)
                            .AutoAdjustNodeSize();

                    }
                }
                GUI.changed = true;
                return true;
            }

            return false;
        }
    }

    class DragNodeManager
    {
        private bool isDragging = false;

        internal bool GUITick(ref NodeEditorProfile NodeEditorProfileRef)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                this.isDragging = false;
                var pointedNode = NodeEditorProfile.GetFirstContainedNode(Event.current.mousePosition, ref NodeEditorProfileRef);
                if (pointedNode != null)
                {
                    foreach (var currentSelectedNode in NodeEditorProfileRef.NodeEdtitorSelectionProfile.CurrentSelectedObjects)
                    {
                        if (currentSelectedNode.GetType().IsSubclassOf(typeof(NodeProfile)))
                        {
                            var selectedNodeProfile = (NodeProfile)currentSelectedNode;
                            if (pointedNode.Id == selectedNodeProfile.Id)
                            {
                                this.isDragging = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (this.isDragging && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    foreach (var currentSelectedNode in NodeEditorProfileRef.NodeEdtitorSelectionProfile.CurrentSelectedObjects)
                    {
                        if (currentSelectedNode.GetType().IsSubclassOf(typeof(NodeProfile)))
                        {
                            var selectedNodeProfile = (NodeProfile)currentSelectedNode;
                            selectedNodeProfile.Move(Event.current.delta);
                        }
                    }

                    GUI.changed = true;
                    return true;
                }
            }

            return false;
        }
    }

    class NodeSelectionInspector
    {
        private bool isDragging;
        private Rect selectionRect;
        private Color selectedColor;

        public bool GUITick(ref NodeEditorProfile NodeEditorProfile)
        {
            this.selectedColor = NodeEditorProfile.SelectedBackgoundColor;

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && Event.current.alt)
            {
                this.isDragging = true;
                this.selectionRect = new Rect(Event.current.mousePosition, Vector2.zero);
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {

                if (this.isDragging)
                {
                    this.AdjustSelectionRectToStartTopLeft();
                    this.SetActiveObjects(
                        NodeEditorProfile.GetAllContainedNode(this.selectionRect, ref NodeEditorProfile).ConvertAll(n => (Object)n),
                        ref NodeEditorProfile);
                }
                else
                {
                    var selectedNode =
                         NodeEditorProfile.GetFirstContainedNode(Event.current.mousePosition, ref NodeEditorProfile);
                    if (selectedNode != null)
                    {
                        var selectedEdge = selectedNode.IsContainedInInputEdge(Event.current.mousePosition);
                        if (selectedEdge == null)
                        {
                            selectedEdge = selectedNode.IsContainedInOutputEdge(Event.current.mousePosition);
                        }

                        if (selectedEdge != null)
                        {
                            this.SetActiveObjects(new List<Object>() { selectedEdge }, ref NodeEditorProfile);
                        }
                        else
                        {
                            this.SetActiveObjects(new List<Object>() { selectedNode }, ref NodeEditorProfile);
                        }
                    }
                }
                this.isDragging = false;
            }

            if (this.isDragging && Event.current.alt)
            {
                if (Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    this.selectionRect.size = this.selectionRect.size + Event.current.delta;
                }

                EditorGUI.DrawRect(this.selectionRect, NodeEditorProfile.SlectionRectangleColor);
                GUI.changed = true;
            }

            return this.isDragging;
        }

        public void SetActiveObjects(List<Object> newObjects, ref NodeEditorProfile nodeEditorProfile)
        {
            nodeEditorProfile.NodeEdtitorSelectionProfile.OldSelectedObjects =
                nodeEditorProfile.NodeEdtitorSelectionProfile.CurrentSelectedObjects;
            nodeEditorProfile.NodeEdtitorSelectionProfile.CurrentSelectedObjects = newObjects;

            if (nodeEditorProfile.NodeEdtitorSelectionProfile.OldSelectedObjects != null)
            {
                foreach (var oldSelectedNode in nodeEditorProfile.NodeEdtitorSelectionProfile.OldSelectedObjects)
                {
                    this.SetObjectSelectedFlag(oldSelectedNode, false);
                }

            }

            foreach (var newSelectedNode in nodeEditorProfile.NodeEdtitorSelectionProfile.CurrentSelectedObjects)
            {
                this.SetObjectSelectedFlag(newSelectedNode, true);
            }
            if (nodeEditorProfile.NodeEdtitorSelectionProfile.CurrentSelectedObjects != null && nodeEditorProfile.NodeEdtitorSelectionProfile.CurrentSelectedObjects.Count > 0)
            {
                Selection.activeObject = nodeEditorProfile.NodeEdtitorSelectionProfile.CurrentSelectedObjects[0];
            }
        }

        private void SetObjectSelectedFlag(Object obj, bool value)
        {
            if (obj.GetType().IsSubclassOf(typeof(NodeEdgeProfile)))
            {
                ((NodeEdgeProfile)obj).SetIsSelected(value, this.selectedColor);
            }
            else if (obj.GetType().IsSubclassOf(typeof(NodeProfile)))
            {
                ((NodeProfile)obj).SetIsSelected(value, this.selectedColor);
            }

            GUI.changed = true;
        }

        private void AdjustSelectionRectToStartTopLeft()
        {
            if (this.selectionRect.size.x < 0)
            {
                var delta = this.selectionRect.size.x;
                this.selectionRect.position = this.selectionRect.position + new Vector2(delta, 0);
                this.selectionRect.size = new Vector2(Math.Abs(delta), this.selectionRect.size.y);
            }
            if(this.selectionRect.size.y < 0)
            {
                var delta = this.selectionRect.size.y;
                this.selectionRect.position = this.selectionRect.position + new Vector2(0, delta);
                this.selectionRect.size = new Vector2(this.selectionRect.size.x, Math.Abs(delta));
            }
        }

    }

    class DragGridManager
    {
        private bool isDragging = false;

        internal void GUITick(ref NodeEditorProfile nodeEditorProfile)
        {
            if (Event.current.button == 0)
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    this.isDragging = true;
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    this.isDragging = false;
                }
                else if (Event.current.type == EventType.MouseDrag)
                {
                    nodeEditorProfile.Drag(Event.current.delta);
                    GUI.changed = true;
                }
            }
        }
    }

}