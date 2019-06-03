using UnityEngine;
using System.Collections;
using Editor_LevelAvailabilityNodeEditor;
using CoreGame;

namespace Editor_DiscussionTimelineNodeEditor
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "DiscussionTimelineNodeEditorProfile", menuName = "Configuration/AdventureGame/DiscussionConfiguration/DiscussionTimelineNodeEditorProfile", order = 1)]
    public class DiscussionTimelineNodeEditorProfile : TimelineNodeEditorProfile
    {
        public override TimelineIDs TimelineID => TimelineIDs.DISCUSSION_TIMELINE;
    }
}
