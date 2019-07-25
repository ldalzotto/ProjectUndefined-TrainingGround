using UnityEngine;
using System.Collections;

namespace RTPuzzle
{
    public class AIScriptedPatrolComponentManager : AbstractAIPatrolComponentManager
    {
        private bool isPatrolling;

        public override void GizmoTick()
        {
        }

        public override void OnDestinationReached()
        {
        }

        public override Vector3? OnManagerTick(float d, float timeAttenuationFactor)
        {
            this.isPatrolling = true;
            return null;
        }

        public override void OnStateReset()
        {
            this.isPatrolling = false;
        }

        protected override bool IsPatrolling()
        {
            return isPatrolling;
        }
    }

}
