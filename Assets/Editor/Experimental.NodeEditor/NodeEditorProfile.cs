using UnityEngine;
using System.Collections;
using OdinSerializer;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Experimental.Editor_NodeEditor
{
    [CreateAssetMenu(fileName = "NodeEditorProfile", menuName = "Experimental/NodeEditorProfile", order = 1)]
    [System.Serializable]
    public class NodeEditorProfile : SerializedScriptableObject
    {
        public const string RELATIVE_GRAPH_TMP_FOLDER_PATH = "tmp";
        public string GraphTmpFolderPath;
        public string NodesTmpFolderPath;

        public Rect EditorBound = new Rect(new Vector2(0, 0), new Vector2(0, 0));
        public float ZoomScale = 1f;
        public Dictionary<int, NodeProfile> Nodes = new Dictionary<int, NodeProfile>();

        public Color SelectedBackgoundColor = new Color(0.63f, 1f, 0.95f);
        public NodeEditorGridProfile NodeEditorGridProfile;
        public NodeEdtitorSelectionProfile NodeEdtitorSelectionProfile;

        public void Init()
        {
            if (string.IsNullOrEmpty(this.GraphTmpFolderPath))
            {
                var graphTmpFolderFullPath = FileUtil.GetAssetDirectoryPath(this) + RELATIVE_GRAPH_TMP_FOLDER_PATH;
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

        internal void RefreshNodes()
        {
            this.Nodes.Clear();
            var nodes = AssetDatabase.FindAssets("t:" + typeof(NodeProfile).Name, new string[] { this.NodesTmpFolderPath }).ToList().ConvertAll((p) => (NodeProfile)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(p), typeof(NodeProfile)));
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var castedNode = (NodeProfile)node;
                    this.Nodes.Add(castedNode.Id, castedNode);
                }
            }
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

        internal void OnDeletedNode(NodeProfile nodeProfile)
        {
            this.Nodes.Remove(nodeProfile.Id);
        }
    }

    [System.Serializable]
    public class NodeEditorGridProfile
    {
        public float GridSpacing = 5;
        public int StrongColorGridPeriod = 10;
        public Color WeakColor = new Color(0.6603774f, 0.6603774f, 0.6603774f);
        public Color StrongColor = Color.grey;
    }

    [System.Serializable]
    public class NodeEdtitorSelectionProfile
    {
        public UnityEngine.Object currentSelectedObject;
        public UnityEngine.Object oldSelectedObject;
    }

}