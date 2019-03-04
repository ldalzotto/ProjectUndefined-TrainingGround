using UnityEngine;

namespace RTPuzzle
{
    public class AIAttractiveObjectComponent
    {
        #region State
        private bool isAttracted;
        #endregion
        private AttractiveObjectType involvedAttractiveObject;
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
                SetAttractedObject(collisionType);
            }
        }

        public void OnTriggerStay(Collider collider, CollisionType collisionType)
        {
            if (collisionType.IsRTAttractiveObject)
            {
                SetAttractedObject(collisionType);
            }
        }

        private void SetAttractedObject(CollisionType collisionType)
        {
            this.isAttracted = true;
            this.attractionPosition = collisionType.transform.position;
            this.involvedAttractiveObject = AttractiveObjectType.GetAttractiveObjectFromCollisionType(collisionType);
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

        public void ClearAttractedObject()
        {
            this.involvedAttractiveObject = null;
        }

        #region Logical Conditions
        public bool IsDestructedAttractiveObjectEqualsToCurrent(AttractiveObjectType attractiveObjectToDestroy)
        {
            return (this.involvedAttractiveObject != null &&
                attractiveObjectToDestroy.GetInstanceID() == this.involvedAttractiveObject.GetInstanceID());
        }
        #endregion

    }
}
