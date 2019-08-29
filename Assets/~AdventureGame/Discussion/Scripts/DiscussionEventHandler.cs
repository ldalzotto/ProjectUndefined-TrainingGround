using System;
using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

    public class DiscussionEventHandler : MonoBehaviour
    {

        private DiscussionManager DiscussionManager;

        private TimelinesEventManager ScenarioTimelineEventManager;

        private void Start()
        {
            DiscussionManager = GameObject.FindObjectOfType<DiscussionManager>();
            ScenarioTimelineEventManager = GameObject.FindObjectOfType<TimelinesEventManager>();
        }
        
        public AbstractDiscussionWindowManager OnDiscussionTreeStart(DiscussionTreeId discussionTreeId)
        {
            return DiscussionManager.OnDiscussionTreeStart(discussionTreeId);
        }
        
        public void OnDiscussionChoiceMade(DiscussionNodeId discussionChoiceMade)
        {
            ScenarioTimelineEventManager.OnScenarioActionExecuted(new DiscussionChoiceScenarioAction(discussionChoiceMade));
        }

        public void OnAdventureDiscussionTextOnlyStart(PointOfInterestType talkingPointOfInterestType)
        {
            DiscussionManager.OnAdventureDiscussionTextOnlyStart(talkingPointOfInterestType);
        }
    }

}