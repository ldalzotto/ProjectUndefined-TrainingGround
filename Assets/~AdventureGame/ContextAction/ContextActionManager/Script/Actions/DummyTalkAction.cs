
using CoreGame;
using System.Collections.Generic;

namespace AdventureGame
{

    [System.Serializable]
    public class DummyTalkAction : AContextAction
    {

        public DummyTalkAction(List<SequencedAction> nextContextActions) : base(nextContextActions) { }

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
    }

}