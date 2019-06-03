using CoreGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    public class DiscussionEventHandler : MonoBehaviour
    {

        private DiscussionWindowManager DiscussionWindowManager;
        private TimelinesEventManager ScenarioTimelineEventManager;

        public delegate void DiscussionWindowSleepExternalHandler();
        private event DiscussionWindowSleepExternalHandler OnDiscussionWindowSleepExternal;

        public delegate void DiscussionTextNodeHandler();
        private event DiscussionTextNodeHandler OnDiscussionTextNodeEndEvent;

        public delegate void DiscussionChoiceNodeHandler(DiscussionNodeId selectedChoice);
        private event DiscussionChoiceNodeHandler OnDiscussionChoiceNodeEndEvent;

        private void Start()
        {
            DiscussionWindowManager = GameObject.FindObjectOfType<DiscussionWindowManager>();
            ScenarioTimelineEventManager = GameObject.FindObjectOfType<TimelinesEventManager>();
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
        public void OnDiscussionChoiceStart(List<DiscussionChoice> discussionChoices)
        {
            DiscussionWindowManager.OnChoicePopupAwake(discussionChoices);
        }

        public void OnDiscussionChoiceEnd(DiscussionNodeId selectedChoice)
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

}