//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Editor_MainGameCreationWizard;
using GameConfigurationID;
using RTPuzzle;
using UnityEditor;

namespace Editor_GrabObjectCreationWizard
{
    [System.Serializable]
    public class GrabObjectConfigurationCreation : CreateableScriptableObjectComponent<GrabObjectInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var GrabObjectConfiguration = editorInfomrationsData.CommonGameConfigurations.GetConfiguration<GrabObjectConfiguration>();
            this.CreateAsset(InstancePath.GetConfigurationDataPath(GrabObjectConfiguration), 
                editorInfomrationsData.GrabObjectID.ToString() + "_" + this.GetType().BaseType.GetGenericArguments()[0].Name, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.GrabObjectID, GrabObjectConfiguration, editorProfile);
        }
    }
}