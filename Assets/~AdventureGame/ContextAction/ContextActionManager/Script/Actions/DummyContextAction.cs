using CoreGame;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AdventureGame
{

    [System.Serializable]
    public class DummyContextAction : AContextAction
    {

        [NonSerialized]
        private float elapsedTime;

        public DummyContextAction(List<SequencedAction> nextContextActions) : base(nextContextActions) { }

        public override bool ComputeFinishedConditions()
        {
            return elapsedTime >= 2f;
        }

        public override void FirstExecutionAction(SequencedActionInput ContextActionInput)
        {
            Debug.Log("DUMMY");
            var actionInput = ContextActionInput as DummyContextActionInput;
            elapsedTime = 0f;
            Debug.Log(Time.frameCount + actionInput.Text);
        }
        public override void Tick(float d)
        {
            elapsedTime += d;
        }

        public override void AfterFinishedEventProcessed()
        {

        }
    }

    [System.Serializable]
    public class DummyContextActionInput : AContextActionInput
    {
        private string text;

        public DummyContextActionInput(string text)
        {
            this.text = text;
        }

        public string Text { get => text; }
    }
}