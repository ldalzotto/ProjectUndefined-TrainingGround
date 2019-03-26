using System.Collections.Generic;
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelConfigurationData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfigurationDataData", order = 1)]
    public class LevelConfigurationData : ScriptableObject
    {
        [SerializeField]
        private float availableTimeAmount;

        [SerializeField]
        public List<PlayerActionIdWrapper> playerActionIds = new List<PlayerActionIdWrapper>();

        private List<RTPPlayerAction> playerActions;

        public void Init(PlayerActionConfiguration playerActionConfiguration)
        {
            this.playerActions = new List<RTPPlayerAction>();

            foreach (var playerActionid in playerActionIds)
            {
                var playerActionInherentData = playerActionConfiguration.ConfigurationInherentData[playerActionid.playerActionId];
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
        
        public float AvailableTimeAmount { get => availableTimeAmount; set => availableTimeAmount = value; }
        public List<PlayerActionIdWrapper> PlayerActionIds { get => playerActionIds;}
    }

    [System.Serializable]
    public class PlayerActionIdWrapper
    {
        [SearchableEnum]
        public PlayerActionId playerActionId;
    }

}
