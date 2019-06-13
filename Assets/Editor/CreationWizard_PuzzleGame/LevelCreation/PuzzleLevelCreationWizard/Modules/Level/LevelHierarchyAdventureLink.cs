using UnityEditor;

namespace Editor_PuzzleLevelCreationWizard
{
    [System.Serializable]
    public class LevelHierarchyAdventureLink : CreationModuleComponent
    {

        public override void ResetEditor()
        { }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
        }

        public override void OnGenerationClicked(AbstractCreationWizardEditorProfile editorProfile)
        {
            base.OnGenerationClicked(editorProfile);
            var editorInformations = editorProfile.GetModule<EditorInformations>().EditorInformationsData;
            editorInformations.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration.AddPuzzleChunkLevel(editorInformations.AssociatedAdventureLevelID, editorInformations.LevelZoneChunkID);
            editorProfile.LevelHierarchyAdded(editorInformations.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration, editorInformations.AssociatedAdventureLevelID, editorInformations.LevelZoneChunkID);
        }
    }
}