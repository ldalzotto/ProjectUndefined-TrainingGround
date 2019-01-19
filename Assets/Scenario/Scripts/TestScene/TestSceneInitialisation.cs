using System.Collections.Generic;

public class TestSceneInitialisation : TimelineInitilizer
{
    protected override List<TimelineNode> BuildInitialDiscussionTimelineNodes()
    {
        return new List<TimelineNode>()
        {
            new IdCardGrabScenarioNode(), new IdCardGrabScenarioNodeV2()
         };
    }
}