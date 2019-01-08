﻿using System;
using System.Collections;
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
        UpdateScenarioGraph(scenarioAction);
    }
    #endregion

    private void UpdateScenarioGraph(ScenarioAction executedScenarioAction)
    {
        Debug.Log("Scenario graph update. ItemId : " + executedScenarioAction.ItemInvolved + " POIId : " + executedScenarioAction.PoiInvolved + " Action : " + executedScenarioAction.ActionType.ToString());
        StartCoroutine(ScenarioNodesManager.IncrementScenarioGraph(executedScenarioAction));
    }

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

    public IEnumerator IncrementScenarioGraph(ScenarioAction executedScenarioAction)
    {
        var scenarioNodesIncrementation = ComputeScenarioIncrementation(executedScenarioAction);
        AddToNodes(scenarioNodesIncrementation.nextScenarioNodes);
        yield return new WaitForEndOfFrame();
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

public class ScenarioAction
{
    private Type actionType;
    private ItemID itemInvolved;
    private PointOfInterestId poiInvolved;

    public ScenarioAction(Type actionType, ItemID itemInvolved, PointOfInterestId poiInvolved)
    {
        this.actionType = actionType;
        this.itemInvolved = itemInvolved;
        this.poiInvolved = poiInvolved;
    }

    public ItemID ItemInvolved { get => itemInvolved; }
    public PointOfInterestId PoiInvolved { get => poiInvolved; }
    public Type ActionType { get => actionType; }

    public override bool Equals(object obj)
    {
        var action = obj as ScenarioAction;
        return action != null &&
               EqualityComparer<Type>.Default.Equals(actionType, action.actionType) &&
               itemInvolved == action.itemInvolved &&
               poiInvolved == action.poiInvolved;
    }

    public override int GetHashCode()
    {
        var hashCode = 272091116;
        hashCode = hashCode * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(actionType);
        hashCode = hashCode * -1521134295 + itemInvolved.GetHashCode();
        hashCode = hashCode * -1521134295 + poiInvolved.GetHashCode();
        return hashCode;
    }
}

public abstract class ScenarioNode
{
    private Dictionary<ScenarioAction, ScenarioNode> transitionRequirements;

    public Dictionary<ScenarioAction, ScenarioNode> TransitionRequirements { get => transitionRequirements; }

    protected abstract Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements();
    protected ScenarioNode()
    {
        transitionRequirements = BuildTransitionRequiremements();
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