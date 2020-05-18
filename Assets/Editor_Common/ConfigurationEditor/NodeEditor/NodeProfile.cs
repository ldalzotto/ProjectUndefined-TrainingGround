﻿using UnityEngine;
using System.Collections;
using OdinSerializer;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace NodeGraph
{
    [System.Serializable]
    public abstract class NodeProfile : SerializedScriptableObject
    {
        public int Id;

        public List<NodeEdgeProfile> InputEdges = new List<NodeEdgeProfile>();
        public List<NodeEdgeProfile> OutputEdges = new List<NodeEdgeProfile>();

#if UNITY_EDITOR
        public Rect Bounds = new Rect(new Vector2(0, 0), new Vector2(100, 100));
        public string EdgesDirectoryPath;
        protected virtual Color NodeColor()
        {
            return Color.gray;
        }
        protected virtual Vector2 Size()
        {
            return new Vector2(100, 100);
        }
        protected virtual string NodeTitle() { return this.GetType().Name; }

        private Rect offsettedBounds = new Rect();
        private Rect currentInnerNodeRect = new Rect();

        private bool IsSelected = false;
        private Color selectedColor;
        public Rect OffsettedBounds { get => offsettedBounds; }

        public static void CreateNode(NodeProfile nodeInstance, ref NodeEditorProfile nodeEditorProfileRef, Vector2 startPosition)
        {
            var randomNodeId = UnityEngine.Random.Range(0, 99999);
            while (nodeEditorProfileRef.Nodes.ContainsKey(randomNodeId))
            {
                randomNodeId = UnityEngine.Random.Range(0, 99999);
            }
            nodeInstance.Id = randomNodeId;
            nodeInstance.Bounds.position = startPosition - nodeEditorProfileRef.EditorBound.position;
            nodeInstance.Bounds.size = nodeInstance.Size();
            nodeInstance.EdgesDirectoryPath = nodeEditorProfileRef.NodesTmpFolderPath + "/Node_" + nodeInstance.Id.ToString() + "_Edges";
            var edgeDI = new DirectoryInfo(nodeInstance.EdgesDirectoryPath);
            if (!edgeDI.Exists)
            {
                edgeDI.Create();
            }

            nodeInstance.InputEdges = nodeInstance.InitInputEdges();
            nodeInstance.OutputEdges = nodeInstance.InitOutputEdges();

            AssetDatabase.CreateAsset(nodeInstance, nodeEditorProfileRef.NodesTmpFolderPath + "/Node_" + nodeInstance.Id.ToString() + ".asset");
        }

        public abstract List<NodeEdgeProfile> InitInputEdges();
        public abstract List<NodeEdgeProfile> InitOutputEdges();

        public void GUITick(ref NodeEditorProfile nodeEditorProfileRef)
        {
            this.offsettedBounds.position = this.Bounds.position + nodeEditorProfileRef.EditorBound.position;
            this.offsettedBounds.size = this.Bounds.size;

            var oldBackGouncColor = GUI.backgroundColor;
            GUI.backgroundColor = this.NodeColor();
            if (this.IsSelected)
            {
                GUI.backgroundColor = this.selectedColor;
            }

            EditorGUI.BeginChangeCheck();
            
            GUILayout.BeginArea(this.offsettedBounds, this.NodeTitle(), GUI.skin.window);
            this.currentInnerNodeRect = EditorGUILayout.BeginVertical();
            this.NodeGUI(ref nodeEditorProfileRef);
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
            GUI.backgroundColor = oldBackGouncColor;

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
            }
        }

        protected virtual void NodeGUI(ref NodeEditorProfile nodeEditorProfileRef)
        {
            var maxEdgeIndex = Mathf.Max(this.InputEdges.Count, this.OutputEdges.Count);
            for (var i = 0; i < maxEdgeIndex; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (i < this.InputEdges.Count)
                {
                    this.InputEdges[i].GUIEdgeRectangles(this.offsettedBounds);
                }
                if (i < this.OutputEdges.Count)
                {
                    this.OutputEdges[i].GUIEdgeRectangles(this.offsettedBounds);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        protected void AddInputEdge(NodeEdgeProfile nodeEdgeProfile)
        {
            this.InputEdges.Add(nodeEdgeProfile);
        }

        protected void AddOutputEdge(NodeEdgeProfile nodeEdgeProfile)
        {
            this.OutputEdges.Add(nodeEdgeProfile);
        }

        public void DeleteNode(ref NodeEditorProfile nodeEditorProfileRef)
        {
            foreach (var inputNodeEdge in this.InputEdges)
            {
                inputNodeEdge.ClearBackwardConnections();
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(inputNodeEdge));
            }
            this.InputEdges.Clear();
            foreach (var outputNodeEdge in this.OutputEdges)
            {
                outputNodeEdge.ClearConnections();
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(outputNodeEdge));
            }
            this.OutputEdges.Clear();

            nodeEditorProfileRef.OnDeletedNode(this);
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this));
            Directory.Delete(this.EdgesDirectoryPath);
        }

        protected void DeleteEdge(NodeEdgeProfile edgeToDelete)
        {
            this.InputEdges.Remove(edgeToDelete);
            this.OutputEdges.Remove(edgeToDelete);
            edgeToDelete.ClearBackwardConnections();
            edgeToDelete.ClearConnections();
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(edgeToDelete));
        }

        public void Move(Vector2 delta)
        {
            this.Bounds.position += delta;
            EditorUtility.SetDirty(this);
        }

        public NodeEdgeProfile IsContainedInInputEdge(Vector2 position)
        {
            foreach (var inputEdge in this.InputEdges)
            {
                if (inputEdge.Bounds.Contains(position))
                {
                    return inputEdge;
                }
            }
            return null;
        }

        public NodeEdgeProfile IsContainedInOutputEdge(Vector2 position)
        {

            foreach (var outputEdge in this.OutputEdges)
            {
                if (outputEdge.Bounds.Contains(position)) { return outputEdge; }
            }
            return null;
        }

        private Rect GetInputEdgeRect(int index)
        {
            float deltaHeight = 20f;
            for (var i = 0; i < index; i++)
            {
                deltaHeight += this.InputEdges[i].Bounds.size.y;
            }
            return new Rect(offsettedBounds.position + new Vector2(0, deltaHeight), new Vector2(this.offsettedBounds.width / 2, 0));
        }
        private Rect GetOutputEdgeRect(int index)
        {
            float deltaHeight = 20f;
            for (var i = 0; i < index; i++)
            {
                deltaHeight += this.OutputEdges[i].Bounds.size.y;
            }
            return new Rect(offsettedBounds.position + new Vector2(0, deltaHeight) + new Vector2(this.offsettedBounds.width / 2, 0), new Vector2(this.offsettedBounds.width / 2, 0));
        }

        public int GetNextEdgeId()
        {
            var inputEdgesid = this.InputEdges.ConvertAll(e => e.Id);
            var outputEdgesId = this.OutputEdges.ConvertAll(e => e.Id);
            var id = UnityEngine.Random.Range(0, 99999);
            while (inputEdgesid.Contains(id) || outputEdgesId.Contains(id))
            {
                id = UnityEngine.Random.Range(0, 99999);
            }
            return id;
        }

        public void SetIsSelected(bool value, Color selectionColor)
        {
            this.IsSelected = value;
            this.selectedColor = selectionColor;
        }

        public void AutoAdjustNodeSize()
        {
            this.Bounds.size = new Vector2(this.offsettedBounds.size.x, this.currentInnerNodeRect.height + 25);
        }
#endif

    }



}

