using UnityEngine;
using System.Collections;
using RTPuzzle;
using Editor_MainGameCreationWizard;
using UnityEditor;
using GameConfigurationID;

namespace Editor_LaunchProjectileCreationWizard
{
    [System.Serializable]
    public class ProjectileConfigurationDataCreation : CreateableScriptableObjectComponent<LaunchProjectileInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInformations = editorProfile.GetModule<EditorInformations>();
            this.CreateAsset(InstancePath.ProjectileInherentDataPath, editorInformations.EditorInformationsData.LaunchProjectileId.ToString() + NameConstants.ProjectileInherentData
                 , editorProfile);
            this.AddToGameConfiguration(editorInformations.EditorInformationsData.LaunchProjectileId, editorInformations.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LaunchProjectileConfiguration, editorProfile);
            SerializableObjectHelper.Modify(this.CreatedObject, (SerializedObject so) =>
            { 
                so.FindProperty(nameof(this.CreatedObject.PreActionAnimation)).enumValueIndex = (int)AnimationID.ACTION_CA_PROJECTILE;
                so.FindProperty(nameof(this.CreatedObject.PostActionAnimation)).enumValueIndex = (int)AnimationID.ACTION_CA_PROJECTILE_THROW;
            });
        }
    }
}