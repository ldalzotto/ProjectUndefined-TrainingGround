using System;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    [CreateAssetMenu(fileName = "CoreStaticConfiguration", menuName = "Configuration/CoreGame/StaticConfiguration/CoreStaticConfiguration", order = 1)]
    public class CoreStaticConfiguration : ScriptableObject
    {
        public CoreInputConfiguration CoreInputConfiguration;
        public CorePrefabConfiguration CorePrefabConfiguration;
        public GlobalGameConfiguration GlobalGameConfiguration;
    }
}