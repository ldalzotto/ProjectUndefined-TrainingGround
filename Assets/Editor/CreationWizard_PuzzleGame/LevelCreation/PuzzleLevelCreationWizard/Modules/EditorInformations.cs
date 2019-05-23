using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using CreationWizard;
using RTPuzzle;
using System.Linq;
using System;
using CoreGame;
using Editor_PuzzleGameCreationWizard;

namespace Editor_PuzzleLevelCreationWizard
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
            return new List<string>() {
                ErrorHelper.AlreadyPresentInConfiguration(this.EditorInformationsData.LevelZonesID,
                        this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelConfiguration)),
                ErrorHelper.AlreadyPresentInConfiguration(this.EditorInformationsData.LevelZonesID,
                        this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelZonesSceneConfiguration)),
                ErrorHelper.AlreadyPresentInConfiguration(this.EditorInformationsData.LevelZonesID,
                        this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.LevelHierarchyConfiguration)),
                ErrorHelper.AlreadyPresentInConfiguration(this.EditorInformationsData.LevelZoneChunkID,
                        this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration.ConfigurationInherentData.Keys.ToList().ConvertAll(e => (Enum)e),
                        nameof(this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ChunkZonesSceneConfiguration))

            }
            .Find((s) => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [SearchableEnum]
        public LevelZonesID LevelZonesID;
        [SearchableEnum]
        public LevelZoneChunkID LevelZoneChunkID;

        public CommonGameConfigurations CommonGameConfigurations;
    }

}
