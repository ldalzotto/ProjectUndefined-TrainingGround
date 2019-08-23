using UnityEngine;
using System.Collections;
using OdinSerializer;
using CoreGame;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AdventurePlayerMovementConfiguration", menuName = "Configuration/AdventureGame/AdventureStaticConfiguration/AdventurePlayerMovementConfiguration", order = 1)]
    public class AdventurePlayerMovementConfiguration : SerializedScriptableObject
    {
        public TransformMoveManagerComponentV3 PlayerTransformMoveComponent;
    }
}
