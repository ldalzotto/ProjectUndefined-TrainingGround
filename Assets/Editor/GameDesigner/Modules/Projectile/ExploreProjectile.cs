using CoreGame;
using RTPuzzle;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Editor_GameDesigner
{
    [System.Serializable]
    public class ExploreProjectile : ExploreModule
    {
        private List<ProjectileInherentData> ProjectileDatas = new List<ProjectileInherentData>();
        private Dictionary<ProjectileInherentData, Editor> ProjectileDatasEditor = new Dictionary<ProjectileInherentData, Editor>();


        public override void GUITick()
        {
            this.DisplayObjects("Projectiles : ", this.ProjectileDatas, ref this.ProjectileDatasEditor);
        }

        public override void OnEnabled()
        {
            base.OnEnabled();
            var levelManager = GameObject.FindObjectOfType<LevelManager>();
            var projectileConfiguration = AssetFinder.SafeSingleAssetFind<ProjectileConfiguration>("t:" + typeof(ProjectileConfiguration).Name);
            var playerActionConfiguration = AssetFinder.SafeSingleAssetFind<PlayerActionConfiguration>("t:" + typeof(PlayerActionConfiguration).Name);
            var levelConfiguraiton = AssetFinder.SafeSingleAssetFind<LevelConfiguration>("t:" + typeof(LevelConfiguration).Name);

            if (levelManager != null && projectileConfiguration != null && playerActionConfiguration != null && levelConfiguraiton != null)
            {
                this.ProjectileDatas =
                        levelConfiguraiton.ConfigurationInherentData[levelManager.LevelID].PlayerActionIds
                            .ConvertAll(w => w.playerActionId)
                            .ConvertAll(pId => playerActionConfiguration.ConfigurationInherentData[pId])
                            .Select(action => action)
                            .Where(action => action.GetType() == typeof(LaunchProjectileActionInherentData))
                            .Select(action => (LaunchProjectileActionInherentData)action)
                            .Select(action => projectileConfiguration.ConfigurationInherentData[action.launchProjectileId])
                            .ToList();
            }
        }
    }

}