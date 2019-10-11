using CoreGame;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using NodeGraph_Editor;
#endif

namespace CoreGame
{
    [System.Serializable]
    public class CutsceneWorkflowWaitForSecondsAction : SequencedAction
    {

        [SerializeField]
        public float SecondsToWait = 0f;

        [NonSerialized]
        private bool hasEnded;
        [NonSerialized]
        private float currentTimeElapsed;

        public CutsceneWorkflowWaitForSecondsAction(float SecondsToWait, List<SequencedAction> nexActions) : base(nexActions)
        {
            this.SecondsToWait = SecondsToWait;
        }

        public CutsceneWorkflowWaitForSecondsAction(List<SequencedAction> nextActions) : base(nextActions)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return this.hasEnded;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            this.hasEnded = false;
            this.currentTimeElapsed = 0f;
        }

        public override void Tick(float d)
        {
            this.currentTimeElapsed += d;
            this.hasEnded = this.currentTimeElapsed >= SecondsToWait;
        }

        public override void Interupt()
        {
            base.Interupt();
            this.hasEnded = true;
        }

#if UNITY_EDITOR
        public override void ActionGUI()
        {
            this.SecondsToWait = NodeEditorGUILayout.FloatField("Seconds to wait : ", string.Empty, this.SecondsToWait);
        }
#endif
    }

}
