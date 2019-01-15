﻿using UnityEngine;

public class DiscussionEventHandler : MonoBehaviour
{

    private DiscussionWindowManager DiscussionWindowManager;

    public delegate void DiscussionWindowSleepExternalHandler();
    private event DiscussionWindowSleepExternalHandler OnDiscussionWindowSleepExternal;

    private void Start()
    {
        DiscussionWindowManager = GameObject.FindObjectOfType<DiscussionWindowManager>();
    }

    public void OnDiscussionWindowAwake(Vector3 discussionUIPosition, string textToWrite)
    {
        DiscussionWindowManager.OnDiscussionWindowAwake(discussionUIPosition, textToWrite);
    }
    public void OnDiscussionWindowSleep()
    {
        DiscussionWindowManager.OnDiscussionWindowSleep();
        OnDiscussionWindowSleepExternal.Invoke();
    }

    public void AddOnSleepExternalHanlder(DiscussionWindowSleepExternalHandler DiscussionWindowSleepExternalHandler)
    {
        OnDiscussionWindowSleepExternal += DiscussionWindowSleepExternalHandler;
    }
    public void RemoveOnSleepExternalHanlder(DiscussionWindowSleepExternalHandler DiscussionWindowSleepExternalHandler)
    {
        OnDiscussionWindowSleepExternal -= DiscussionWindowSleepExternalHandler;
    }
}
