using UnityEngine;

public class DiscussionEventHandler : MonoBehaviour
{

    private DiscussionWindowManager DiscussionWindowManager;

    private void Start()
    {
        DiscussionWindowManager = GameObject.FindObjectOfType<DiscussionWindowManager>();
    }

    public void OnDiscussionWindowAwake()
    {
        DiscussionWindowManager.OnDiscussionWindowAwake();
    }
    public void OnDiscussionWindowSleep()
    {
        DiscussionWindowManager.OnDiscussionWindowSleep();
    }
}
