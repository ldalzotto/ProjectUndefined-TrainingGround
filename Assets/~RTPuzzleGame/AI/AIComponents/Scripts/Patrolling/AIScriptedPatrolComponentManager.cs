using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class AIScriptedPatrolComponentManager : AbstractAIPatrolComponentManager
    {
        public override void GizmoTick()
        {
        }

        public override void OnDestinationReached()
        {
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            return null;
        }

        public override void OnStateReset()
        {
        }

        protected override bool IsPatrolling()
        {
            return true;
        }
    }

}
