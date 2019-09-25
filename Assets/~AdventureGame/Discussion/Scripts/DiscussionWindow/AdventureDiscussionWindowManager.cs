using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

    public class AdventureDiscussionWindowManager : AbstractDiscussionWindowManager
    {

        #region External Dependencies
        private PointOfInterestManager PointOfInterestManager;
        private DiscussionEventHandler DiscussionEventHandler;
        #endregion

        #region Internal Managers
        private DiscussionPOIAnimationManager DiscussionPOIAnimationManager;
        #endregion

        public AdventureDiscussionWindowManager(DiscussionTreeId DiscussionTreeId, DiscussionWindowManagerStrategy DiscussionWindowManagerStrategy = null)
        {
            this.PointOfInterestManager = AdventureGameSingletonInstances.PointOfInterestManager;
            this.DiscussionEventHandler = AdventureGameSingletonInstances.DiscussionEventHandler;
            this.DiscussionPOIAnimationManager = new DiscussionPOIAnimationManager();
            this.BaseInit(DiscussionTreeId, null, DiscussionWindowManagerStrategy);
        }

        protected override bool GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode, out Vector3 worldPosition, out WindowPositionType WindowPositionType)
        {
            if (!base.GetAbstractTextOnlyNodePosition(abstractDiscussionTextOnlyNode, out worldPosition, out WindowPositionType))
            {
                if (abstractDiscussionTextOnlyNode.GetType() == typeof(AdventureDiscussionTextOnlyNode))
                {
                    var TalkingPointOfInterestType = this.PointOfInterestManager.GetActivePointOfInterest(((AdventureDiscussionTextOnlyNode)abstractDiscussionTextOnlyNode).Talker);
                    worldPosition = TalkingPointOfInterestType.transform.position + new Vector3(0, TalkingPointOfInterestType.GetAverageModelBoundLocalSpace().SideDistances.y, 0);
                    WindowPositionType = WindowPositionType.WORLD;
                    this.DiscussionEventHandler.OnAdventureDiscussionTextOnlyStart(TalkingPointOfInterestType);
                    return true;
                }
                return false;
            }
            return true;
        }
    }


}