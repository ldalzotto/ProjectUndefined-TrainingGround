using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OdinSerializer;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NodeGraph
{
    [System.Serializable]
    public abstract class NodeEditorProfile : SerializedScriptableObject
    {
        [SerializeField]
        public string GraphTmpFolderPath;
        [SerializeField]
        public string NodesTmpFolderPath;

        [SerializeField]
        public Rect EditorBound = new Rect(new Vector2(0, 0), new Vector2(0, 0));

        [SerializeField]
        public Dictionary<int, NodeProfile> Nodes;

        [SerializeField]
        public Color SelectedBackgoundColor = new Color(0.63f, 1f, 0.95f);
        [SerializeField]
        public Color SlectionRectangleColor = new Color(1f, 1f, 0f, 0.3f);

        [SerializeField]
        public NodeEditorZoomProfile NodeEditorZoomProfile;
        [SerializeField]
        public NodeEditorGridProfile NodeEditorGridProfile;
        [SerializeField]
        public NodeEdtitorSelectionProfile NodeEdtitorSelectionProfile;
        [SerializeField]
        public NodeCreationPickerProfile NodeCreationPickerProfile;

#if UNITY_EDITOR
        public virtual void Init()
        {
            if(this.Nodes == null)
            {
                this.Nodes = new Dictionary<int, NodeProfile>();
            }
            if (string.IsNullOrEmpty(this.GraphTmpFolderPath))
            {
                var graphTmpFolderFullPath = FileUtil.GetAssetDirectoryPath(this) + this.name + "_tmp";
                var di = new DirectoryInfo(graphTmpFolderFullPath);
                if (!di.Exists)
                {
                    di.Create();
                }

                GraphTmpFolderPath = graphTmpFolderFullPath;
            }

            if (string.IsNullOrEmpty(this.NodesTmpFolderPath))
            {
                this.NodesTmpFolderPath = this.GraphTmpFolderPath + "/Nodes";
                var di = new DirectoryInfo(NodesTmpFolderPath);
                if (!di.Exists)
                {
                    di.Create();
                }
            }

            this.RefreshAll();
        }

        private void RefreshAll()
        {
            this.RefreshNodes();
        }

        public void RefreshNodes()
        {
            this.Nodes.Clear();
            var nodes = AssetDatabase
                .FindAssets("t:" + typeof(NodeProfile).Name, new string[] { this.NodesTmpFolderPath }).ToList()
                .ConvertAll((p) =>
                    (NodeProfile)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(p), typeof(NodeProfile)));
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var castedNode = (NodeProfile)node;
                    this.Nodes.Add(castedNode.Id, castedNode);
                }
            }
            EditorUtility.SetDirty(this);
        }

        public void Drag(Vector2 delta)
        {
            this.EditorBound.position += delta;
        }

        public static NodeProfile GetFirstContainedNode(Vector2 position, ref NodeEditorProfile NodeEditorProfileRef)
        {
            foreach (var node in NodeEditorProfileRef.Nodes.Values)
            {
                if (node.OffsettedBounds.Contains(position))
                {
                    return node;
                }
            }

            return null;
        }

        public static List<NodeProfile> GetAllContainedNode(Rect selectionRect, ref NodeEditorProfile NodeEditorProfileRef)
        {
            var foundedNodes = new List<NodeProfile>();
            foreach (var node in NodeEditorProfileRef.Nodes.Values)
            {
                if (node.OffsettedBounds.Overlaps(selectionRect))
                {
                    foundedNodes.Add(node);
                }
            }
            return foundedNodes;
        }

        internal void OnDeletedNode(NodeProfile nodeProfile)
        {
            this.Nodes.Remove(nodeProfile.Id);
        }
        
        protected override void OnAfterDeserialize()
        {
            base.OnAfterDeserialize();
        }

        protected override void OnBeforeSerialize()
        {
            base.OnBeforeSerialize();
        }
#endif
    }

    [Serializable]
    public class NodeEditorZoomProfile
    {
        public Rect EditorZoomBound = new Rect(Vector2.zero, Vector2.zero);
        public float ZoomScale = 1f;
        public Vector2 ZoomCoordsOrigin = Vector2.zero;
    }

    [Serializable]
    public class NodeEditorGridProfile
    {
        public float GridSpacing = 5;
        public int StrongColorGridPeriod = 10;
        public Color WeakColor = new Color(0.6603774f, 0.6603774f, 0.6603774f);
        public Color StrongColor = Color.grey;
    }

    [Serializable]
    public class NodeEdtitorSelectionProfile
    {
        public List<Object> CurrentSelectedObjects;
        public List<Object> OldSelectedObjects;
    }

    [Serializable]
    public class NodeCreationPickerProfile
    {
        public string SelectedKey;

        public Vector2 PickerSize = new Vector2(200, 300);
    }
}