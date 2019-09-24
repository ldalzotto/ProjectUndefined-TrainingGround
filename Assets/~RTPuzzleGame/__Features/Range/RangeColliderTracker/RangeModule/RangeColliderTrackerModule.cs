using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    public interface IRangeColliderTrackerModuleDataTriever
    {
        List<CollisionType> GetTrackedPlayerColliders();
    }

    public class RangeColliderTrackerModule : MonoBehaviour, IRangeColliderTrackerModuleDataTriever
    {
        private List<CollisionType> trackedColliders = new List<CollisionType>();

        #region Index Trackers
        private List<CollisionType> playerTrackedColliders = new List<CollisionType>();
        #endregion

        #region IRangeColliderTrackerModuleDataTriever
        public List<CollisionType> GetTrackedPlayerColliders()
        {
            return playerTrackedColliders;
        }
        #endregion

        public void OnRangeTriggerEnter(CollisionType collisionType)
        {
            if (collisionType != null)
            {
                this.trackedColliders.Add(collisionType);
                if (collisionType.IsPlayer)
                {
                    this.playerTrackedColliders.Add(collisionType);
                }
            }
        }

        public void OnRangeTriggerExit(CollisionType collisionType)
        {
            if (collisionType != null)
            {
                this.trackedColliders.Remove(collisionType);
                if (collisionType.IsPlayer)
                {
                    this.playerTrackedColliders.Remove(collisionType);
                }
            }
        }
    }

}

