using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public abstract class PuzzleAIBehavior
    {
        #region External Dependencies
        protected NavMeshAgent selfAgent;
        protected Action<FOV> OnFOVChange;
        #endregion

        public PuzzleAIBehavior(NavMeshAgent selfAgent, Action<FOV> OnFOVChange)
        {
            this.selfAgent = selfAgent;
            this.OnFOVChange = OnFOVChange;
        }

        public abstract Nullable<Vector3> TickAI(in float d,in float timeAttenuationFactor);
        public abstract void TickGizmo();
        public abstract void OnTriggerEnter(Collider collider);
        public abstract void OnTriggerStay(Collider collider);
        public abstract void OnTriggerExit(Collider collider);

        public abstract void OnDestinationReached();
        public virtual void OnAttractiveObjectDestroyed(AttractiveObjectType attractiveObjectToDestroy) { }
    }

}
