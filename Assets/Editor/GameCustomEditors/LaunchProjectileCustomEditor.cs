using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using Editor_GameDesigner;
using System.Collections.Generic;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(LaunchProjectileModule))]
    public class LaunchProjectileCustomEditor : AbstractGameCustomEditorWithLiveSelection<LaunchProjectileModule, ProjectileCustomEditorContext, LaunchProjectileConfigurationModule, EditProjectile>
    {
        private void OnEnable()
        {
            if (target != null)
            {
                this.drawModules = new List<GUIDrawModule<LaunchProjectileModule, ProjectileCustomEditorContext>>()
                {
                    new ProjectileThrowRange(),
                    new ProjectileEffectRange()
                };

                var launchProjectile = (LaunchProjectileModule)target;
                this.context = new ProjectileCustomEditorContext();
                this.context.PlayerObject = GameObject.FindObjectOfType<PlayerManager>();
                this.context.LaunchProjectile = launchProjectile;
                var projectileConfiguraiton = AssetFinder.SafeSingleAssetFind<LaunchProjectileConfiguration>("t:" + typeof(LaunchProjectileConfiguration).Name);
                if (projectileConfiguraiton != null && projectileConfiguraiton.ConfigurationInherentData.ContainsKey(launchProjectile.LaunchProjectileID))
                {
                    this.context.ProjectileInherentData = projectileConfiguraiton.ConfigurationInherentData[launchProjectile.LaunchProjectileID];
                }
            }
        }
    }

    public class ProjectileCustomEditorContext
    {
        public PlayerManager PlayerObject;
        public LaunchProjectileModule LaunchProjectile;
        public LaunchProjectileInherentData ProjectileInherentData;
    }

    class ProjectileThrowRange : GUIDrawModule<LaunchProjectileModule, ProjectileCustomEditorContext>
    {
        public override void SceneGUI(ProjectileCustomEditorContext context, LaunchProjectileModule target)
        {
            Handles.color = Color.magenta;
            var position = context.LaunchProjectile.transform.position;
            if (context.PlayerObject != null)
            {
                position = context.PlayerObject.transform.position;
            }
            Handles.Label(position + Vector3.up * context.ProjectileInherentData.ProjectileThrowRange, nameof(context.ProjectileInherentData.ProjectileThrowRange), MyEditorStyles.LabelMagenta);
            Handles.DrawWireDisc(position, Vector3.up, context.ProjectileInherentData.ProjectileThrowRange);
        }
    }

    class ProjectileEffectRange : GUIDrawModule<LaunchProjectileModule, ProjectileCustomEditorContext>
    {
        public override void SceneGUI(ProjectileCustomEditorContext context, LaunchProjectileModule target)
        {
            Handles.color = Color.red;
            Handles.Label(context.LaunchProjectile.transform.position + Vector3.up * context.ProjectileInherentData.ExplodingEffectRange, nameof(context.ProjectileInherentData.ExplodingEffectRange), MyEditorStyles.LabelRed);
            Handles.DrawWireDisc(context.LaunchProjectile.transform.position, Vector3.up, context.ProjectileInherentData.ExplodingEffectRange);
        }
    }
}