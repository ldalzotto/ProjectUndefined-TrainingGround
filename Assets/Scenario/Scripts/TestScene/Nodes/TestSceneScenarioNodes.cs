using System.Collections.Generic;

public class IdCardGrabScenarioNode : ScenarioNode
{
    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new ScenarioAction(typeof(GrabAction), ItemID.ID_CARD, PointOfInterestId.ID_CARD), new IdCardGiveScenarioNode() }
        };
    }
}

public class IdCardGrabScenarioNodeV2 : ScenarioNode
{
    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new ScenarioAction(typeof(GrabAction), ItemID.ID_CARD_V2, PointOfInterestId.ID_CARD_V2), new IdCardGiveScenarioNode() }
        };
    }
}

public class IdCardGiveScenarioNode : ScenarioNode
{
    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new ScenarioAction(typeof(GiveAction), ItemID.ID_CARD, PointOfInterestId.BOUNCER), null }
        };
    }
}

public class IdCardGiveScenarioNodeV2 : ScenarioNode
{
    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return new Dictionary<ScenarioAction, ScenarioNode>()
        {
            {new ScenarioAction(typeof(GiveAction), ItemID.ID_CARD_V2, PointOfInterestId.BOUNCER), null }
        };
    }
}