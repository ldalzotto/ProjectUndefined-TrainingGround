﻿using System.Collections.Generic;
using System.Linq;
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

        [SerializeField]
        public LevelCompletionInherentData LevelCompletionInherentData;

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
        public List<PlayerActionIdWrapper> PlayerActionIds { get => playerActionIds; }

#if UNITY_EDITOR
        public void AddPlayerActionId(PlayerActionIdWrapper playerActionIdWrapper)
        {
            this.playerActionIds.Add(playerActionIdWrapper);
            this.playerActionIds = this.playerActionIds.Distinct().ToList();
        }
#endif
    }

    [System.Serializable]
    public class PlayerActionIdWrapper
    {
        [SearchableEnum]
        public PlayerActionId playerActionId;

        public PlayerActionIdWrapper(PlayerActionId playerActionId)
        {
            this.playerActionId = playerActionId;
        }

        public override bool Equals(object obj)
        {
            var wrapper = obj as PlayerActionIdWrapper;
            return wrapper != null &&
                   playerActionId == wrapper.playerActionId;
        }

        public override int GetHashCode()
        {
            return 985414159 + playerActionId.GetHashCode();
        }
    }

}
