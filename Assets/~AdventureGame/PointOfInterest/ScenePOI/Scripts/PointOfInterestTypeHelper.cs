using System;
using CoreGame;
using UnityEngine;

namespace AdventureGame
{
    public static class PointOfInterestTypeHelper
    {
        public static PointOfInterestType FromCollisionType(CollisionType collisionType)
        {
            return collisionType.GetComponentInParent<PointOfInterestType>();
        }
        
    }

}
