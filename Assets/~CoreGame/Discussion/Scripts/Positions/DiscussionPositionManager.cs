using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace CoreGame
{
    public class DiscussionPositionManager : MonoBehaviour
    {
        #region Data Retrieval
        public DiscussionPositionMarker GetDiscussionPosition(DiscussionPositionMarkerID discussionPositionMarkerID)
        {
            return  CoreGameSingletonInstances.DiscussionPositionsType.GetPosition(discussionPositionMarkerID);
        }
        #endregion
    }
}
