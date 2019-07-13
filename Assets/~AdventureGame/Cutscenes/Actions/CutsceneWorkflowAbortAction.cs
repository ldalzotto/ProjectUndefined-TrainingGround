using CoreGame;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace AdventureGame
{
    [System.Serializable]
    public class CutsceneWorkflowAbortAction : SequencedAction
    {

        [NonSerialized]
        private List<SequencedAction> sequencedActionsToInterrupt;

        public List<SequencedAction> SequencedActionsToInterrupt { set => sequencedActionsToInterrupt = value; }

        public CutsceneWorkflowAbortAction(List<SequencedAction> nextActions) : base(nextActions)
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
            if (this.sequencedActionsToInterrupt != null)
            {
                foreach (var sequencedActionToInterrupt in this.sequencedActionsToInterrupt)
                {
                    if (!sequencedActionToInterrupt.IsFinished())
                    {
                        sequencedActionToInterrupt.Interupt();
                    }
                }
            }
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
