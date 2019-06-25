using CreationWizard;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_LevelChunkCreationWizard
{
    [System.Serializable]
    public class EditorInformations : CreationModuleComponent
    {
        [SerializeField]
        public EditorInformationsData EditorInformationsData;

        protected override string headerDescriptionLabel => "Base informations used by the creation wizard.";

        public override void ResetEditor()
        {
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)), true);
        }

        private void InitProperties()
        {
            EditorInformationsHelper.InitProperties(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        public override string ComputeErrorState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            string associatedPuzzleLevelWargning = string.Empty;
            if (this.EditorInformationsData.AssociatedPuzzleLevelID != LevelZonesID.NONE)
            {
                associatedPuzzleLevelWargning = ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AssociatedPuzzleLevelID, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration.GetKeys(), nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration));
            }

            return new List<string>() {
                associatedPuzzleLevelWargning,
                ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AssociatedAdventureLevelID,this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration.GetKeys(), nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration)),
                ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LevelZoneChunkID, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration),
                ErrorHelper.NotAlreadyPresentInConfiguration(this.EditorInformationsData.AssociatedAdventureLevelID, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration.GetKeys(), nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration))
            }
            .Find((s) => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [CustomEnum(isCreateable: true)]
        public LevelZoneChunkID LevelZoneChunkID;
        [CustomEnum(isCreateable: true)]
        public LevelZonesID AssociatedAdventureLevelID;
        [CustomEnum(isCreateable: true)]
        public LevelZonesID AssociatedPuzzleLevelID = LevelZonesID.NONE;

        public CommonGameConfigurations CommonGameConfigurations;
    }

}
