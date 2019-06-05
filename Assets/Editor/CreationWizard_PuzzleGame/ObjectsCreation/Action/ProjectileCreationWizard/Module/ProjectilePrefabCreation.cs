using UnityEngine;
using System.Collections;
using RTPuzzle;
using System;
using Editor_MainGameCreationWizard;
using UnityEditor;

namespace Editor_ProjectileCreationWizard
{
    [System.Serializable]
    public class ProjectilePrefabCreation : CreateablePrefabComponent<LaunchProjectile>
    {
        public override Func<AbstractCreationWizardEditorProfile, LaunchProjectile> BasePrefabProvider
        {
            get
            {
                return (AbstractCreationWizardEditorProfile editorProfile) => editorProfile.GetModule<EditorInformations>().EditorInformationsData.CommonGameConfigurations.PuzzleLevelCommonPrefabs.BaseLaunchProjectilePrefab;
            }
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>();
            this.Create(editorInformations.EditorInformationsData.CommonGameConfigurations.InstancePath.ProjectilePrefabPath, editorInformations.EditorInformationsData.LaunchProjectileId.ToString() +
                NameConstants.ProjectilePrefab, editorProfile);
            this.CreatedPrefab.LaunchProjectileId = editorInformations.EditorInformationsData.LaunchProjectileId;
            PrefabUtility.SavePrefabAsset(this.CreatedPrefab.gameObject);
        }
    }
}