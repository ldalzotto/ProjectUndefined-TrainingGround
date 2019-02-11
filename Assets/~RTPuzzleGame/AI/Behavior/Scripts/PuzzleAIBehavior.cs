using System;
using UnityEngine;
using UnityEngine.AI;

namespace RTPuzzle
{
    public abstract class PuzzleAIBehavior
    {

        #region External Dependencies
        protected NavMeshAgent selfAgent;
        #endregion

        public PuzzleAIBehavior(NavMeshAgent selfAgent)
        {
            this.selfAgent = selfAgent;
        }

        public abstract Nullable<Vector3> TickAI();
        public abstract void TickGizmo();
        public abstract void OnTriggerEnter(Collider collider);
        public abstract void OnTriggerStay(Collider collider);
        public abstract void OnTriggerExit(Collider collider);

        public abstract void OnDestinationReached();

    }

}
