using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "PlayerActionsInherentData", menuName = "Configuration/PuzzleGame/PlayerActionConfiguration/PlayerActionsInherentData", order = 1)]
    public class PlayerActionsInherentData : ScriptableObject
    {
        public List<PlayerActionInherentData> PlayerActionInherentDatas;
        private List<RTPPlayerAction> playerActions;

        public PlayerActionsInherentData(List<PlayerActionInherentData> playerActionInherentDatas)
        {
            this.PlayerActionInherentDatas = playerActionInherentDatas;
        }

        public void Init()
        {
            this.playerActions = new List<RTPPlayerAction>();
            foreach (var playerActionInherentData in PlayerActionInherentDatas)
            {
                if (playerActionInherentData.GetType() == typeof(LaunchProjectileActionInherentData))
                {
                    this.playerActions.Add(new LaunchProjectileAction((LaunchProjectileActionInherentData)playerActionInherentData));
                }
                else if (playerActionInherentData.GetType() == typeof(AttractiveObjectActionInherentData))
                {
                    this.playerActions.Add(new AttractiveObjectAction((AttractiveObjectActionInherentData)playerActionInherentData));
                }
            }
        }

        public List<RTPPlayerAction> PlayerActions { get => playerActions; }
    }
}
