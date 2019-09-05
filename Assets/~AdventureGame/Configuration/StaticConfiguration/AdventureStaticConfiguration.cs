using OdinSerializer;
using UnityEngine;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AdventureStaticConfiguration", menuName = "Configuration/AdventureGame/AdventureStaticConfiguration", order = 1)]
    public class AdventureStaticConfiguration : SerializedScriptableObject
    {
        public AdventurePlayerMovementConfiguration AdventurePlayerMovementConfiguration;
        public AdventureDiscussionStaticConfiguration AdventureDiscussionStaticConfiguration;
        public AdventurePrefabConfiguration AdventurePrefabConfiguration;
    }
}
