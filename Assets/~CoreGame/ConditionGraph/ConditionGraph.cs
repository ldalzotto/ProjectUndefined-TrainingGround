using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OdinSerializer;
using System.Linq;

namespace CoreGame
{
    public interface IConditionGraph
    {
        ConditionNode NodeProvider();
    }

    [System.Serializable]
    public abstract class ConditionGraph : SerializedScriptableObject, IConditionGraph
    {
        public const int ROOT_NODE_KEY = -1;

        [SerializeField]
        private ConditionNode conditionRootNode;

        [SerializeField]
        private Dictionary<int, ConditionNode> conditionGraph = new Dictionary<int, ConditionNode>();

        public ConditionNode ConditionRootNode { get => conditionRootNode; set => conditionRootNode = value; }

        public bool ResolveGraph(ref ConditionGraphResolutionInput conditionGraphResolutionInput)
        {
            return this.conditionRootNode.ResolveNode(ref conditionGraphResolutionInput, this);
        }

        public int SetRootNode(ConditionNode conditionNode)
        {
            this.conditionGraph[ROOT_NODE_KEY] = conditionNode;
            this.conditionRootNode = conditionNode;
            return ROOT_NODE_KEY;
        }

        public int AddNode(ConditionNode conditionNode)
        {
            int key = 0;
            if (this.conditionGraph.Keys.Count > 0)
            {
                key = (this.conditionGraph.Keys.ToList().Max() + 1);
            }
            this.conditionGraph[key] = conditionNode;
            return key;
        }

        public void RemoveNode(int nodeIndex)
        {
            this.conditionGraph.Remove(nodeIndex);
        }

        public ConditionNode GetNode(int index)
        {
            return this.conditionGraph[index];
        }

        public List<ConditionNode> GetAllNodes()
        {
            return this.conditionGraph.Values.ToList();
        }

        public bool NodeExists(int index)
        {
            return this.conditionGraph.ContainsKey(index);
        }

        public abstract ConditionNode NodeProvider();

    }

    [System.Serializable]
    public abstract class ConditionNode
    {
        public bool IsAnd;
        public bool IsOr;

        public bool NestedStart;
        public bool NestedEnd;

        public List<int> SameLevelNodesKey;

        public List<int> AndNestedNodeKey;
        public List<int> OrNestedNodeKey;

        public ConditionNode()
        {
        }

        protected abstract bool ConditionEvaluation(ref ConditionGraphResolutionInput ConditionGraphResolutionInput);
#if UNITY_EDITOR
        public abstract void SpecificEditorRender();
#endif

        public bool ResolveNode(ref ConditionGraphResolutionInput ConditionGraphResolutionInput, ConditionGraph conditionGraph)
        {
            bool returnValue = this.ConditionEvaluation(ref ConditionGraphResolutionInput);

            if (this.SameLevelNodesKey != null)
            {
                foreach (var sameLevelNodeKey in this.SameLevelNodesKey)
                {
                    var involvedNode = conditionGraph.GetNode(sameLevelNodeKey);
                    if (involvedNode.IsAnd)
                    {
                        returnValue = returnValue && involvedNode.ResolveNode(ref ConditionGraphResolutionInput, conditionGraph);
                    }
                    else if (involvedNode.IsOr)
                    {
                        returnValue = returnValue || involvedNode.ResolveNode(ref ConditionGraphResolutionInput, conditionGraph);
                    }
                }
            }

            if (AndNestedNodeKey != null && AndNestedNodeKey.Count > 0)
            {
                var involvedNode = conditionGraph.GetNode(AndNestedNodeKey[0]);
                returnValue = returnValue && involvedNode.ResolveNode(ref ConditionGraphResolutionInput, conditionGraph);
            }
            if (OrNestedNodeKey != null && OrNestedNodeKey.Count > 0)
            {
                var involvedNode = conditionGraph.GetNode(OrNestedNodeKey[0]);
                returnValue = returnValue || involvedNode.ResolveNode(ref ConditionGraphResolutionInput, conditionGraph);
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
    public interface ConditionGraphResolutionInput
    {
    }
}