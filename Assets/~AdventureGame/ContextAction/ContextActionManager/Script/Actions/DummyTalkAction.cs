
using CoreGame;
using GameConfigurationID;
using System.Collections.Generic;

namespace AdventureGame
{

    [System.Serializable]
    public class DummyTalkAction : AContextAction
    {

        public DummyTalkAction(List<SequencedAction> nextContextActions, SelectionWheelNodeConfigurationId SelectionWheelNodeConfigurationId) : base(nextContextActions, SelectionWheelNodeConfigurationId) { }

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