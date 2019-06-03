using OdinSerializer;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace CoreGame
{
    public interface ITimelineNodeManager
    {
        void Init();
        void PersistAsync();
        void IncrementGraph(TimeLineAction executedTimelineAction);

    }
    public abstract class ATimelineNodeManager : MonoBehaviour
    {
        public abstract TimelineIDs GetTimelineID();
    }

    public abstract class TimelineNodeManagerV2<T, NODE_KEY> : ATimelineNodeManager, ITimelineNodeManager
    {
        #region External Dependencies
        protected abstract T workflowActionPassedDataStruct { get; }
        private TimelineInitializerV2<T, NODE_KEY> TimelineInitializer;
        #endregion

        #region Timeline ID
        protected abstract TimelineIDs TimelineID { get; }
        public override TimelineIDs GetTimelineID()
        {
            return this.TimelineID;
        }
        #endregion

        #region Perisistance
        protected abstract bool isPersisted { get; }
        private ATimelinePersister<NODE_KEY> timelinePersister;
        #endregion

        private List<NODE_KEY> nodes = new List<NODE_KEY>();

        public List<NODE_KEY> Nodes { get => nodes; }

        public virtual void Init()
        {
            #region External Dependencies
            this.TimelineInitializer = (TimelineInitializerV2<T, NODE_KEY>)GameObject.FindObjectOfType<CoreConfigurationManager>().TimelineConfiguration().ConfigurationInherentData[TimelineID];
            #endregion

            if (this.isPersisted)
            {
                this.timelinePersister = new ATimelinePersister<NODE_KEY>(this.GetType());
                var loadedNodes = this.timelinePersister.Load();

                if (loadedNodes == null)
                {
                    InitFromConfig();
                    this.PersistAsync();
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

            if (scenarioNodesIncrementation != null && (scenarioNodesIncrementation.nexNodes.Count > 0 || scenarioNodesIncrementation.oldNodes.Count > 0))
            {
                this.PersistAsync();
            }
        }

        public void PersistAsync()
        {
            if (this.timelinePersister != null)
            {
                this.timelinePersister.SaveAsync(this.nodes);
            }
        }

        private void AddToNodes(List<NODE_KEY> nodesToAdd)
        {
            foreach (var nodeToAdd in nodesToAdd)
            {
                if (!this.nodes.Contains(nodeToAdd))
                {
                    nodes.Add(nodeToAdd);
                    var newNode = this.TimelineInitializer.GetNode(nodeToAdd);
                    foreach (var startAction in newNode.OnStartNodeAction)
                    {
                        startAction.Execute(workflowActionPassedDataStruct, newNode);
                    }
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
    public class TimelineNodeV2<T, NODE_KEY>
    {
        public Dictionary<TimeLineAction, List<NODE_KEY>> TransitionRequirements;

        public List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> OnStartNodeAction;

        public List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> OnExitNodeAction;

        public TimelineNodeV2(Dictionary<TimeLineAction, List<NODE_KEY>> transitionRequirements, List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> onStartNodeAction, List<TimelineNodeWorkflowActionV2<T, NODE_KEY>> onExitNodeAction)
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

    [System.Serializable]
    public abstract class TimelineNodeWorkflowActionV2<T, NODE_KEY> : TimelineNodeWorkflowActionV2Drawable
    {
        public abstract void Execute(T workflowActionPassedDataStruct, TimelineNodeV2<T, NODE_KEY> timelineNodeRefence);

#if UNITY_EDITOR
        public abstract void ActionGUI();
#endif
    }
}