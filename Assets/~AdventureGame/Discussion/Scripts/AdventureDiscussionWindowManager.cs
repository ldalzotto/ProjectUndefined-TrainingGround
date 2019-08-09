using CoreGame;
using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{

    public class AdventureDiscussionWindowManager : AbstractDiscussionWindowManager
    {
        private PointOfInterestManager PointOfInterestManager;

        public AdventureDiscussionWindowManager(DiscussionTreeId DiscussionTreeId)
        {
            this.PointOfInterestManager = GameObject.FindObjectOfType<PointOfInterestManager>();
            this.BaseInit(DiscussionTreeId);
        }

        protected override Transform GetAbstractTextOnlyNodePosition(AbstractDiscussionTextOnlyNode abstractDiscussionTextOnlyNode)
        {
            if (abstractDiscussionTextOnlyNode.GetType() == typeof(AdventureDiscussionTextOnlyNode))
            {
                return this.PointOfInterestManager.GetActivePointOfInterest(((AdventureDiscussionTextOnlyNode)abstractDiscussionTextOnlyNode).Talker).transform;
            }
            return null;
        }
    }


}