using System.Collections;
using UnityEngine;

public class DiscussionEventHandler : MonoBehaviour
{

    private DiscussionWindowManager DiscussionWindowManager;
    private ScenarioTimelineEventManager ScenarioTimelineEventManager;

    public delegate void DiscussionWindowSleepExternalHandler();
    private event DiscussionWindowSleepExternalHandler OnDiscussionWindowSleepExternal;

    public delegate void DiscussionTextNodeHandler();
    private event DiscussionTextNodeHandler OnDiscussionTextNodeEndEvent;

    public delegate void DiscussionChoiceNodeHandler(DiscussionChoiceTextId selectedChoice);
    private event DiscussionChoiceNodeHandler OnDiscussionChoiceNodeEndEvent;

    private void Start()
    {
        DiscussionWindowManager = GameObject.FindObjectOfType<DiscussionWindowManager>();
        ScenarioTimelineEventManager = GameObject.FindObjectOfType<ScenarioTimelineEventManager>();
    }

    #region Discussion Window Events
    public void OnDiscussionWindowAwake(DiscussionTextOnlyNode discussionNode, Transform position)
    {
        DiscussionWindowManager.OnDiscussionWindowAwake(discussionNode, position);
    }

    public IEnumerator OnDiscussionWindowSleep()
    {
        yield return StartCoroutine(DiscussionWindowManager.PlayDiscussionCloseAnimation());
        DiscussionWindowManager.OnDiscussionWindowSleep();
        OnDiscussionWindowSleepExternal.Invoke();
    }
    #endregion

    #region Discussion Text Event
    public void OnDiscussionTextNodeEnd()
    {
        OnDiscussionTextNodeEndEvent.Invoke();
    }
    #endregion

    #region Discusion Choices Event
    public void OnDiscussionChoiceStart(DiscussionChoiceNode discussionChoice)
    {
        DiscussionWindowManager.OnChoicePopupAwake(discussionChoice);
    }

    public void OnDiscussionChoiceEnd(DiscussionChoiceTextId selectedChoice)
    {
        ScenarioTimelineEventManager.OnScenarioActionExecuted(new DiscussionChoiceScenarioAction(selectedChoice));
        OnDiscussionChoiceNodeEndEvent.Invoke(selectedChoice);
    }
    #endregion

    public void InitializeEventHanlders(DiscussionWindowSleepExternalHandler DiscussionWindowSleepExternalHandler, DiscussionTextNodeHandler DiscussionTextNodeHandler, DiscussionChoiceNodeHandler DiscussionChoiceNodeHandler)
    {
        OnDiscussionWindowSleepExternal += DiscussionWindowSleepExternalHandler;
        OnDiscussionTextNodeEndEvent += DiscussionTextNodeHandler;
        OnDiscussionChoiceNodeEndEvent += DiscussionChoiceNodeHandler;
    }

    public void DeleteEventHanlders(DiscussionWindowSleepExternalHandler DiscussionWindowSleepExternalHandler, DiscussionTextNodeHandler DiscussionTextNodeHandler, DiscussionChoiceNodeHandler DiscussionChoiceNodeHandler)
    {
        OnDiscussionWindowSleepExternal -= DiscussionWindowSleepExternalHandler;
        OnDiscussionTextNodeEndEvent -= DiscussionTextNodeHandler;
        OnDiscussionChoiceNodeEndEvent -= DiscussionChoiceNodeHandler;
    }

}
