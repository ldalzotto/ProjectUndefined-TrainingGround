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
            var attractiveObjectPrefabCreation = editorProfile.GetModule<AttractiveObjectPrefabCreation>();
            this.CreateAsset(editorInfomrationsData.CommonGameConfigurations.InstancePath.AttractiveObjectInherantDataPath, editorInfomrationsData.AttractiveObjectId.ToString() + NameConstants.AttractiveObjectInherentData, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.AttractiveObjectId, editorInfomrationsData.CommonGameConfigurations.PuzzleGameConfigurations.AttractiveObjectConfiguration, editorProfile);
            this.CreatedObject.AttractiveObjectPrefab = attractiveObjectPrefabCreation.CreatedPrefab;
            SerializableObjectHelper.Modify(this.CreatedObject, (SerializedObject so) =>
            {
                so.FindProperty(nameof(this.CreatedObject.PreActionAnimation)).enumValueIndex = (int)AnimationID.PLAYER_ACTION_CA_POCKET_ITEM;
                so.FindProperty(nameof(this.CreatedObject.PostActionAnimation)).enumValueIndex = (int)AnimationID.PLAYER_ACTION_CA_POCKET_ITEM_LAY;
                so.FindProperty(nameof(this.CreatedObject.AttractiveObjectPrefab)).objectReferenceValue = attractiveObjectPrefabCreation.CreatedPrefab;
                so.FindProperty(nameof(this.CreatedObject.AttractiveObjectModelPrefab)).objectReferenceValue = editorInfomrationsData.AttractiveObjectModel;
            });
        }
    }
}