using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

    public class AdventureDiscussionWindowManager : AbstractDiscussionWindowManager
    {
        private PointOfInterestManager PointOfInterestManager;

        public AdventureDiscussionWindowManager(DiscussionTreeId DiscussionTreeId, DiscussionWindowManagerStrategy DiscussionWindowManagerStrategy = null)
        {
            this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.BaseInit(DiscussionTreeId, DiscussionWindowManagerStrategy);
        }

        protected override bool GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode, out Vector3 worldPosition, out WindowPositionType WindowPositionType)
        {
            if (!base.GetAbstractTextOnlyNodePosition(abstractDiscussionTextOnlyNode, out worldPosition, out WindowPositionType))
            {
                if (abstractDiscussionTextOnlyNode.GetType() == typeof(AdventureDiscussionTextOnlyNode))
                {
                    worldPosition = this.PointOfInterestManager.GetActivePointOfInterest(((AdventureDiscussionTextOnlyNode)abstractDiscussionTextOnlyNode).Talker).transform.position;
                    WindowPositionType = WindowPositionType.WORLD;
                    return true;
                }
                return false;
            }
            return true;
        }
    }


}