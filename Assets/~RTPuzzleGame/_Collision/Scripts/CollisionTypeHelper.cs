using UnityEngine;

namespace RTPuzzle
{
    public class CollisionTypeHelper
    {

        public static AIObjectType GetAIManager(CollisionType collisionType)
        {
            if (collisionType != null)
            {
                if (collisionType.IsAI)
                {
                    return collisionType.GetComponent<AIObjectType>();
                }
            }
            return null;
        }

    }
}

