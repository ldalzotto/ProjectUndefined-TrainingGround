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

namespace Editor_AIObjectTypeDefinitionCreationWizard
{
    [System.Serializable]
    public class AIObjectTypeDefinitionConfigurationCreation : CreateableScriptableObjectComponent<AIObjectTypeDefinitionInherentData>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            this.CreateAsset(InstancePath.AIObjectTypeDefinitionInherentDataPath, editorInfomrationsData.AIObjectTypeDefinitionID.ToString() + NameConstants.AIObjectTypeDefinitionInherentData, editorProfile);
            this.AddToGameConfiguration(editorInfomrationsData.AIObjectTypeDefinitionID, editorInfomrationsData.CommonGameConfigurations.PuzzleGameConfigurations.AIObjectTypeDefinitionConfiguration, editorProfile);
        }
    }
}