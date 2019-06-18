using UnityEngine;
using System.Collections;
using Editor_LevelAvailabilityNodeEditor;
using CoreGame;
using GameConfigurationID;

namespace Editor_DiscussionTimelineNodeEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTimelineNodeEditorProfile", menuName = "Configuration/AdventureGame/DiscussionConfiguration/DiscussionTimelineNodeEditorProfile", order = 1)]
    public class DiscussionTimelineNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineID TimelineID => TimelineID.DISCUSSION_TIMELINE;
    }
}
