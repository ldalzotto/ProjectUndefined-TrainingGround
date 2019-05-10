using CoreGame;
using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Editor_ConditionGraph
{

#if UNITY_EDITOR
    public abstract class ConditionGraphEditor : Editor
    {
        private int nodeToRemove = -9999;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            nodeToRemove = -9999;

            ConditionGraph myTarget = (ConditionGraph)target;
            if (!myTarget.NodeExists(ConditionGraph.ROOT_NODE_KEY))
            {
                myTarget.SetRootNode(myTarget.NodeProvider());
            }
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            this.RenderNode(ConditionGraph.ROOT_NODE_KEY);
            EditorGUILayout.EndVertical();
            this.RemoveNodeFormGraph();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }

        private void RenderNode(int nodeIndex)
        {
            ConditionGraph myTarget = (ConditionGraph)target;

            var conditionNode = myTarget.GetNode(nodeIndex);

            EditorGUILayout.BeginHorizontal();
            conditionNode.SpecificEditorRender();

            conditionNode.IsAnd = GUILayout.Toggle(conditionNode.IsAnd, new GUIContent("&&"), EditorStyles.miniButtonLeft);
            conditionNode.IsOr = GUILayout.Toggle(conditionNode.IsOr, new GUIContent("||"), EditorStyles.miniButtonRight);

            if (GUILayout.Button("+&&", EditorStyles.miniButtonLeft))
            {
                if (conditionNode.AndNestedNodeKey == null)
                {
                    conditionNode.AndNestedNodeKey = new List<int>();
                }
                var addedNodeIndex = myTarget.AddNode(myTarget.NodeProvider());
                conditionNode.AndNestedNodeKey.Add(addedNodeIndex);
            }
            if (GUILayout.Button("+||", EditorStyles.miniButtonRight))
            {
                if (conditionNode.OrNestedNodeKey == null)
                {
                    conditionNode.OrNestedNodeKey = new List<int>();
                }
                var addedNodeIndex = myTarget.AddNode(myTarget.NodeProvider());
                conditionNode.OrNestedNodeKey.Add(addedNodeIndex);
            }

            if (GUILayout.Button("+", EditorStyles.miniButtonLeft))
            {
                var addedNodeIndex = myTarget.AddNode(myTarget.NodeProvider());
                conditionNode.SameLevelNodesKey.Add(addedNodeIndex);
            }
            if (GUILayout.Button("-", EditorStyles.miniButtonRight))
            {
                nodeToRemove = nodeIndex;
            }

            EditorGUILayout.EndHorizontal();

            if (conditionNode.AndNestedNodeKey != null && conditionNode.AndNestedNodeKey.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                EditorGUILayout.LabelField("&&");
                this.RenderNode(conditionNode.AndNestedNodeKey[0]);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            if (conditionNode.OrNestedNodeKey != null && conditionNode.OrNestedNodeKey.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical(EditorStyles.textArea);
                EditorGUILayout.LabelField("||");
                this.RenderNode(conditionNode.OrNestedNodeKey[0]);
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            if (conditionNode.SameLevelNodesKey == null)
            {
                conditionNode.SameLevelNodesKey = new List<int>();
            }

            if (conditionNode != null)
            {
                foreach (var sameLevelNode in conditionNode.SameLevelNodesKey)
                {
                    this.RenderNode(sameLevelNode);
                }
            }

        }


        private void RemoveNodeFormGraph()
        {
            if (this.nodeToRemove >= ConditionGraph.ROOT_NODE_KEY)
            {
                var myTarget = (ConditionGraph)target;

                foreach (var node in myTarget.GetAllNodes())
                {
                    if (node.SameLevelNodesKey.Contains(this.nodeToRemove))
                    {
                        node.SameLevelNodesKey.Remove(this.nodeToRemove);
                    }
                    if (node.AndNestedNodeKey != null &&
                        node.AndNestedNodeKey.Contains(this.nodeToRemove))
                    {
                        node.AndNestedNodeKey = new List<int>();
                    }
                    if (node.OrNestedNodeKey != null &&
                        node.OrNestedNodeKey.Contains(this.nodeToRemove))
                    {
                        node.OrNestedNodeKey = new List<int>();
                    }
                }

                myTarget.RemoveNode(this.nodeToRemove);

            }

        }

    }
#endif

}

