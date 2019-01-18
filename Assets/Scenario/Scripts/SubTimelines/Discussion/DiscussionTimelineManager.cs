using System.Collections.Generic;
using UnityEngine;

public class DiscussionTimelineManager : MonoBehaviour
{

    public void Init()
    {
        var discussionTimelineEventManager = GameObject.FindObjectOfType<DiscussionTimelineEventManager>();
        DiscussionTimelineNodeManager = new DiscussionTimelineNodeManager(discussionTimelineEventManager);
        var DiscussionTimelineInitilizer = GetComponent<DiscussionTimelineInitilizer>();
        foreach (var initialNode in DiscussionTimelineInitilizer.InitialNodes)
        {
            DiscussionTimelineNodeManager.AddNode(initialNode);
        }
    }

    private DiscussionTimelineNodeManager DiscussionTimelineNodeManager;

    #region External Events
    public void OnScenarioActionExecuted(ScenarioAction scenarioAction)
    {
        if (scenarioAction != null)
        {
            Debug.Log("Discussion Timeline graph update. Action : " + scenarioAction.ToString());
            DiscussionTimelineNodeManager.IncrementDiscussionTimeline(scenarioAction);
        }
        else
        {
            Debug.Log("Discussion Timeline graph not updated.");
        }

    }

    public void OnDiscussionTimelineNodeStarted(DiscussionTimelineNode DiscussionTimelineNode, PointOfInterestManager PointOfInterestManager)
    {
        foreach (var startAction in DiscussionTimelineNode.OnStartDiscussionTreeAction)
        {
            startAction.Execute(PointOfInterestManager);
        }
    }
    #endregion
}

public abstract class DiscussionTimelineInitilizer : MonoBehaviour
{
    private List<DiscussionTimelineNode> initialNodes;

    protected DiscussionTimelineInitilizer()
    {
        this.initialNodes = BuildInitialDiscussionTimelineNodes();
    }

    public List<DiscussionTimelineNode> InitialNodes { get => initialNodes; }

    protected abstract List<DiscussionTimelineNode> BuildInitialDiscussionTimelineNodes();
}

public class DiscussionTimelineNodeManager
{

    private DiscussionTimelineEventManager DiscussionTimelineEventManager;

    public DiscussionTimelineNodeManager(DiscussionTimelineEventManager discussionTimelineEventManager)
    {
        DiscussionTimelineEventManager = discussionTimelineEventManager;
    }

    private List<DiscussionTimelineNode> discussionTimelineNodes = new List<DiscussionTimelineNode>();

    public void IncrementDiscussionTimeline(ScenarioAction scenarioAction)
    {
        var discussionTimelineIncrementation = ComputeDiscussionTimelineIncrementation(scenarioAction);

        foreach (var nodeToBeDeleted in discussionTimelineIncrementation.NodesThatAreDeleted)
        {
            discussionTimelineNodes.Remove(nodeToBeDeleted);
        }

        foreach (var newNode in discussionTimelineIncrementation.NewNodes)
        {
            discussionTimelineNodes.Add(newNode);
            DiscussionTimelineEventManager.OnDiscussionTimelineNodeStarted(newNode);
        }
    }

    private DiscussionTimelineIncrementation ComputeDiscussionTimelineIncrementation(ScenarioAction scenarioActionExecuted)
    {
        var nodesThatAreDeleted = new List<DiscussionTimelineNode>();
        var newNodes = new List<DiscussionTimelineNode>();

        foreach (var discussionTimelineNode in discussionTimelineNodes)
        {
            if (discussionTimelineNode.TransitionRequirements != null)
            {
                bool nodeToBeDeleted = false;
                foreach (var transition in discussionTimelineNode.TransitionRequirements)
                {
                    if (transition.Key.Equals(scenarioActionExecuted))
                    {
                        nodeToBeDeleted = true;
                        newNodes.Add(transition.Value);
                    }
                }
                if (nodeToBeDeleted)
                {
                    nodesThatAreDeleted.Add(discussionTimelineNode);
                }
            }
        }
        return new DiscussionTimelineIncrementation(nodesThatAreDeleted, newNodes);
    }

    public void AddNode(DiscussionTimelineNode DiscussionTimelineNode)
    {
        DiscussionTimelineEventManager.OnDiscussionTimelineNodeStarted(DiscussionTimelineNode);
        discussionTimelineNodes.Add(DiscussionTimelineNode);
    }

    class DiscussionTimelineIncrementation
    {
        private List<DiscussionTimelineNode> nodesThatAreDeleted;
        private List<DiscussionTimelineNode> newNodes;

        public DiscussionTimelineIncrementation(List<DiscussionTimelineNode> nodesThatAreDeleted, List<DiscussionTimelineNode> newNodes)
        {
            this.nodesThatAreDeleted = nodesThatAreDeleted;
            this.newNodes = newNodes;
        }

        public List<DiscussionTimelineNode> NodesThatAreDeleted { get => nodesThatAreDeleted; }
        public List<DiscussionTimelineNode> NewNodes { get => newNodes; }
    }
}

public abstract class DiscussionTimelineNode
{
    private Dictionary<ScenarioAction, DiscussionTimelineNode> transitionRequirements;
    private List<DiscussionTimelineModifierAction> onStartDiscussionTreeAction;
    private List<DiscussionTimelineModifierAction> onExitDiscussionTreeAction;

    public List<DiscussionTimelineModifierAction> OnStartDiscussionTreeAction { get => onStartDiscussionTreeAction; }
    public Dictionary<ScenarioAction, DiscussionTimelineNode> TransitionRequirements { get => transitionRequirements; }

    protected abstract Dictionary<ScenarioAction, DiscussionTimelineNode> BuildTransitionRequirements();
    protected abstract List<DiscussionTimelineModifierAction> BuildStartDiscussionTreeActions();
    protected abstract List<DiscussionTimelineModifierAction> BuildExitDiscussionTreeActions();

    protected DiscussionTimelineNode()
    {
        transitionRequirements = BuildTransitionRequirements();
        onStartDiscussionTreeAction = BuildStartDiscussionTreeActions();
        onExitDiscussionTreeAction = BuildExitDiscussionTreeActions();
    }
}


