using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

    public class DiscussionEventHandler : MonoBehaviour
    {

        private DiscussionWindowsContainer DiscussionWindowsContainer;

        private TimelinesEventManager ScenarioTimelineEventManager;

        private void Start()
        {
            DiscussionWindowsContainer = GameObject.FindObjectOfType<DiscussionWindowsContainer>();
            ScenarioTimelineEventManager = GameObject.FindObjectOfType<TimelinesEventManager>();
        }
        
        public AbstractDiscussionWindowManager OnDiscussionTreeStart(DiscussionTreeId discussionTreeId)
        {
            return DiscussionWindowsContainer.OnDiscussionTreeStart(discussionTreeId);
        }
        
        public void OnDiscussionChoiceMade(DiscussionNodeId discussionChoiceMade)
        {
            ScenarioTimelineEventManager.OnScenarioActionExecuted(new DiscussionChoiceScenarioAction(discussionChoiceMade));
        }
    }

}