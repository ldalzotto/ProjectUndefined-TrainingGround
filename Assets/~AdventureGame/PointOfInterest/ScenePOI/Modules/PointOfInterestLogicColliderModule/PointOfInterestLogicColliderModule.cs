using GameConfigurationID;
using UnityEngine;

namespace AdventureGame
{
    public class PointOfInterestLogicColliderModule : APointOfInterestModule
    {
        private Collider associatedCollider;

        public void Init()
        {
            this.associatedCollider = this.GetComponent<Collider>();
        }

        public static class PointOfInterestLogicColliderModuleInstancer
        {
            public static void PopuplateFromDefinition(PointOfInterestLogicColliderModule PointOfInterestLogicColliderModule, PointOfInterestLogicColliderModuleDefinition PointOfInterestLogicColliderModuleDefinition,
                PointOfInterestDefinitionID PointOfInterestDefinitionID)
            {
                var collider = PointOfInterestLogicColliderModule.GetComponent<BoxCollider>();
                collider.center = PointOfInterestLogicColliderModuleDefinition.DeltaLocalPosition;
                if (PointOfInterestDefinitionID == PointOfInterestDefinitionID.PLAYER)
                {
                    var collisionType = PointOfInterestLogicColliderModule.GetComponent<CollisionType>();
                    collisionType.IsPlayer = true;
                }
            }
        }

        #region Data retrieval
        public Vector3 GetWorldPositionColliderCenter()
        {
            return this.associatedCollider.bounds.center;
        }
        #endregion

        public void DisableCollider()
        {
            this.associatedCollider.enabled = false;
        }

        public void EnableCollider()
        {
            this.associatedCollider.enabled = true;
        }
    }

}
