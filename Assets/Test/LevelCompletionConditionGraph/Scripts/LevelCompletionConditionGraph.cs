using OdinSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using static FakeEnums;

[CreateAssetMenu(fileName = "LevelCompletionConditionGraph", menuName = "Test/LevelCompletionConditionGraph", order = 1)]
[System.Serializable]
public class LevelCompletionConditionGraph : SerializedScriptableObject
{
    public const int ROOT_NODE_KEY = -1;

    [SerializeField]
    private LevelCompletionConditionNode levelCompletionConditionRootNode;

    [SerializeField]
    private Dictionary<int, LevelCompletionConditionNode> levelCompletionGraph = new Dictionary<int, LevelCompletionConditionNode>();

    private LevelCompletionConditionGraphResolutionInput LevelCompletionConditionGraphResolutionInput = new LevelCompletionConditionGraphResolutionInput();
    public LevelCompletionConditionNode LevelCompletionConditionRootNode { get => levelCompletionConditionRootNode; set => levelCompletionConditionRootNode = value; }
    public Dictionary<int, LevelCompletionConditionNode> LevelCompletionGraph { get => levelCompletionGraph; }

    public bool ResolveGraph()
    {
        return this.levelCompletionConditionRootNode.ResolveNode(ref LevelCompletionConditionGraphResolutionInput, this);
    }

    public int SetRootNode(LevelCompletionConditionNode levelCompletionConditionNode)
    {
        this.levelCompletionGraph[ROOT_NODE_KEY] = levelCompletionConditionNode;
        this.levelCompletionConditionRootNode = levelCompletionConditionNode;
        return ROOT_NODE_KEY;
    }

    public int AddNode(LevelCompletionConditionNode levelCompletionConditionNode)
    {
        int key = 0;
        if (this.levelCompletionGraph.Keys.Count > 0)
        {
            key = (this.levelCompletionGraph.Keys.ToList().Max() + 1);
        }
        this.levelCompletionGraph[key] = levelCompletionConditionNode;
        return key;
    }

    public void RemoveNode(int nodeIndex)
    {
        this.levelCompletionGraph.Remove(nodeIndex);
    }

    public LevelCompletionConditionNode GetNode(int index)
    {
        return this.levelCompletionGraph[index];
    }

    public bool NodeExists(int index)
    {
        return this.levelCompletionGraph.ContainsKey(index);
    }
}

[System.Serializable]
public class LevelCompletionConditionNode
{
    public KEY_ID Key;
    public KEY_ID Value;

    public bool IsAnd;
    public bool IsOr;

    public bool NestedStart;
    public bool NestedEnd;

    public List<int> SameLevelNodesKey;

    public List<int> AndNestedNodeKey;
    public List<int> OrNestedNodeKey;

    public LevelCompletionConditionNode()
    {
    }

    private bool ConditionEvaluation()
    {
        return (Key == Value);
    }

    public bool ResolveNode(ref LevelCompletionConditionGraphResolutionInput LevelCompletionConditionGraphResolutionInput, LevelCompletionConditionGraph LevelCompletionConditionGraph)
    {
        bool returnValue = this.ConditionEvaluation();

        if (this.SameLevelNodesKey != null)
        {
            foreach (var sameLevelNodeKey in this.SameLevelNodesKey)
            {
                var involvedNode = LevelCompletionConditionGraph.GetNode(sameLevelNodeKey);
                if (involvedNode.IsAnd)
                {
                    returnValue = returnValue && involvedNode.ResolveNode(ref LevelCompletionConditionGraphResolutionInput, LevelCompletionConditionGraph);
                }
                else if (involvedNode.IsOr)
                {
                    returnValue = returnValue || involvedNode.ResolveNode(ref LevelCompletionConditionGraphResolutionInput, LevelCompletionConditionGraph);
                }
            }
        }

        if (AndNestedNodeKey != null && AndNestedNodeKey.Count > 0)
        {
            var involvedNode = LevelCompletionConditionGraph.GetNode(AndNestedNodeKey[0]);
            returnValue = returnValue && involvedNode.ResolveNode(ref LevelCompletionConditionGraphResolutionInput, LevelCompletionConditionGraph);
        }
        if (OrNestedNodeKey != null && OrNestedNodeKey.Count > 0)
        {
            var involvedNode = LevelCompletionConditionGraph.GetNode(OrNestedNodeKey[0]);
            returnValue = returnValue || involvedNode.ResolveNode(ref LevelCompletionConditionGraphResolutionInput, LevelCompletionConditionGraph);
        }
        return returnValue;
    }

    public List<int> GetAllChildNodes()
    {
        var returnNodes = new List<int>(this.SameLevelNodesKey);
        if (this.AndNestedNodeKey != null)
        {
            returnNodes.AddRange(this.AndNestedNodeKey);
        }
        if (this.OrNestedNodeKey != null)
        {
            returnNodes.AddRange(this.OrNestedNodeKey);
        }
        return returnNodes;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LevelCompletionConditionGraph))]
public class LevelCompletionConditionGraphEditor : Editor
{
    private int nodeToRemove = -9999;

    public override void OnInspectorGUI()
    {
        nodeToRemove = -9999;

        LevelCompletionConditionGraph myTarget = (LevelCompletionConditionGraph)target;
        if (!myTarget.NodeExists(LevelCompletionConditionGraph.ROOT_NODE_KEY))
        {
            myTarget.SetRootNode(new LevelCompletionConditionNode());
        }
        EditorGUILayout.BeginVertical(EditorStyles.textArea);
        this.RenderNode(LevelCompletionConditionGraph.ROOT_NODE_KEY);
        EditorGUILayout.EndVertical();
        this.RemoveNodeFormGraph();
    }

    private void RenderNode(int nodeIndex)
    {
        LevelCompletionConditionGraph myTarget = (LevelCompletionConditionGraph)target;

        var levelCompletionConditionNode = myTarget.GetNode(nodeIndex);

        EditorGUILayout.BeginHorizontal();
        levelCompletionConditionNode.Key = (KEY_ID)EditorGUILayout.EnumPopup(levelCompletionConditionNode.Key);
        levelCompletionConditionNode.Value = (KEY_ID)EditorGUILayout.EnumPopup(levelCompletionConditionNode.Value);

        levelCompletionConditionNode.IsAnd = GUILayout.Toggle(levelCompletionConditionNode.IsAnd, new GUIContent("&&"), EditorStyles.miniButtonLeft);
        levelCompletionConditionNode.IsOr = GUILayout.Toggle(levelCompletionConditionNode.IsOr, new GUIContent("||"), EditorStyles.miniButtonRight);

        if (GUILayout.Button("+&&", EditorStyles.miniButtonLeft))
        {
            if (levelCompletionConditionNode.AndNestedNodeKey == null)
            {
                levelCompletionConditionNode.AndNestedNodeKey = new List<int>();
            }
            var addedNodeIndex = myTarget.AddNode(new LevelCompletionConditionNode());
            levelCompletionConditionNode.AndNestedNodeKey.Add(addedNodeIndex);
        }
        if (GUILayout.Button("+||", EditorStyles.miniButtonRight))
        {
            if (levelCompletionConditionNode.OrNestedNodeKey == null)
            {
                levelCompletionConditionNode.OrNestedNodeKey = new List<int>();
            }
            var addedNodeIndex = myTarget.AddNode(new LevelCompletionConditionNode());
            levelCompletionConditionNode.OrNestedNodeKey.Add(addedNodeIndex);
        }

        if (GUILayout.Button("+", EditorStyles.miniButtonLeft))
        {
            var addedNodeIndex = myTarget.AddNode(new LevelCompletionConditionNode());
            levelCompletionConditionNode.SameLevelNodesKey.Add(addedNodeIndex);
        }
        if (GUILayout.Button("-", EditorStyles.miniButtonRight))
        {
            nodeToRemove = nodeIndex;
        }

        EditorGUILayout.EndHorizontal();

        if (levelCompletionConditionNode.AndNestedNodeKey != null && levelCompletionConditionNode.AndNestedNodeKey.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField("&&");
            this.RenderNode(levelCompletionConditionNode.AndNestedNodeKey[0]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        if (levelCompletionConditionNode.OrNestedNodeKey != null && levelCompletionConditionNode.OrNestedNodeKey.Count > 0)
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField("||");
            this.RenderNode(levelCompletionConditionNode.OrNestedNodeKey[0]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        if (levelCompletionConditionNode.SameLevelNodesKey == null)
        {
            levelCompletionConditionNode.SameLevelNodesKey = new List<int>();
        }

        if (levelCompletionConditionNode != null)
        {
            foreach (var sameLevelNode in levelCompletionConditionNode.SameLevelNodesKey)
            {
                this.RenderNode(sameLevelNode);
            }
        }

    }


    private void RemoveNodeFormGraph()
    {
        if (this.nodeToRemove >= LevelCompletionConditionGraph.ROOT_NODE_KEY)
        {
            var myTarget = (LevelCompletionConditionGraph)target;

            foreach (var node in myTarget.LevelCompletionGraph.Values.ToList())
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

[System.Serializable]
public class LevelCompletionConditionGraphResolutionInput
{
}
