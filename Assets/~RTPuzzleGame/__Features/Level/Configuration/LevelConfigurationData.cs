using CoreGame;
using GameConfigurationID;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RTPuzzle
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "LevelConfigurationData", menuName = "Configuration/PuzzleGame/LevelConfiguration/LevelConfigurationDataData", order = 1)]
    public class LevelConfigurationData : ScriptableObject
    {
        [SerializeField]
        private float availableTimeAmount = 20f;

        [SerializeField]
        public List<PlayerActionIdWrapper> playerActionIds = new List<PlayerActionIdWrapper>();

        [SerializeField]
        public LevelRangeEffectInherentData LevelRangeEffectInherentData;

        [NonSerialized]
        private MultiValueDictionary<PlayerActionId, RTPPlayerAction> playerActions;

        public void Init(PlayerActionConfiguration playerActionConfiguration)
        {
            this.playerActions = new MultiValueDictionary<PlayerActionId, RTPPlayerAction>();

            foreach (var playerActionid in playerActionIds)
            {
                var playerActionInherentData = playerActionConfiguration.ConfigurationInherentData[playerActionid.playerActionId];
                this.playerActions.MultiValueAdd(playerActionid.playerActionId, playerActionInherentData.BuildPlayerAction());
            }
        }

        public MultiValueDictionary<PlayerActionId, RTPPlayerAction> PlayerActions { get => playerActions; }

        public float AvailableTimeAmount { get => availableTimeAmount; set => availableTimeAmount = value; }
        public List<PlayerActionIdWrapper> PlayerActionIds { get => playerActionIds; }

#if UNITY_EDITOR
        public void AddPlayerActionId(PlayerActionIdWrapper playerActionIdWrapper)
        {
            this.playerActionIds.Add(playerActionIdWrapper);
            this.playerActionIds = this.playerActionIds.Distinct().ToList();
            EditorUtility.SetDirty(this);
        }

        public void RemovePlayerActionId(PlayerActionId playerActionId)
        {
            var playerActionIdToRemove = this.playerActionIds.Find(p => p.playerActionId == playerActionId);
            if (playerActionIdToRemove != null)
            {
                this.playerActionIds.Remove(playerActionIdToRemove);
                EditorUtility.SetDirty(this);
            }
        }
#endif
    }

    [System.Serializable]
    public class PlayerActionIdWrapper
    {
        [CustomEnum]
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

    [System.Serializable]
    public class LevelRangeEffectInherentData
    {
        public float DeltaIntensity = 0;
        [Range(-0.5f, 0.5f)]
        public float DeltaMixFactor = 0;
    }
    

}
