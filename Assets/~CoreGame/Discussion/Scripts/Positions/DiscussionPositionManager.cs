using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace CoreGame
{
    public class DiscussionPositionManager : MonoBehaviour
    {
        private DiscussionPositionsType discussionPositionsType;

        public void Init()
        {
            this.discussionPositionsType = GameObject.FindObjectOfType<DiscussionPositionsType>();
        }

        #region Data Retrieval
        public DiscussionPositionMarker GetDiscussionPosition(DiscussionPositionMarkerID discussionPositionMarkerID)
        {
            return this.discussionPositionsType.GetPosition(discussionPositionMarkerID);
        }
        #endregion
    }
}
