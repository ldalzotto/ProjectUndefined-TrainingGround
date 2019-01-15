using System.Collections.Generic;

public class IdCardGrabScenarioNode : ScenarioNode
{
    protected override Dictionary<PointOfInterestId, DiscussionTree> BuildDiscussionTrees()
    {
        return new Dictionary<PointOfInterestId, DiscussionTree>()
        {
            {PointOfInterestId.BOUNCER, new DiscussionTree(new DiscussionSentence(DisucssionSentenceId.BOUNCER_SENTENCE_1, PointOfInterestId.BOUNCER,
                        new DiscussionSentence(DisucssionSentenceId.PLAYER_SENTENCE_1, PointOfInterestId.PLAYER,
                            new DiscussionSentence(DisucssionSentenceId.BOUNCER_SENTENCE_2, PointOfInterestId.BOUNCER, null)
                        ))) }
        };
    }

    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD, PointOfInterestId.ID_CARD), new IdCardGiveScenarioNode() }
        };
    }
}

public class IdCardGrabScenarioNodeV2 : ScenarioNode
{
    protected override Dictionary<PointOfInterestId, DiscussionTree> BuildDiscussionTrees()
    {
        return null;
    }

    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new GrabScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2), new IdCardGiveScenarioNodeV2() }
        };
    }
}

public class IdCardGiveScenarioNode : ScenarioNode
{
    protected override Dictionary<PointOfInterestId, DiscussionTree> BuildDiscussionTrees()
    {
        return null;
    }

    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new GiveScenarioAction(ItemID.ID_CARD, PointOfInterestId.BOUNCER), null }
        };
    }
}

public class IdCardGiveScenarioNodeV2 : ScenarioNode
{
    protected override Dictionary<PointOfInterestId, DiscussionTree> BuildDiscussionTrees()
    {
        return null;
    }

    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new GiveScenarioAction(ItemID.ID_CARD_V2, PointOfInterestId.BOUNCER), null }
        };
    }
}