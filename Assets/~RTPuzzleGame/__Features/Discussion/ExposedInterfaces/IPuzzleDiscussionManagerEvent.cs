using UnityEngine;
using System.Collections;
using GameConfigurationID;

namespace RTPuzzle
{
    public interface IPuzzleDiscussionManagerEvent
    {
        void PlayDiscussion(DiscussionTreeId DiscussionTreeId);
    }
}
