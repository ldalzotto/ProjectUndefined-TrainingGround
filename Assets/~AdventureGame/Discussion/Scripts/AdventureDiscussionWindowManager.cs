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

        protected override Transform GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode)
        {
            var position = base.GetAbstractTextOnlyNodePosition(abstractDiscussionTextOnlyNode);
            if (position == null)
            {
                if (abstractDiscussionTextOnlyNode.GetType() == typeof(AdventureDiscussionTextOnlyNode))
                {
                    return this.PointOfInterestManager.GetActivePointOfInterest(((AdventureDiscussionTextOnlyNode)abstractDiscussionTextOnlyNode).Talker).transform;
                }
            }
            return position;
        }
    }


}