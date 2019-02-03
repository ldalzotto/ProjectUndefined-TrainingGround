using System;
using System.Collections.Generic;
using timeline.serialized;
using UnityEngine;

public abstract class ATimelineNodeManager : MonoBehaviour
{
    #region External Dependencies
    private PointOfInterestManager PointOfInterestManager;
    #endregion

    private List<TimelineNode> nodes = new List<TimelineNode>();

    public List<TimelineNode> Nodes { get => nodes; }

    public void Init()
    {
        PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
        var TimelineInitilizer = GetComponent<TimelineInitializer>();
        if (!TimelineInitilizer.HasBeenInit)
        {
            AddToNodes(TimelineInitilizer.InitialNodes);
            TimelineInitilizer.HasBeenInit = true;
        }

    }

    public void IncrementGraph(ScenarioAction executedScenarioAction)
    {
        var scenarioNodesIncrementation = ComputeScenarioIncrementation(executedScenarioAction);
        AddToNodes(scenarioNodesIncrementation.nexNodes);
        foreach (var oldnode in scenarioNodesIncrementation.oldNodes)
        {
            nodes.Remove(oldnode);
            foreach (var endAction in oldnode.OnExitNodeAction)
            {
                endAction.Execute(PointOfInterestManager, oldnode);
            }
        }
    }

    private void AddToNodes(List<TimelineNode> nodesToAdd)
    {
        nodes.AddRange(nodesToAdd);
        foreach (var newNode in nodesToAdd)
        {
            foreach (var startAction in newNode.OnStartNodeAction)
            {
                startAction.Execute(PointOfInterestManager, newNode);
            }
        }
    }

    private NodesIncrementation ComputeScenarioIncrementation(ScenarioAction executedScenarioAction)
    {
        List<TimelineNode> nextScenarioNodes = new List<TimelineNode>();
        List<TimelineNode> oldScenarioNodes = new List<TimelineNode>();
        foreach (var node in nodes)
        {
            var computedNodes = node.ComputeTransitions(executedScenarioAction);
            if (computedNodes != null && computedNodes.Count > 0)
            {
                foreach (var computedNode in computedNodes)
                {
                    if (computedNode != null)
                    {
                        nextScenarioNodes.Add(computedNode);
                    }
                    oldScenarioNodes.Add(node);
                }
            }
        }
        return new NodesIncrementation(nextScenarioNodes, oldScenarioNodes);
    }

    private class NodesIncrementation
    {
        public List<TimelineNode> nexNodes;
        public List<TimelineNode> oldNodes;

        public NodesIncrementation(List<TimelineNode> nextNodes, List<TimelineNode> oldNodes)
        {
            this.nexNodes = nextNodes;
            this.oldNodes = oldNodes;
        }
    }
}

public abstract class TimelineInitializer : MonoBehaviour
{
    private bool hasBeenInit;
    public bool HasBeenInit { get => hasBeenInit; set => hasBeenInit = value; }
    public abstract List<TimelineNode> InitialNodes { get; }
    public abstract Enum TimelineId { get; }
}

public abstract class TimelineNode
{
    public abstract Dictionary<ScenarioAction, TimelineNode> TransitionRequirements { get; }
    public abstract List<TimelineNodeWorkflowAction> OnStartNodeAction { get; }
    public abstract List<TimelineNodeWorkflowAction> OnExitNodeAction { get; }

    public List<TimelineNode> ComputeTransitions(ScenarioAction executedScenarioAction)
    {
        if (TransitionRequirements == null)
        {
            return null;
        }

        List<TimelineNode> nextNodes = new List<TimelineNode>();
        foreach (var transitionRequirement in TransitionRequirements)
        {
            //transitionRequirement.Value == null means the end of a branch
            if (transitionRequirement.Key.Equals(executedScenarioAction))
            {
                nextNodes.Add(transitionRequirement.Value);
            }
        }
        return nextNodes;
    }

    public TimelineNodeSerialized Map2Serialized()
    {
        var serializedTransitions = new List<TimelineNodeTransitionSerialized>();
        if (TransitionRequirements != null)
        {
            foreach (var transition in TransitionRequirements)
            {
                TimelineNodeSerialized nexSerializedNode = null;
                if (transition.Value != null)
                {
                    nexSerializedNode = transition.Value.Map2Serialized();
                }
                serializedTransitions.Add(new TimelineNodeTransitionSerialized(transition.Key, nexSerializedNode, transition.Key.GetType().Name));
            }
        }

        return new TimelineNodeSerialized(serializedTransitions, GetType().Name);
    }
}

public abstract class TimelineNodeWorkflowAction
{
    public abstract void Execute(PointOfInterestManager PointOfInterestManager, TimelineNode timelineNodeRefence);
}