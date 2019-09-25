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
            this.DiscussionWindowsContainer = AdventureGameSingletonInstances.DiscussionWindowsContainer;
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
            var adventureDiscussionWindowManager = new AdventureDiscussionWindowManager(discussionTreeId);
            this.DiscussionWindowsContainer.AddDiscussionTree(adventureDiscussionWindowManager);
            return adventureDiscussionWindowManager;
        }

        public void OnAdventureDiscussionTextOnlyStart(PointOfInterestType talkingPointOfInterestType)
        {
            this.DiscussionPOIAnimationManager.OnAdventureDiscussionTextOnlyStart(talkingPointOfInterestType);
        }
        #endregion
    }
}
