using UnityEngine;
using System.Collections;
using Editor_PuzzleGameCreationWizard;
using UnityEditor;
using RTPuzzle;
using System.Collections.Generic;
using CreationWizard;

namespace Editor_ProjectileCreationWizard
{
    [System.Serializable]
    public class EditorInformations : CreationModuleComponent
    {
        public EditorInformationsData EditorInformationsData;

        public override void ResetEditor()
        {
        }

        private void InitProperties()
        {
            EditorInformationsHelper.InitProperties(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        protected override void OnInspectorGUIImpl(SerializedObject serializedObject, AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(this.EditorInformationsData)), true);
        }

        public override string ComputeErrorState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return EditorInformationsHelper.ComputeErrorState(ref this.EditorInformationsData.CommonGameConfigurations);
        }

        public override string ComputeWarningState(AbstractCreationWizardEditorProfile editorProfile)
        {
            this.InitProperties();
            return new List<string>()
            {
              ErrorHelper.AlreadyPresentInConfigurationV2(this.EditorInformationsData.LaunchProjectileId, this.EditorInformationsData.CommonGameConfigurations.PuzzleGameConfigurations.ProjectileConfiguration)
            }.Find(s => !string.IsNullOrEmpty(s));
        }
    }

    [System.Serializable]
    public class EditorInformationsData
    {
        public LaunchProjectileId LaunchProjectileId;
        public CommonGameConfigurations CommonGameConfigurations;
    }

}