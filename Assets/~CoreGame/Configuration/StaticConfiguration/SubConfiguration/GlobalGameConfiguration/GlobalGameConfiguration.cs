using UnityEngine;
using System.Collections;
using OdinSerializer;
using GameConfigurationID;

namespace CoreGame
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "GlobalGameConfiguration", menuName = "Configuration/CoreGame/GlobalGameConfiguration/GlobalGameConfiguration", order = 1)]
    public class GlobalGameConfiguration : SerializedScriptableObject
    {
        [CustomEnum()]
        public LevelZonesID NewGameStartLevelID;

        public CameraFollowManagerComponent CameraFollowManagerComponent;
    }

}
