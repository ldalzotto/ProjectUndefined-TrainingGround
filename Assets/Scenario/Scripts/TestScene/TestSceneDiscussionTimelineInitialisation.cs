using System.Collections.Generic;

public class TestSceneDiscussionTimelineInitialisation : TimelineInitilizer
{
    protected override List<TimelineNode> BuildInitialDiscussionTimelineNodes()
    {
        return new List<TimelineNode>()
        {
            new BouncerKODiscussionNode()
        };
    }
}
