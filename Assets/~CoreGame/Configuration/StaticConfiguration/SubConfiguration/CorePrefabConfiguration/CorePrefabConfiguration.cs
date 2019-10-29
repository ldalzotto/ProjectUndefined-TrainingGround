using System;
using OdinSerializer;
using UnityEngine;

namespace CoreGame
{
    [Serializable]
    [CreateAssetMenu(fileName = "CorePrefabConfiguration", menuName = "Configuration/CoreGame/CorePrefabConfiguration/CorePrefabConfiguration", order = 1)]
    public class CorePrefabConfiguration : SerializedScriptableObject
    {
        public GameObject ActionWheelNodePrefab;
    }
}