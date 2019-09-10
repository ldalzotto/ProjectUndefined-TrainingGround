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
            var puzzleCutsceneGraph = this.CreateAsset(InstancePath.PuzzleCutsceneInherentDataPath, editorInfomrationsData.PuzzleCutsceneID.ToString() + NameConstants.PuzzleCutsceneGraph, editorProfile);
            var puzzleCutsceneConfiguration = editorProfile.GetModule<PuzzleCutsceneConfigurationCreation>().CreatedObject;
            puzzleCutsceneConfiguration.PuzzleCutsceneGraph = puzzleCutsceneGraph;
            EditorUtility.SetDirty(puzzleCutsceneConfiguration);
            AssetDatabase.SaveAssets();
        }
    }
}