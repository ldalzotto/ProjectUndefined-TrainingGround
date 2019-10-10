using AdventureGame;
using CoreGame;
using Editor_DiscussionTimelineNodeEditor;
using Editor_DiscussionTreeNodeEditor;
using Editor_LevelAvailabilityNodeEditor;
using Editor_ScenarioNodeEditor;
using NodeGraph;
using RTPuzzle;
using UnityEditor;
using UnityEngine;

namespace Experimental.Editor_NodeEditor
{
    [CustomEditor(typeof(NodeEditorProfile), true)]
    public class NodeEditorCustomInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OPEN IN EDITOR"))
            {

                if (this.target.GetType() == typeof(LevelAvailabilityNodeEditorProfile))
                {
                    LevelAvailabilityNodeEditor.Init((NodeEditorProfile)this.target, typeof(LevelAvailabilityNodeEditor));
                }
                else if (this.target.GetType() == typeof(ScenarioNodeEditorProfile))
                {
                    ScenarioNodeEditor.Init((NodeEditorProfile)this.target, typeof(ScenarioNodeEditor));
                }
                else if (this.target.GetType() == typeof(DiscussionTreeNodeEditorProfile))
                {
                    DiscussionTreeNodeEditorProfile.Init((NodeEditorProfile)this.target);
                }
                else if (this.target.GetType() == typeof(DiscussionTimelineNodeEditorProfile))
                {
                    DiscussionTimelineNodeEditor.Init((NodeEditorProfile)this.target, typeof(DiscussionTimelineNodeEditor));
                }
                else if (this.target.GetType() == typeof(CutsceneGraph))
                {
                    CutsceneNodeEditor.Init((NodeEditorProfile)this.target);
                }
                else if (this.target.GetType() == typeof(PuzzleCutsceneGraph))
                {
                    PuzzleCutsceneNodeEditor.Init((NodeEditorProfile)this.target);
                }
                else if (this.target.GetType() == typeof(TutorialGraph))
                {
                    TutorialNodeEditor.Init((NodeEditorProfile)this.target);
                }
                else if (this.target.GetType() == typeof(AIPatrolGraph))
                {
                    AIPatrolNodeEditor.Init((NodeEditorProfile)this.target);
                }
                else
                {
                    Debug.LogError("NodeEditor not defined.");
                }
            }
            base.OnInspectorGUI();
        }
    }
}
