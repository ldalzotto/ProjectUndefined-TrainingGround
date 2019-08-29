using System;
using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public class DiscussionManager : MonoBehaviour
    {
        #region External Dependencies
        private DiscussionWindowsContainer DiscussionWindowsContainer;
        #endregion

        #region Internal Managers
        private DiscussionPOIAnimationManager DiscussionPOIAnimationManager;
        #endregion

        public void Init()
        {
            this.DiscussionWindowsContainer = GameObject.FindObjectOfType<DiscussionWindowsContainer>();
            this.DiscussionWindowsContainer.Init();
            this.DiscussionPOIAnimationManager = new DiscussionPOIAnimationManager();
        }

        public void Tick(float d)
        {
            this.DiscussionWindowsContainer.Tick(d);
            this.DiscussionPOIAnimationManager.Tick(d);
        }

        #region External Events
        public AbstractDiscussionWindowManager OnDiscussionTreeStart(DiscussionTreeId discussionTreeId)
        {
            return this.DiscussionWindowsContainer.OnDiscussionTreeStart(discussionTreeId);
        }

        public void OnAdventureDiscussionTextOnlyStart(PointOfInterestType talkingPointOfInterestType)
        {
            this.DiscussionPOIAnimationManager.OnAdventureDiscussionTextOnlyStart(talkingPointOfInterestType);
        }
        #endregion
    }
}
