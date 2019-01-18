
using System.Collections.Generic;

public class BouncerDiscussionNode : DiscussionTimelineNode
{
    protected override List<DiscussionTimelineModifierAction> BuildExitDiscussionTreeActions()
    {
        return null;
    }

    protected override List<DiscussionTimelineModifierAction> BuildStartDiscussionTreeActions()
    {
        return new List<DiscussionTimelineModifierAction>()
        {
            new DiscussionTimelineTreeCreationAction(PointOfInterestId.BOUNCER, new DiscussionTree(DiscussionSentencesConstants.Sentenses[DiscussionSentenceId.BOUNCER_SENTENCE]) )
        };
    }

    protected override Dictionary<ScenarioAction, DiscussionTimelineNode> BuildTransitionRequirements()
    {
        return new Dictionary<ScenarioAction, DiscussionTimelineNode>()
        {
            {new DiscussionChoiceScenarioAction(DiscussionChoiceTextId.BOUNCER_CHOICE_1), new BouncerDeleteNode1() },
            {new DiscussionChoiceScenarioAction(DiscussionChoiceTextId.BOUNCER_CHOICE_2), new BouncerDeleteNode2() },
            {new DiscussionChoiceScenarioAction(DiscussionChoiceTextId.BOUNCER_CHOICE_3), new BouncerDeleteNode3() },
        };
    }
}

public class BouncerDeleteNode1 : DiscussionTimelineNode
{
    protected override List<DiscussionTimelineModifierAction> BuildExitDiscussionTreeActions()
    {
        return null;
    }

    protected override List<DiscussionTimelineModifierAction> BuildStartDiscussionTreeActions()
    {
        return new List<DiscussionTimelineModifierAction>()
        {
            new DiscussionTimelineTreeChoiceDeleteAction(PointOfInterestId.BOUNCER, DiscussionChoiceTextId.BOUNCER_CHOICE_1)
        };
    }

    protected override Dictionary<ScenarioAction, DiscussionTimelineNode> BuildTransitionRequirements()
    {
        return null;
    }
}


public class BouncerDeleteNode2 : DiscussionTimelineNode
{
    protected override List<DiscussionTimelineModifierAction> BuildExitDiscussionTreeActions()
    {
        return null;
    }

    protected override List<DiscussionTimelineModifierAction> BuildStartDiscussionTreeActions()
    {
        return new List<DiscussionTimelineModifierAction>()
        {
            new DiscussionTimelineTreeChoiceDeleteAction(PointOfInterestId.BOUNCER, DiscussionChoiceTextId.BOUNCER_CHOICE_2)
        };
    }

    protected override Dictionary<ScenarioAction, DiscussionTimelineNode> BuildTransitionRequirements()
    {
        return null;
    }
}


public class BouncerDeleteNode3 : DiscussionTimelineNode
{
    protected override List<DiscussionTimelineModifierAction> BuildExitDiscussionTreeActions()
    {
        return null;
    }

    protected override List<DiscussionTimelineModifierAction> BuildStartDiscussionTreeActions()
    {
        return new List<DiscussionTimelineModifierAction>()
        {
            new DiscussionTimelineTreeChoiceDeleteAction(PointOfInterestId.BOUNCER, DiscussionChoiceTextId.BOUNCER_CHOICE_3)
        };
    }

    protected override Dictionary<ScenarioAction, DiscussionTimelineNode> BuildTransitionRequirements()
    {
        return null;
    }
}