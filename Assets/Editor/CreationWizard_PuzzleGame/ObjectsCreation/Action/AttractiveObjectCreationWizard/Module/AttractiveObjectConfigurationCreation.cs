using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using UnityEditor;

namespace Editor_AttractiveObjectCreationWizard
{
    [System.Serializable]
    public class AttractiveObjectConfigurationCreation : CreateableScriptableObjectComponent<AttractiveObjectInherentConfigurationData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var AttractiveObjectConfiguration = editorInfomrationsData.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration;
            this.CreateAsset(InstancePath.GetConfigurationDataPath(AttractiveObjectConfiguration), editorInfomrationsData.AttractiveObjectId.ToString() + "_" + 
                this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.AttractiveObjectId, AttractiveObjectConfiguration, editorProfile);
            SerializableObjectHelper.Modify(this.CreatedObject, (SerializedObject so) =>
            {
                so.FindProperty(nameof(this.CreatedObject.PreActionAnimationGraph)).enumValueIndex = (int)PuzzleCutsceneID._GENERIC_AnimationWithFollowObject;
                so.FindProperty(nameof(this.CreatedObject.PostActionAnimationGraph)).enumValueIndex = (int)PuzzleCutsceneID._GENERIC_AnimationWithFollowObject;
                so.FindProperty(nameof(this.CreatedObject.PreActionAnimation)).enumValueIndex = (int)AnimationID.ACTION_CA_POCKET_ITEM;
                so.FindProperty(nameof(this.CreatedObject.PostActionAnimation)).enumValueIndex = (int)AnimationID.ACTION_CA_POCKET_ITEM_LAY;
            });
        }
    }
}