using System.Collections.Generic;

public class TestSceneDiscussionTimelineInitialisation : DiscussionTimelineInitilizer
{
    protected override List<DiscussionTimelineNode> BuildInitialDiscussionTimelineNodes()
    {
        return new List<DiscussionTimelineNode>()
        {
            new BouncerDiscussionNode()
        };
    }
}
