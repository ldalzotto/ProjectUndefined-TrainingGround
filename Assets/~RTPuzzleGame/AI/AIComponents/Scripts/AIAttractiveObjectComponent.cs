using System;
using UnityEngine;

namespace RTPuzzle
{
    public class AIAttractiveObjectComponent
    {
        #region State
        private bool isAttracted;
        #endregion

        private Vector3? attractionPosition;

        public bool IsAttracted { get => isAttracted; }

        public Vector3? TickComponent()
        {
            if (isAttracted)
            {
                return attractionPosition;
            }
            return null;
        }

        public void OnTriggerEnter(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTAttractiveObject)
            {
                this.isAttracted = true;
                this.attractionPosition = collisionType.transform.position;
            }
        }

        public void OnTriggerStay(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTAttractiveObject)
            {
                this.isAttracted = true;
                this.attractionPosition = collisionType.transform.position;
            }
        }

        internal void OnTriggerExit(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTAttractiveObject)
            {
                OnDestinationReached();
            }
        }

        public void OnDestinationReached()
        {
            this.isAttracted = false;
            attractionPosition = null;
        }
    }
}
