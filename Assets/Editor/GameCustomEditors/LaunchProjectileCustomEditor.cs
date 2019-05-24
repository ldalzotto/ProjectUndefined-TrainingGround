using UnityEngine;
using System.Collections;
using UnityEditor;
using RTPuzzle;
using Editor_GameDesigner;
using System.Collections.Generic;

namespace Editor_GameCustomEditors
{
    [ExecuteInEditMode]
    [CustomEditor(typeof(LaunchProjectile))]
    public class LaunchProjectileCustomEditor : AbstractGameCustomEditorWithLiveSelection<LaunchProjectile, ProjectileCustomEditorContext, ProjectileConfiguration, EditProjectile>
    {
        private void OnEnable()
        {
            if (target != null)
            {
                this.drawModules = new List<GUIDrawModule<LaunchProjectile, ProjectileCustomEditorContext>>()
                {
                    new ProjectileThrowRange(),
                    new ProjectileEffectRange(),
                    new ProjectileEscapeAngle()
                };

                var launchProjectile = (LaunchProjectile)target;
                this.context = new ProjectileCustomEditorContext();
                this.context.PlayerObject = GameObject.FindObjectOfType<PlayerManager>();
                this.context.LaunchProjectile = launchProjectile;
                var projectileConfiguraiton = AssetFinder.SafeSingleAssetFind<ProjectileConfiguration>("t:" + typeof(ProjectileConfiguration).Name);
                if (projectileConfiguraiton != null && projectileConfiguraiton.ConfigurationInherentData.ContainsKey(launchProjectile.LaunchProjectileId))
                {
                    this.context.ProjectileInherentData = projectileConfiguraiton.ConfigurationInherentData[launchProjectile.LaunchProjectileId];
                }
            }
        }
    }

    public class ProjectileCustomEditorContext
    {
        public PlayerManager PlayerObject;
        public LaunchProjectile LaunchProjectile;
        public ProjectileInherentData ProjectileInherentData;
    }

    class ProjectileThrowRange : GUIDrawModule<LaunchProjectile, ProjectileCustomEditorContext>
    {
        public override void SceneGUI(ProjectileCustomEditorContext context, LaunchProjectile target)
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

    class ProjectileEffectRange : GUIDrawModule<LaunchProjectile, ProjectileCustomEditorContext>
    {
        public override void SceneGUI(ProjectileCustomEditorContext context, LaunchProjectile target)
        {
            Handles.color = Color.red;
            Handles.Label(context.LaunchProjectile.transform.position + Vector3.up * context.ProjectileInherentData.EffectRange, nameof(context.ProjectileInherentData.EffectRange), MyEditorStyles.LabelRed);
            Handles.DrawWireDisc(context.LaunchProjectile.transform.position, Vector3.up, context.ProjectileInherentData.EffectRange);
        }
    }

    class ProjectileEscapeAngle : GUIDrawModule<LaunchProjectile, ProjectileCustomEditorContext>
    {
        public override void SceneGUI(ProjectileCustomEditorContext context, LaunchProjectile target)
        {
            Handles.color = Color.yellow;
            Handles.Label(context.LaunchProjectile.transform.position + Vector3.up * 5f, nameof(context.ProjectileInherentData.EscapeSemiAngle), MyEditorStyles.LabelYellow);
            Handles.DrawWireArc(context.LaunchProjectile.transform.position, Vector3.up, context.LaunchProjectile.transform.forward, context.ProjectileInherentData.EscapeSemiAngle, 5f);
            Handles.DrawWireArc(context.LaunchProjectile.transform.position, Vector3.up, context.LaunchProjectile.transform.forward, -context.ProjectileInherentData.EscapeSemiAngle, 5f);

        }
    }
}