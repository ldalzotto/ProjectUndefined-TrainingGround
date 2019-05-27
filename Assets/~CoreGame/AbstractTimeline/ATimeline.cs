using OdinSerializer;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace CoreGame
{
    public interface ITimelineNodeManager
    {
        void Init();
        void Persist();
    }
    public abstract class ATimelineNodeManager : MonoBehaviour
    {
    }

    public abstract class TimelineNodeManager<T> : ATimelineNodeManager, ITimelineNodeManager
    {
        #region External Dependencies
        protected abstract T workflowActionPassedDataStruct { get; }
        #endregion

        #region Perisistance
        protected abstract bool isPersisted { get; }
        private ATimelinePersister<TimelineNode<T>> timelinePersister;
        #endregion

        private List<TimelineNode<T>> nodes = new List<TimelineNode<T>>();

        public List<TimelineNode<T>> Nodes { get => nodes; }

        public virtual void Init()
        {
            if (this.isPersisted)
            {
                this.timelinePersister = new ATimelinePersister<TimelineNode<T>>(this.GetType());
                var loadedNodes = this.timelinePersister.Load();

                if (loadedNodes == null)
                {
                    InitFromConfig();
                    this.timelinePersister.Save(this.nodes);
                }
                else
                {
                    this.nodes = loadedNodes;
                }
            }
            else
            {
                InitFromConfig();
            }
        }

        private void InitFromConfig()
        {
            var TimelineInitilizer = GetComponent<TimelineInitializer<T>>();
            if (!TimelineInitilizer.HasBeenInit)
            {
                AddToNodes(TimelineInitilizer.InitialNodes);
                TimelineInitilizer.HasBeenInit = true;
            }
        }

        public void IncrementGraph(TimeLineAction executedTimelineAction)
        {
            var scenarioNodesIncrementation = ComputeScenarioIncrementation(executedTimelineAction);
            AddToNodes(scenarioNodesIncrementation.nexNodes);
            foreach (var oldnode in scenarioNodesIncrementation.oldNodes)
            {
                nodes.Remove(oldnode);
                foreach (var endAction in oldnode.OnExitNodeAction)
                {
                    endAction.Execute(workflowActionPassedDataStruct, oldnode);
                }
            }
            this.Persist();
        }

        public void Persist()
        {
            if (this.timelinePersister != null)
            {
                this.timelinePersister.Save(this.nodes);
            }
        }

        private void AddToNodes(List<TimelineNode<T>> nodesToAdd)
        {
            nodes.AddRange(nodesToAdd);
            foreach (var newNode in nodesToAdd)
            {
                foreach (var startAction in newNode.OnStartNodeAction)
                {
                    startAction.Execute(workflowActionPassedDataStruct, newNode);
                }
            }
        }

        private NodesIncrementation ComputeScenarioIncrementation(TimeLineAction executedTimelineAction)
        {
            List<TimelineNode<T>> nextTimelineNodes = new List<TimelineNode<T>>();
            List<TimelineNode<T>> oldTimelineNodes = new List<TimelineNode<T>>();
            foreach (var node in nodes)
            {
                var computedNodes = node.ComputeTransitions(executedTimelineAction);
                if (computedNodes != null && computedNodes.Count > 0)
                {
                    foreach (var computedNode in computedNodes)
                    {
                        if (computedNode != null)
                        {
                            nextTimelineNodes.Add(computedNode);
                        }
                        oldTimelineNodes.Add(node);
                    }
                }
            }
            return new NodesIncrementation(nextTimelineNodes, oldTimelineNodes);
        }

        private class NodesIncrementation
        {
            public List<TimelineNode<T>> nexNodes;
            public List<TimelineNode<T>> oldNodes;

            public NodesIncrementation(List<TimelineNode<T>> nextNodes, List<TimelineNode<T>> oldNodes)
            {
                this.nexNodes = nextNodes;
                this.oldNodes = oldNodes;
            }
        }
    }

    public abstract class TimelineNodeManagerV2<T, NODE_KEY> : ATimelineNodeManager, ITimelineNodeManager
    {
        #region External Dependencies
        protected abstract T workflowActionPassedDataStruct { get; }
        #endregion

        public TimelineInitializerScriptableObject AbstractTimelineInitializer;
        private TimelineInitializerV2<T, NODE_KEY> TimelineInitializer;

        #region Perisistance
        protected abstract bool isPersisted { get; }
        private ATimelinePersister<NODE_KEY> timelinePersister;
        #endregion

        private List<NODE_KEY> nodes = new List<NODE_KEY>();

        public List<NODE_KEY> Nodes { get => nodes; }

        public virtual void Init()
        {
            this.TimelineInitializer = (TimelineInitializerV2<T, NODE_KEY>)AbstractTimelineInitializer;
            if (this.isPersisted)
            {
                this.timelinePersister = new ATimelinePersister<NODE_KEY>(this.GetType());
                var loadedNodes = this.timelinePersister.Load();

                if (loadedNodes == null)
                {
                    InitFromConfig();
                    this.timelinePersister.Save(this.nodes);
                }
                else
                {
                    this.nodes = loadedNodes;
                }
            }
            else
            {
                InitFromConfig();
            }
        }

        private void InitFromConfig()
        {
            AddToNodes(TimelineInitializer.InitialNodes);
        }

        public void IncrementGraph(TimeLineAction executedTimelineAction)
        {
            var scenarioNodesIncrementation = ComputeScenarioIncrementation(executedTimelineAction);
            AddToNodes(scenarioNodesIncrementation.nexNodes);
            foreach (var oldnodeKey in scenarioNodesIncrementation.oldNodes)
            {
                var oldNode = this.TimelineInitializer.GetNode(oldnodeKey);
                nodes.Remove(oldnodeKey);
                foreach (var endAction in oldNode.OnExitNodeAction)
                {
                    endAction.Execute(workflowActionPassedDataStruct, oldNode);
                }
            }
            this.Persist();
        }

        public void Persist()
        {
            if (this.timelinePersister != null)
            {
                this.timelinePersister.Save(this.nodes);
            }
        }

        private void AddToNodes(List<NODE_KEY> nodesToAdd)
        {
            nodes.AddRange(nodesToAdd);
            foreach (var newNodeId in nodesToAdd)
            {
                var newNode = this.TimelineInitializer.GetNode(newNodeId);
                foreach (var startAction in newNode.OnStartNodeAction)
                {
                    startAction.Execute(workflowActionPassedDataStruct, newNode);
                }
            }
        }

        private NodesIncrementation ComputeScenarioIncrementation(TimeLineAction executedTimelineAction)
        {
            List<NODE_KEY> nextTimelineNodes = new List<NODE_KEY>();
            List<NODE_KEY> oldTimelineNodes = new List<NODE_KEY>();
            foreach (var nodeId in nodes)
            {
                var node = this.TimelineInitializer.GetNode(nodeId);
                var computedNodes = node.ComputeTransitions(executedTimelineAction);
                if (computedNodes != null && computedNodes.Count > 0)
                {
                    foreach (var computedNode in computedNodes)
                    {
                        if (computedNode != null)
                        {
                            nextTimelineNodes.Add(computedNode);
                        }
                        oldTimelineNodes.Add(nodeId);
                    }
                }
            }
            return new NodesIncrementation(nextTimelineNodes, oldTimelineNodes);
        }

        private class NodesIncrementation
        {
            public List<NODE_KEY> nexNodes;
            public List<NODE_KEY> oldNodes;

            public NodesIncrementation(List<NODE_KEY> nexNodes, List<NODE_KEY> oldNodes)
            {
                this.nexNodes = nexNodes;
                this.oldNodes = oldNodes;
            }
        }
    }
    public abstract class TimelineInitializer<T> : MonoBehaviour
    {
        private bool hasBeenInit;
        public bool HasBeenInit { get => hasBeenInit; set => hasBeenInit = value; }
        public abstract List<TimelineNode<T>> InitialNodes { get; }
        public abstract Enum TimelineId { get; }
    }

    [System.Serializable]
    public abstract class TimelineInitializerV2<T, NODE_KEY> : TimelineInitializerScriptableObject
    {
        [SerializeField]
        public Dictionary<NODE_KEY, TimelineNodeV2<T, NODE_KEY>> Nodes;
        [SerializeField]
        public List<NODE_KEY> InitialNodes;

        public TimelineNodeV2<T, NODE_KEY> GetNode(NODE_KEY key)
        {
            return Nodes[key];
        }
    }

    [System.Serializable]
    public abstract class TimelineInitializerScriptableObject : SerializedScriptableObject { }

    [System.Serializable]
    public abstract class TimelineNode<T>
    {
        [SerializeField]
        public abstract Dictionary<TimeLineAction, TimelineNode<T>> TransitionRequirements { get; }

        [SerializeField]
        public abstract List<TimelineNodeWorkflowAction<T>> OnStartNodeAction { get; }

        [SerializeField]
        public abstract List<TimelineNodeWorkflowAction<T>> OnExitNodeAction { get; }

        public List<TimelineNode<T>> ComputeTransitions(TimeLineAction executedTimelineAction)
        {
            if (TransitionRequirements == null)
            {
                return null;
            }

            List<TimelineNode<T>> nextNodes = new List<TimelineNode<T>>();
            foreach (var transitionRequirement in TransitionRequirements)
            {
                //transitionRequirement.Value == null means the end of a branch
                if (transitionRequirement.Key.Equals(executedTimelineAction))
                {
                    nextNodes.Add(transitionRequirement.Value);
                }
            }
            return nextNodes;
        }

    }

    [System.Serializable]
    public abstract class TimelineNodeV2<T, NODE_KEY>
    {
        public Dictionary<TimeLineAction, List<NODE_KEY>> TransitionRequirements;

        public List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> OnStartNodeAction;

        public List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> OnExitNodeAction;

        protected TimelineNodeV2(Dictionary<TimeLineAction, List<NODE_KEY>> transitionRequirements, List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> onStartNodeAction, List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> onExitNodeAction)
        {
            TransitionRequirements = transitionRequirements;
            OnStartNodeAction = onStartNodeAction;
            OnExitNodeAction = onExitNodeAction;
        }

        public List<NODE_KEY> ComputeTransitions(TimeLineAction executedTimelineAction)
        {
            if (TransitionRequirements == null)
            {
                return null;
            }

            List<NODE_KEY> nextNodes = new List<NODE_KEY>();
            foreach (var transitionRequirement in TransitionRequirements)
            {
                //transitionRequirement.Value == null means the end of a branch
                if (transitionRequirement.Key.Equals(executedTimelineAction))
                {
                    nextNodes.AddRange(transitionRequirement.Value);
                }
            }
            return nextNodes;
        }

    }

    [System.Serializable]
    public abstract class TimelineNodeWorkflowAction<T>
    {
        public abstract void Execute(T workflowActionPassedDataStruct, TimelineNode<T> timelineNodeRefence);
    }

    [System.Serializable]
    public abstract class TimelineNodeWorkflowActionV2<T, NODE_KEY>
    {
        public abstract void Execute(T workflowActionPassedDataStruct, TimelineNodeV2<T, NODE_KEY> timelineNodeRefence);
    }
}