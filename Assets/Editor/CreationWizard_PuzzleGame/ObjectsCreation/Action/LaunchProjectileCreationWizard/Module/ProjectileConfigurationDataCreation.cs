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
            var launchProjectileConfiguration = editorInformations.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LaunchProjectileConfiguration;
            this.CreateAsset(InstancePath.GetConfigurationDataPath(launchProjectileConfiguration), editorInformations.EditorInformationsData.LaunchProjectileId.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name
                 , editorProfile);
            this.AddToGameConfiguration(editorInformations.EditorInformationsData.LaunchProjectileId, launchProjectileConfiguration, editorProfile);
            SerializableObjectHelper.Modify(this.CreatedObject, (SerializedObject so) =>
            { 
                so.FindProperty(nameof(this.CreatedObject.PreActionAnimation)).enumValueIndex = (int)AnimationID.ACTION_CA_PROJECTILE;
                so.FindProperty(nameof(this.CreatedObject.PostActionAnimation)).enumValueIndex = (int)AnimationID.ACTION_CA_PROJECTILE_THROW;
                so.FindProperty(nameof(this.CreatedObject.PostActionAnimationV2)).enumValueIndex = (int)PuzzleCutsceneID._GENERIC_AnimationWithFollowObject;
                so.FindProperty(nameof(this.CreatedObject.PreActionAnimationV2)).enumValueIndex = (int)PuzzleCutsceneID._GENERIC_AnimationWithFollowObject;
            });
        }
    }
}