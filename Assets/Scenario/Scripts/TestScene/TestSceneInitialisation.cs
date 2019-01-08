using System.Collections.Generic;

public class TestSceneInitialisation : ScenarioInitialisation
{
    public override List<ScenarioNode> InitialScenarioNodes()
    {
        return new List<ScenarioNode>()
        {
            new IdCardGrabScenarioNode()
        };
    }
}