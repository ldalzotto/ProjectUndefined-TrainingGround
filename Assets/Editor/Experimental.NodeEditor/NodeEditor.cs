using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace Experimental.Editor_NodeEditor
{
    public class NodeEditor : EditorWindow
    {
        [MenuItem("Experimental/NodeEditor")]
        static void Init()
        {
            NodeEditor window = (NodeEditor)EditorWindow.GetWindow(typeof(NodeEditor));
            window.Show();
        }


        private void OnEnable()
        {
            this.NodeEditorContextMenu = new NodeEditorContextMenu();
            this.NodeEditorContextMenu.Init(this);
            this.DragNodeManager = new DragNodeManager();
            this.DragGridManager = new DragGridManager();
            this.GridDrawer = new GridDrawer();
            this.CreateConnectionManager = new CreateConnectionManager();
            this.NodeSelectionInspector = new NodeSelectionInspector();
            this.DeleteNodeManager = new DeleteNodeManager();
        }

        [SerializeField]
        private NodeEditorProfile NodeEditorProfile;

        private NodeEditorContextMenu NodeEditorContextMenu;
        private DragNodeManager DragNodeManager;
        private DragGridManager DragGridManager;
        private GridDrawer GridDrawer;
        private NodeSelectionInspector NodeSelectionInspector;
        private CreateConnectionManager CreateConnectionManager;
        private DeleteNodeManager DeleteNodeManager;

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            NodeEditorProfile = (NodeEditorProfile)EditorGUILayout.ObjectField(this.NodeEditorProfile, typeof(NodeEditorProfile), false, GUILayout.Width(150f));
            if (EditorGUI.EndChangeCheck())
            {
                if (NodeEditorProfile != null)
                {
                    NodeEditorProfile.Init();
                }
            }
            if (NodeEditorProfile != null)
            {
                NodeEditorProfile.EditorBound.size = this.position.size;
                this.GridDrawer.GUITick(ref this.NodeEditorProfile);
                this.NodeEditorContextMenu.GUITick();

                foreach (var node in this.NodeEditorProfile.Nodes.Values)
                {
                    node.GUITick(ref this.NodeEditorProfile);
                }

                this.NodeSelectionInspector.GUITick(ref this.NodeEditorProfile);
                
                if (!this.CreateConnectionManager.GUITick(ref this.NodeEditorProfile))
                {
                    if(!this.DeleteNodeManager.GUITick(ref this.NodeEditorProfile))
                    {
                        if (!this.DragNodeManager.GUITick(ref this.NodeEditorProfile))
                        {
                            this.DragGridManager.GUITick(ref this.NodeEditorProfile);
                        }
                    }
                }



            }
            if (GUI.changed) Repaint();
        }

        internal void OnAddNode(Vector2 mousePosition)
        {
            NodeProfile.CreateNode((NodeProfile)ScriptableObject.CreateInstance(typeof(IntAdditionNode)), ref this.NodeEditorProfile, mousePosition);
            this.NodeEditorProfile.RefreshNodes();
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
                var currentSelectedObject = nodeEditorProfile.NodeEdtitorSelectionProfile.currentSelectedObject;
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
                    GUI.changed = true;
                    return true;
                }
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
    class DragNodeManager
    {
        private NodeProfile selectedNode;
        internal bool GUITick(ref NodeEditorProfile NodeEditorProfileRef)
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                this.selectedNode = NodeEditorProfile.GetFirstContainedNode(Event.current.mousePosition, ref NodeEditorProfileRef);
            }
            else
            {
                if (this.selectedNode != null && Event.current.type == EventType.MouseDrag && Event.current.button == 0)
                {
                    this.selectedNode.Move(Event.current.delta);
                    GUI.changed = true;
                    return true;
                }
            }
            return false;
        }

    }

    class NodeSelectionInspector
    {

        private Color selectedColor;

        private void OnSelectionChanged()
        {

        }

        public void GUITick(ref NodeEditorProfile NodeEditorProfile)
        {
            this.selectedColor = NodeEditorProfile.SelectedBackgoundColor;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                var selectedNode = NodeEditorProfile.GetFirstContainedNode(Event.current.mousePosition, ref NodeEditorProfile);
                if (selectedNode != null)
                {
                    var selectedEdge = selectedNode.IsContainedInInputEdge(Event.current.mousePosition);
                    if (selectedEdge == null)
                    {
                        selectedEdge = selectedNode.IsContainedInOutputEdge(Event.current.mousePosition);
                    }
                    if (selectedEdge != null)
                    {
                        SetActiveObject(selectedEdge, ref NodeEditorProfile);
                    }
                    else
                    {
                        SetActiveObject(selectedNode, ref NodeEditorProfile);
                    }
                }
            }
        }

        private void SetActiveObject(UnityEngine.Object newObject, ref NodeEditorProfile nodeEditorProfile)
        {
            nodeEditorProfile.NodeEdtitorSelectionProfile.oldSelectedObject = nodeEditorProfile.NodeEdtitorSelectionProfile.currentSelectedObject;
            nodeEditorProfile.NodeEdtitorSelectionProfile.currentSelectedObject = newObject;
            if (nodeEditorProfile.NodeEdtitorSelectionProfile.oldSelectedObject != null)
            {
                this.SetObjectSelectedFlag(nodeEditorProfile.NodeEdtitorSelectionProfile.oldSelectedObject, false);
            }
            this.SetObjectSelectedFlag(nodeEditorProfile.NodeEdtitorSelectionProfile.currentSelectedObject, true);
            Selection.activeObject = newObject;
        }

        private void SetObjectSelectedFlag(UnityEngine.Object obj, bool value)
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

        private bool IsNodeOrEdge(object selectedObject)
        {
            return selectedObject.GetType().IsSubclassOf(typeof(NodeEdgeProfile)) || selectedObject.GetType().IsSubclassOf(typeof(NodeProfile));
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

    class GridDrawer
    {

        public void GUITick(ref NodeEditorProfile nodeEditorProfile)
        {

            int widthDivs = Mathf.CeilToInt(nodeEditorProfile.EditorBound.width / nodeEditorProfile.NodeEditorGridProfile.GridSpacing);
            int heightDivs = Mathf.CeilToInt(nodeEditorProfile.EditorBound.height / nodeEditorProfile.NodeEditorGridProfile.GridSpacing);

            var oldColor = Handles.color;

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.color = nodeEditorProfile.NodeEditorGridProfile.WeakColor;
                if ((i % nodeEditorProfile.NodeEditorGridProfile.StrongColorGridPeriod) == 0)
                {
                    Handles.color = nodeEditorProfile.NodeEditorGridProfile.StrongColor;
                }
                Handles.DrawLine(new Vector3(i * nodeEditorProfile.NodeEditorGridProfile.GridSpacing, nodeEditorProfile.EditorBound.yMax, 0),
                    new Vector3(i * nodeEditorProfile.NodeEditorGridProfile.GridSpacing, nodeEditorProfile.EditorBound.yMin, 0));
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.color = nodeEditorProfile.NodeEditorGridProfile.WeakColor;
                if ((j % nodeEditorProfile.NodeEditorGridProfile.StrongColorGridPeriod) == 0)
                {
                    Handles.color = nodeEditorProfile.NodeEditorGridProfile.StrongColor;
                }
                Handles.DrawLine(new Vector3(nodeEditorProfile.EditorBound.xMax, j * nodeEditorProfile.NodeEditorGridProfile.GridSpacing, 0),
                   new Vector3(nodeEditorProfile.EditorBound.xMin, j * nodeEditorProfile.NodeEditorGridProfile.GridSpacing, 0));
            }

            Handles.color = oldColor;
        }
    }

}
