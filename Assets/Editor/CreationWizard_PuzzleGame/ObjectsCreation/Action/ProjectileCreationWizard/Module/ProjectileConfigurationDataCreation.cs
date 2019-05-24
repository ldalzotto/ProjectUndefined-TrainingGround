using UnityEngine;
using System.Collections;
using RTPuzzle;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;

namespace Editor_ProjectileCreationWizard
{
    [System.Serializable]
    public class ProjectileConfigurationDataCreation : CreateableScriptableObjectComponent<ProjectileInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>();
            var projectilePrefabCreation = editorProfile.GetModule<ProjectilePrefabCreation>();
            this.CreateAsset(editorInformations.EditorInformationsData.CommonGameConfigurations.InstancePath.ProjectileInherentDataPath, editorInformations.EditorInformationsData.LaunchProjectileId.ToString() + NameConstants.ProjectileInherentData
                 , editorProfile);
            this.AddToGameConfiguration(editorInformations.EditorInformationsData.LaunchProjectileId, editorInformations.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ProjectileConfiguration, editorProfile);
            SerializableObjectHelper.Modify(this.CreatedObject, (SerializedObject so) =>
            {
                so.FindProperty("projectilePrefab").objectReferenceValue = projectilePrefabCreation.CreatedPrefab;
                so.FindProperty(nameof(this.CreatedObject.PreActionAnimation)).enumValueIndex = (int)PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_PROJECTILE;
                so.FindProperty(nameof(this.CreatedObject.PostActionAnimation)).enumValueIndex = (int)PlayerAnimatioNamesEnum.PLAYER_ACTION_CA_PROJECTILE_THROW;
            });
        }
    }
}