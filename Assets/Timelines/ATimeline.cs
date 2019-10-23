using System;
using System.Collections.Generic;
using OdinSerializer;
using Persistence;
using UnityEngine;

namespace Timelines
{
    public interface ITimelineNodeManager
    {
        void Init(TimelineInitializerScriptableObject providedTimelineInitializer = null);
        void IncrementGraph(TimeLineAction executedTimelineAction);
    }

    public abstract class TimelineNodeManagerV2<NODE_KEY> : ITimelineNodeManager
    {
        #region External Dependencies

        private TimelineInitializerV2<NODE_KEY> TimelineInitializer;

        #endregion

        #region Timeline ID

        protected abstract TimelineID TimelineID { get; }

        #endregion

        #region Perisistance

        protected abstract bool isPersisted { get; }
        private ATimelinePersister<ATimelinePersistedNodes<NODE_KEY>> timelinePersister;

        #endregion

        private ATimelinePersistedNodes<NODE_KEY> persistedNodes = new ATimelinePersistedNodes<NODE_KEY>() {Nodes = new List<NODE_KEY>()};

        public List<NODE_KEY> Nodes
        {
            get => persistedNodes.Nodes;
        }

        public virtual void Init(TimelineInitializerScriptableObject providedTimelineInitializer = null)
        {
            if (providedTimelineInitializer == null)
            {
                #region External Dependencies

                this.TimelineInitializer = (TimelineInitializerV2<NODE_KEY>) TimelineConfigurationGameObject.Get().TimelineConfiguration.ConfigurationInherentData[TimelineID];

                #endregion
            }
            else
            {
                this.TimelineInitializer = (TimelineInitializerV2<NODE_KEY>) providedTimelineInitializer;
            }


            if (this.isPersisted)
            {
                this.timelinePersister = new ATimelinePersister<ATimelinePersistedNodes<NODE_KEY>>(this.GetType());
                if (this.persistedNodes.Nodes.Count == 0)
                {
                    var loadedNodes = this.timelinePersister.Load();

                    if (loadedNodes.Nodes == null)
                    {
                        InitFromConfig();
                        this.PersistAsync();
                    }
                    else
                    {
                        this.persistedNodes = loadedNodes;
                    }
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

            //(0) -> Execution of exit first
            foreach (var oldnodeKey in scenarioNodesIncrementation.oldNodes)
            {
                var oldNode = this.TimelineInitializer.GetNode(oldnodeKey);
                persistedNodes.Nodes.Remove(oldnodeKey);
                foreach (var endAction in oldNode.OnExitNodeAction)
                {
                    endAction.Execute(oldNode);
                }
            }

            AddToNodes(scenarioNodesIncrementation.nexNodes);

            if (scenarioNodesIncrementation != null && (scenarioNodesIncrementation.nexNodes.Count > 0 || scenarioNodesIncrementation.oldNodes.Count > 0))
            {
                this.PersistAsync();
            }
        }

        private void PersistAsync()
        {
            if (this.timelinePersister != null)
            {
                this.timelinePersister.SaveAsync(this.persistedNodes);
            }
        }

        private void AddToNodes(List<NODE_KEY> nodesToAdd)
        {
            foreach (var nodeToAdd in nodesToAdd)
            {
                if (!this.persistedNodes.Nodes.Contains(nodeToAdd))
                {
                    persistedNodes.Nodes.Add(nodeToAdd);
                    var newNode = this.TimelineInitializer.GetNode(nodeToAdd);
                    foreach (var startAction in newNode.OnStartNodeAction)
                    {
                        startAction.Execute(newNode);
                    }
                }
            }
        }

        private NodesIncrementation ComputeScenarioIncrementation(TimeLineAction executedTimelineAction)
        {
            List<NODE_KEY> nextTimelineNodes = new List<NODE_KEY>();
            List<NODE_KEY> oldTimelineNodes = new List<NODE_KEY>();
            foreach (var nodeId in persistedNodes.Nodes)
            {
                var node = this.TimelineInitializer.GetNode(nodeId);
                var computedNodes = node.ComputeTransitions(executedTimelineAction);
                if (computedNodes != null && computedNodes.Count > 0)
                {
                    foreach (var computedNode in computedNodes)
                    {
                        if (computedNode != null)
                        {
                            if (!nextTimelineNodes.Contains(computedNode))
                            {
                                nextTimelineNodes.Add(computedNode);
                            }
                        }

                        if (!oldTimelineNodes.Contains(nodeId))
                        {
                            oldTimelineNodes.Add(nodeId);
                        }
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

    [Serializable]
    public abstract class TimelineInitializerV2<NODE_KEY> : TimelineInitializerScriptableObject
    {
        [SerializeField] public Dictionary<NODE_KEY, TimelineNodeV2<NODE_KEY>> Nodes;
        [SerializeField] public List<NODE_KEY> InitialNodes;

        public TimelineNodeV2<NODE_KEY> GetNode(NODE_KEY key)
        {
            return Nodes[key];
        }
    }

    [Serializable]
    public abstract class TimelineInitializerScriptableObject : SerializedScriptableObject
    {
    }

    [Serializable]
    public class TimelineNodeV2<NODE_KEY>
    {
        public Dictionary<TimeLineAction, List<NODE_KEY>> TransitionRequirements;

        public List<TimelineNodeWorkflowActionV2<NODE_KEY>> OnStartNodeAction;

        public List<TimelineNodeWorkflowActionV2<NODE_KEY>> OnExitNodeAction;

        public TimelineNodeV2(Dictionary<TimeLineAction, List<NODE_KEY>> transitionRequirements, List<TimelineNodeWorkflowActionV2<NODE_KEY>> onStartNodeAction, List<TimelineNodeWorkflowActionV2<NODE_KEY>> onExitNodeAction)
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

    public interface TimelineNodeWorkflowActionV2Drawable
    {
#if UNITY_EDITOR
        void ActionGUI();
#endif
    }

    [Serializable]
    public abstract class TimelineNodeWorkflowActionV2<NODE_KEY> : TimelineNodeWorkflowActionV2Drawable
    {
        public abstract void Execute(TimelineNodeV2<NODE_KEY> timelineNodeRefence);

#if UNITY_EDITOR
        public virtual void ActionGUI()
        {
        }
#endif
    }
}