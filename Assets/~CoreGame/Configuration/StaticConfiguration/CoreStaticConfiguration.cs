using UnityEngine;
using UnityEditor;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "CoreStaticConfiguration", menuName = "Configuration/CoreGame/StaticConfiguration/CoreStaticConfiguration", order = 1)]
    public class CoreStaticConfiguration : ScriptableObject
    {
        public CoreInputConfiguration CoreInputConfiguration;
    }
}
