using UnityEngine;

public class DiscussionEventHandler : MonoBehaviour
{

    private DiscussionWindowManager DiscussionWindowManager;

    private void Start()
    {
        DiscussionWindowManager = GameObject.FindObjectOfType<DiscussionWindowManager>();
    }

    public void OnDiscussionWindowAwake(Vector3 discussionUIPosition)
    {
        DiscussionWindowManager.OnDiscussionWindowAwake(discussionUIPosition);
    }
    public void OnDiscussionWindowSleep()
    {
        DiscussionWindowManager.OnDiscussionWindowSleep();
    }
}
