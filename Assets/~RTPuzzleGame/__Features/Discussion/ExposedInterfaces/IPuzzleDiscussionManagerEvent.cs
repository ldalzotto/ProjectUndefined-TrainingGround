using UnityEngine;
using System.Collections;
using GameConfigurationID;
using System.Collections.Generic;
using CoreGame;

namespace RTPuzzle
{
    public interface IPuzzleDiscussionManagerEvent
    {
        void PlayDiscussion(DiscussionTreeId DiscussionTreeId, Dictionary<CutsceneParametersName, object> discussionGraphParameters);
    }
}
