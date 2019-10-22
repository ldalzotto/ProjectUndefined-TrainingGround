using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace CoreGame
{
    public class DiscussionPositionManager : GameSingleton<DiscussionPositionManager>
    {
        #region Data Retrieval

        public DiscussionPositionMarker GetDiscussionPosition(DiscussionPositionMarkerID discussionPositionMarkerID)
        {
            return CoreGameSingletonInstances.DiscussionPositionsType.GetPosition(discussionPositionMarkerID);
        }

        #endregion
    }
}