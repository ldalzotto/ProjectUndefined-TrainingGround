using UnityEngine;

namespace RTPuzzle
{
    public class CollisionTypeHelper
    {

        public static NPCAIManager GetAIManager(CollisionType collisionType)
        {
            if (collisionType != null)
            {
                if (collisionType.IsAI)
                {
                    return collisionType.GetComponent<NPCAIManager>();
                }
            }
            return null;
        }

    }
}

