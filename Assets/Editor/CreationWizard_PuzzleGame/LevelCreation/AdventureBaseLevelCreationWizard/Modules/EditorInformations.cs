using CoreGame;
using CreationWizard;
using Editor_MainGameCreationWizard;
using GameConfigurationID;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Editor_AdventureBaseLevelCreationWizard
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
                ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LevelZonesID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelZonesSceneConfiguration>()),
                ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LevelZonesID, this.EditorInformationsData.CommonGameConfigurations.GetConfiguration<LevelHierarchyConfiguration>())
             }
            .Find((s) => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        [CustomEnum()]
        public LevelZonesID LevelZonesID;

        public CommonGameConfigurations CommonGameConfigurations;
    }

}
