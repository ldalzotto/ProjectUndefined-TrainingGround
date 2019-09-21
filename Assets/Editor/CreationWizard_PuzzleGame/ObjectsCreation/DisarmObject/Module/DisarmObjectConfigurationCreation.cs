using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using UnityEditor;

namespace Editor_DisarmObjectCreationWizard
{
    [System.Serializable]
    public class DisarmObjectConfigurationCreation : CreateableScriptableObjectComponent<DisarmObjectInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var DisarmObjectConfiguration = editorInfomrationsData.CommonGameConfigurations.GetConfiguration<DisarmObjectConfiguration>();
            this.CreateAsset(InstancePath.GetConfigurationDataPath(DisarmObjectConfiguration),
                editorInfomrationsData.DisarmObjectID.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.DisarmObjectID, DisarmObjectConfiguration, editorProfile);
        }
    }
}