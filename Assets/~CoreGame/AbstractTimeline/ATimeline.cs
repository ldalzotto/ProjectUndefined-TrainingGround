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

    public abstract class TimelineInitializer<T> : MonoBehaviour
    {
        private bool hasBeenInit;
        public bool HasBeenInit { get => hasBeenInit; set => hasBeenInit = value; }
        public abstract List<TimelineNode<T>> InitialNodes { get; }
        public abstract Enum TimelineId { get; }
    }

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
    public abstract class TimelineNodeWorkflowAction<T>
    {
        public abstract void Execute(T workflowActionPassedDataStruct, TimelineNode<T> timelineNodeRefence);
    }
}