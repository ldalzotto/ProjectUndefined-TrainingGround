using UnityEngine;

public class DiscussionEventHandler : MonoBehaviour
{

    private DiscussionWindowManager DiscussionWindowManager;
    private ScenarioTimelineManager ScenarioTimelineManager;

    public delegate void DiscussionWindowSleepExternalHandler();
    private event DiscussionWindowSleepExternalHandler OnDiscussionWindowSleepExternal;

    public delegate DiscussionChoiceNode DiscussionTextNodeHandler();
    private event DiscussionTextNodeHandler OnDiscussionTextNodeEndEvent;

    private void Start()
    {
        DiscussionWindowManager = GameObject.FindObjectOfType<DiscussionWindowManager>();
        ScenarioTimelineManager = GameObject.FindObjectOfType<ScenarioTimelineManager>();
    }

    #region Discussion Window Events
    public void OnDiscussionWindowAwake(DiscussionTextOnlyNode discussionNode, Transform position)
    {
        DiscussionWindowManager.OnDiscussionWindowAwake(discussionNode, position);
    }

    public void OnDiscussionTextNodeEnd()
    {
        var nextDisucssionChoiceNode = OnDiscussionTextNodeEndEvent.Invoke();
        if (nextDisucssionChoiceNode != null)
        {
            DiscussionWindowManager.OnChoicePopupAwake(nextDisucssionChoiceNode);
        }
        else
        {
            DiscussionWindowManager.OnDiscussionEnd();
        }
    }

    public void OnDiscussionWindowSleep()
    {
        DiscussionWindowManager.OnDiscussionWindowSleep();
        OnDiscussionWindowSleepExternal.Invoke();
    }
    #endregion

    #region Discusion Choices Event
    public void OnDiscussionChoiceMade(DiscussionChoiceTextId selectedChoice)
    {
        ScenarioTimelineManager.OnScenarioActionExecuted(new DiscussionChoiceScenarioAction(selectedChoice));
    }
    #endregion

    public void AddOnSleepExternalHanlder(DiscussionWindowSleepExternalHandler DiscussionWindowSleepExternalHandler)
    {
        OnDiscussionWindowSleepExternal += DiscussionWindowSleepExternalHandler;
    }
    public void RemoveOnSleepExternalHanlder(DiscussionWindowSleepExternalHandler DiscussionWindowSleepExternalHandler)
    {
        OnDiscussionWindowSleepExternal -= DiscussionWindowSleepExternalHandler;
    }

    public void AddOnDiscussionTextNodeEndEventHandler(DiscussionTextNodeHandler DiscussionTextNodeHandler)
    {
        OnDiscussionTextNodeEndEvent += DiscussionTextNodeHandler;
    }
    public void RemoveOnDiscussionTextNodeEndEventHandler(DiscussionTextNodeHandler DiscussionTextNodeHandler)
    {
        OnDiscussionTextNodeEndEvent -= DiscussionTextNodeHandler;
    }

}
