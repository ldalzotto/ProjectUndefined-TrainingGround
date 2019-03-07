using CoreGame;
using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionConfiguration", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/PlayerActionConfiguration", order = 1)]
    public class PlayerActionConfiguration : DictionarySerialization<LevelZonesID, PlayerActionsInherentData>
    {

        public void Init()
        {
            foreach (var playerActionConfData in LaunchProjectileInherentDatas)
            {
                playerActionConfData.Value.Init();
            }
        }
    }
}
