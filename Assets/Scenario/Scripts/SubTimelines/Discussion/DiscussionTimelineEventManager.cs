using UnityEngine;

public class DiscussionTimelineEventManager : MonoBehaviour
{
    private DiscussionTimelineManager DiscussionTimelineManager;
    private PointOfInterestManager PointOfInterestManager;

    private void Start()
    {
        DiscussionTimelineManager = GameObject.FindObjectOfType<DiscussionTimelineManager>();
        PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
    }

    #region External Events
    public void OnDiscussionTimelineNodeStarted(DiscussionTimelineNode discussionTimelineNode)
    {
        DiscussionTimelineManager.OnDiscussionTimelineNodeStarted(discussionTimelineNode, PointOfInterestManager);
    }
    #endregion
}
