using UnityEngine;
using System.Collections;
using CoreGame;
using RTPuzzle;
using UnityEditor;
using Editor_MainGameCreationWizard;
using System.Collections.Generic;
using System.Linq;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExplorePlayerActions : ExploreModule
    {
        private LevelManager levelManager;
        private List<PlayerActionInherentData> levelPlayerActionInherentDatas;
        private Dictionary<PlayerActionInherentData, Editor> playerActionsCachedEditors;

        public override void GUITick()
        {
            this.DisplayObjects("Player actions : ", this.levelPlayerActionInherentDatas, ref this.playerActionsCachedEditors);
        }
        public override void OnEnabled()
        {
            base.OnEnabled();
            this.levelManager = GameObject.FindObjectOfType<LevelManager>();
            this.playerActionsCachedEditors = new Dictionary<PlayerActionInherentData, Editor>();
            this.levelPlayerActionInherentDatas = this.commonGameConfigurations.PuzzleGameConfigurations.PlayerActionConfiguration.ConfigurationInherentData.Select(p => p)
              .Where(p => this.commonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData[this.levelManager.LevelID].PlayerActionIds.ConvertAll(pl => pl.playerActionId).Contains(p.Key))
              .Select(p => p.Value).ToList();
        }
    }
}