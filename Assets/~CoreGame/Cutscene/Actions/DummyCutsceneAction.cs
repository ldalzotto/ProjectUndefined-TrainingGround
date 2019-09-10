using CoreGame;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace CoreGame
{
    [System.Serializable]
    public class DummyCutsceneAction : SequencedAction
    {
        public DummyCutsceneAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
        }

        public override void Tick(float d)
        {
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            NodeEditorGUILayout.LabelField("");
        }
#endif
    }

}
