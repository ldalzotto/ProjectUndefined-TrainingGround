using Editor_MainGameCreationWizard;
using RTPuzzle;
using UnityEditor;

namespace Editor_PuzzleCutsceneCreationWizard
{
    [System.Serializable]
    public class PuzzleCutsceneGraphCreation : CreateableScriptableObjectComponent<PuzzleCutsceneGraph>
    {
        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            var editorInfomrationsData = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            var PuzzleCutsceneConfiguration = editorInfomrationsData.CommonGameConfigurations.GetConfiguration<PuzzleCutsceneConfiguration>();

            var puzzleCutsceneGraph = this.CreateAsset(InstancePath.GetConfigurationDataPath(PuzzleCutsceneConfiguration), editorInfomrationsData.PuzzleCutsceneID.ToString() + NameConstants.PuzzleCutsceneGraph, editorProfile);
            var puzzleCutsceneConfiguration = editorProfile.GetModule<PuzzleCutsceneConfigurationCreation>().CreatedObject;
            puzzleCutsceneConfiguration.PuzzleCutsceneGraph = puzzleCutsceneGraph;
            EditorUtility.SetDirty(puzzleCutsceneConfiguration);
            AssetDatabase.SaveAssets();
        }
    }
}