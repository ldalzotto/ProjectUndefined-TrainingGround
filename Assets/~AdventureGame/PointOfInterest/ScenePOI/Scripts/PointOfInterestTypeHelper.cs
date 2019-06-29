using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public static class PointOfInterestTypeHelper
    {
        public static PointOfInterestType FromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponent<PointOfInterestType>();
        }

        public static GameObject GetModelObject(PointOfInterestType pointOfInterestType)
        {
            return pointOfInterestType.transform.parent.gameObject.FindChildObjectWithLevelLimit(PointOfInterestTypeConstants.MODEL_OBJECT_NAME, 0);
        }

        public static PointOfInterestModules GetPointOfInterestModules(PointOfInterestType pointOfInterestType)
        {
            return pointOfInterestType.transform.parent.GetComponentInChildren<PointOfInterestModules>();
        }

        public static GameObject GetModulesObject(PointOfInterestModules PointOfInterestModules)
        {
            return PointOfInterestModules.gameObject.FindChildObjectRecursively(PointOfInterestTypeConstants.MODULES_OBJECT_NAME);
        }

        public static DataComponentContainer GetDataComponentContainer(PointOfInterestType pointOfInterestType)
        {
            return pointOfInterestType.transform.parent.GetComponentInChildren<DataComponentContainer>();
        }

        public static GameObject GetDataComponentsObject(DataComponentContainer DataComponentContainer)
        {
            return DataComponentContainer.gameObject.FindChildObjectRecursively(PointOfInterestTypeConstants.DATA_COMPONENTS_OBJECT_NAME);
        }
    }

}
