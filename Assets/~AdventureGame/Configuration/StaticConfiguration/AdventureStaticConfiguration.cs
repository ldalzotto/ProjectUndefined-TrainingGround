using UnityEngine;
using UnityEditor;

namespace AdventureGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "AdventureStaticConfiguration", menuName = "Configuration/AdventureGame/StaticConfiguration/AdventureStaticConfiguration", order = 1)]
    public class AdventureStaticConfiguration : ScriptableObject
    {
        public DiscussionTextRepertoire DiscussionTestRepertoire;
    }
}
