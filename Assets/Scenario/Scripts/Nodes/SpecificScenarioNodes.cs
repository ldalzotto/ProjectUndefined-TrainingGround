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


public class IdCardGiveScenarioNode : ScenarioNode
{
    protected override Dictionary<ScenarioAction, ScenarioNode> BuildTransitionRequiremements()
    {
        return null;
    }
}