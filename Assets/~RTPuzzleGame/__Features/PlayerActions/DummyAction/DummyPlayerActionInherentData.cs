using System;
using InteractiveObjects;
using UnityEngine;

namespace RTPuzzle
{
    [Serializable]
    [CreateAssetMenu(fileName = "DummyPlayerActionInherentData", menuName = "Test/DummyPlayerActionInherentData", order = 1)]
    public class DummyPlayerActionInherentData : PlayerActionInherentData
    {
        public override RTPPlayerAction BuildPlayerAction(PlayerInteractiveObject PlayerInteractiveObject)
        {
            return new DummyPlayerAction(CorePlayerActionDefinition);
        }
    }

    public class DummyPlayerAction : RTPPlayerAction
    {
        public DummyPlayerAction(CorePlayerActionDefinition CorePlayerActionDefinition) : base(CorePlayerActionDefinition)
        {
        }

        public override bool FinishedCondition()
        {
            return true;
        }

        public override void GizmoTick()
        {
        }

        public override void GUITick()
        {
        }

        public override void LateTick(float d)
        {
        }

        public override void Tick(float d)
        {
        }
    }
}