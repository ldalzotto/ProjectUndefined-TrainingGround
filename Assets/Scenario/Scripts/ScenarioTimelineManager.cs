using System.Collections.Generic;
using UnityEngine;

//TODO -> Adding custom inspector to visualize ScenarioNodesManager
public class ScenarioTimelineManager : MonoBehaviour
{

    private ScenarioNodesManager ScenarioNodesManager;

    private void Start()
    {
        #region External Dependencies
        var ScenarioTimelineEventManager = GameObject.FindObjectOfType<ScenarioTimelineEventManager>();
        #endregion
        var scenarioInitialization = GetComponent<ScenarioInitialisation>();

        ScenarioNodesManager = new ScenarioNodesManager(ScenarioTimelineEventManager, scenarioInitialization);
    }

    #region External Events
    public void OnScenarioActionExecuted(ScenarioAction scenarioAction)
    {
        Debug.Log("Scenario graph update. ItemId : " + scenarioAction.ToString());
        ScenarioNodesManager.IncrementScenarioGraph(scenarioAction);
    }
    #endregion
}

#region Scenario Nodes Manager
class ScenarioNodesManager
{
    private ScenarioTimelineEventManager ScenarioTimelineEventManager;

    private List<ScenarioNode> scenarioNodes = new List<ScenarioNode>();

    public ScenarioNodesManager(ScenarioTimelineEventManager ScenarioTimelineEventManager, ScenarioInitialisation scenarioInitialization)
    {
        this.ScenarioTimelineEventManager = ScenarioTimelineEventManager;
        AddToNodes(scenarioInitialization.InitialScenarioNodes());
    }

    public void IncrementScenarioGraph(ScenarioAction executedScenarioAction)
    {
        var scenarioNodesIncrementation = ComputeScenarioIncrementation(executedScenarioAction);
        AddToNodes(scenarioNodesIncrementation.nextScenarioNodes);
        foreach (var oldnode in scenarioNodesIncrementation.oldScenarioNodes)
        {
            scenarioNodes.Remove(oldnode);
            this.ScenarioTimelineEventManager.OnScenarioNodeEnded(oldnode);
        }
    }

    private ScenarioNodesIncrementation ComputeScenarioIncrementation(ScenarioAction executedScenarioAction)
    {
        List<ScenarioNode> nextScenarioNodes = new List<ScenarioNode>();
        List<ScenarioNode> oldScenarioNodes = new List<ScenarioNode>();
        foreach (var scenarioNode in scenarioNodes)
        {
            var computedNodes = scenarioNode.ComputeTransitions(executedScenarioAction);
            if (computedNodes != null && computedNodes.Count > 0)
            {
                foreach (var computedNode in computedNodes)
                {
                    if (computedNode != null)
                    {
                        nextScenarioNodes.Add(computedNode);
                    }
                    oldScenarioNodes.Add(scenarioNode);
                }
            }
        }
        return new ScenarioNodesIncrementation(nextScenarioNodes, oldScenarioNodes);
    }

    private void AddToNodes(List<ScenarioNode> scenarioNodesToAdd)
    {
        scenarioNodes.AddRange(scenarioNodesToAdd);
        foreach (var newScenarioNode in scenarioNodesToAdd)
        {
            ScenarioTimelineEventManager.OnScenarioNodeStarted(newScenarioNode);
        }
    }

    private class ScenarioNodesIncrementation
    {
        public List<ScenarioNode> nextScenarioNodes;
        public List<ScenarioNode> oldScenarioNodes;

        public ScenarioNodesIncrementation(List<ScenarioNode> nextScenarioNodes, List<ScenarioNode> oldScenarioNodes)
        {
            this.nextScenarioNodes = nextScenarioNodes;
            this.oldScenarioNodes = oldScenarioNodes;
        }
    }
}
#endregion


public abstract class ScenarioNode
{
    private Dictionary<ScenarioAction, ScenarioNode> transitionRequirements;
    private Dictionary<PointOfInterestId, DiscussionTree> discussionTrees;

    public Dictionary<ScenarioAction, ScenarioNode> TransitionRequirements { get => transitionRequirements; }
    public Dictionary<PointOfInterestId, DiscussionTree> DiscussionTrees { get => discussionTrees; }

    protected abstract Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements();
    protected abstract Dictionary<PointOfInterestId, DiscussionTree> BuildDiscussionTrees();

    protected ScenarioNode()
    {
        transitionRequirements = BuildTransitionRequiremements();
        discussionTrees = BuildDiscussionTrees();
    }

    public List<ScenarioNode> ComputeTransitions(ScenarioAction executedScenarioAction)
    {
        if (transitionRequirements == null)
        {
            return null;
        }

        List<ScenarioNode> nextScenarioNodes = new List<ScenarioNode>();
        foreach (var transitionRequirement in transitionRequirements)
        {
            //transitionRequirement.Value == null means the end of a branch
            if (transitionRequirement.Key.Equals(executedScenarioAction))
            {
                nextScenarioNodes.Add(transitionRequirement.Value);
            }
        }
        return nextScenarioNodes;
    }
}

public enum ScenarioNodeLifecycle
{
    ON_START, ON_END
}