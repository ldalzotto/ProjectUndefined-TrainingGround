using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionConfiguration", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/PlayerActionConfiguration", order = 1)]
    public class PlayerActionConfiguration : ScriptableObject, ISerializationCallbackReceiver
    {
        public Dictionary<LevelZonesID, PlayerActionsInherentData> conf = new Dictionary<LevelZonesID, PlayerActionsInherentData>() {
            /* {

                  LevelZonesID.SEWER_RTP,
                      new PlayerActionsInherentData(
                          new List<PlayerActionInherentData>(){
                              new LaunchProjectileActionInherentData(LaunchProjectileId.STONE, SelectionWheelNodeConfigurationId.THROW_PLAYER_PUZZLE_WHEEL_CONFIG,0.5f),
                               new AttractiveObjectActionInherentData(AttractiveObjectId.CHEESE, SelectionWheelNodeConfigurationId.ATTRACTIVE_OBJECT_LAY_WHEEL_CONFIG, 3f)
                          })
                         
        } */
        };

        [SerializeField]
        private List<PlayerActionsInherentSerializableDatasWithID> PlayerActionsInherentSerializableDatas;

        public void Init()
        {
            foreach (var playerActionConfData in conf)
            {
                playerActionConfData.Value.Init();
            }
        }

        public void OnBeforeSerialize()
        {
            if (PlayerActionsInherentSerializableDatas != null)
            {
                PlayerActionsInherentSerializableDatas.Clear();
            }

            foreach (var playerActionConfData in conf)
            {
                PlayerActionsInherentSerializableDatas.Add(new PlayerActionsInherentSerializableDatasWithID(playerActionConfData.Key, playerActionConfData.Value));
            }
        }

        public void OnAfterDeserialize()
        {
            foreach (var playerActionSerializableData in PlayerActionsInherentSerializableDatas)
            {
                conf.Add(playerActionSerializableData.LevelZonesID, playerActionSerializableData.LaunchProjectileInherentData);
            }
        }

        [System.Serializable]
        class PlayerActionsInherentSerializableDatasWithID
        {
            public LevelZonesID LevelZonesID;
            public PlayerActionsInherentData LaunchProjectileInherentData;

            public PlayerActionsInherentSerializableDatasWithID(LevelZonesID levelZonesID, PlayerActionsInherentData launchProjectileInherentData)
            {
                LevelZonesID = levelZonesID;
                LaunchProjectileInherentData = launchProjectileInherentData;
            }
        }
    }
}
