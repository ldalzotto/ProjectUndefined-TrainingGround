using System;
using System.Collections.Generic;
using SequencedAction;

#if UNITY_EDITOR

#endif

namespace CoreGame
{
    [Serializable]
    public class DummyCutsceneAction : ASequencedAction
    {
        public DummyCutsceneAction(Func<List<ASequencedAction>> nextActionsDeferred) : base(nextActionsDeferred)
        {
        }

        public override void AfterFinishedEventProcessed()
        {
        }

        public override bool ComputeFinishedConditions()
        {
            return true;
        }

        public override void FirstExecutionAction()
        {
        }

        public override void Tick(float d)
        {
        }
    }
}